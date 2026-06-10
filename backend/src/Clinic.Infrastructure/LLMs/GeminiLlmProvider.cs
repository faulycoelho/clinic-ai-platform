using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Clinic.Domain.Enums.ConversationEnums;

namespace Clinic.Infrastructure.LLMs
{
    public class GeminiLlmProvider : ILLMProvider
    {
        private readonly LlmOptions _llmOptions;
        private readonly HttpClient _httpClient;


        public GeminiLlmProvider(LlmOptions llmOptions, HttpClient httpClient)
        {
            _llmOptions = llmOptions;
            _httpClient = httpClient;
        }

        public async Task<LlmResponse> ChatAsync(LlmChatRequest request, CancellationToken ct = default)
        {
            var contents = BuildContents(request);
            var tools = BuildTools(request.Tools);
            var systemInstruction = new { parts = new[] { new { text = request.SystemPrompt } } };

            var generationConfig = new { maxOutputTokens = _llmOptions.MaxTokens };

            var payload = tools.Count > 0
                ? (object)new { contents, tools, systemInstruction, generationConfig }
                : new { contents, systemInstruction, generationConfig };

            var model = _llmOptions.Model;
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={_llmOptions.ApiKey}";

            var response = await _httpClient.PostAsJsonAsync(url, payload, ct);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                throw new HttpRequestException(
                    $"Gemini API {(int)response.StatusCode}: {errorBody}",
                    null, response.StatusCode);
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>(ct);
            return ParseResponse(json);
        }

        internal static List<object> BuildTools(IReadOnlyList<LlmToolDefinition> tools)
        {
            if (tools.Count == 0) return [];

            var functionDeclarations = tools.Select(t =>
            {
                var schema = JsonSerializer.Deserialize<JsonElement>(t.ParametersJsonSchema);
                return (object)new
                {
                    name = t.Name,
                    description = t.Description,
                    parameters = schema
                };
            }).ToList();

            return [new { functionDeclarations }];
        }

        internal static List<object> BuildContents(LlmChatRequest request)
        {
            var contents = new List<object>();

            foreach (var msg in request.History)
            {
                var role = msg.Role switch
                {
                    MessageRole.User => "user",
                    MessageRole.Assistant => "model",
                    _ => (string?)null
                };
                if (role is null) continue;
                contents.Add(new { role, parts = new[] { new { text = msg.Content } } });
            }

            contents.Add(new { role = "user", parts = new[] { new { text = request.UserMessage } } });

            if (request.PreviousAssistantToolCalls is { Count: > 0 } toolCalls
                && request.ToolResults is { Count: > 0 } toolResults)
            {
                var modelParts = toolCalls.Select(tc =>
                    tc.RawPartJson is not null
                        ? JsonSerializer.Deserialize<JsonElement>(tc.RawPartJson)
                        : JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(new
                        {
                            functionCall = new
                            {
                                name = tc.Name,
                                args = JsonSerializer.Deserialize<JsonElement>(tc.ArgumentsJson)
                            }
                        }))
                ).ToList();

                contents.Add(new { role = "model", parts = modelParts });

                var responseParts = toolResults.Select(tr =>
                {
                    var parsed = JsonSerializer.Deserialize<JsonElement>(tr.ContentJson);
                    var responseObj = parsed.ValueKind == JsonValueKind.Object
                        ? parsed
                        : JsonSerializer.Deserialize<JsonElement>(
                            JsonSerializer.Serialize(new { result = parsed }));

                    return (object)new
                    {
                        functionResponse = new
                        {
                            name = tr.ToolName,
                            response = responseObj
                        }
                    };
                }).ToList();

                contents.Add(new { role = "user", parts = responseParts });
            }

            return contents;
        }

        internal static LlmResponse ParseResponse(JsonElement json)
        {
            string? textContent = null;
            var toolCalls = new List<LlmToolCall>();

            if (json.TryGetProperty("candidates", out var candidates)
                && candidates.ValueKind == JsonValueKind.Array
                && candidates.GetArrayLength() > 0)
            {
                var candidate = candidates[0];

                if (candidate.TryGetProperty("content", out var content)
                    && content.TryGetProperty("parts", out var parts)
                    && parts.ValueKind == JsonValueKind.Array)
                {
                    foreach (var part in parts.EnumerateArray())
                    {
                        if (part.TryGetProperty("text", out var textEl))
                        {
                            textContent = textEl.GetString();
                        }
                        else if (part.TryGetProperty("functionCall", out var fc))
                        {
                            var name = fc.GetProperty("name").GetString() ?? string.Empty;
                            var args = fc.TryGetProperty("args", out var argsEl)
                                ? argsEl.GetRawText()
                                : "{}";
                            toolCalls.Add(new LlmToolCall(
                                Guid.NewGuid().ToString(), name, args,
                                RawPartJson: part.GetRawText()));
                        }
                    }
                }
            }

            var stopReason = toolCalls.Count > 0 ? LlmStopReason.ToolUse : LlmStopReason.EndTurn;

            return new LlmResponse
            {
                TextContent = textContent,
                ToolCalls = toolCalls,
                StopReason = stopReason
            };
        }

        public async Task<float[]> GenerateEmbeddingAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("O texto não pode ser vazio.", nameof(text));

            var endpoint =
                $"https://generativelanguage.googleapis.com/v1beta/models/{_llmOptions.ModelEmbedding}:embedContent?key={_llmOptions.ApiKey}";

            var request = new EmbedRequest
            {
                Content = new Content
                {
                    Parts =
                    [
                        new Part
                        {
                            Text = text
                        }
                    ]
                }
            };

            var json = JsonSerializer.Serialize(request);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            using var response = await _httpClient.SendAsync(
                httpRequest,
                cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Erro ao gerar embedding. Status: {response.StatusCode}. " +
                    $"Resposta: {responseContent}");
            }

            var embeddingResponse = JsonSerializer.Deserialize<EmbedResponse>(
                responseContent,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (embeddingResponse?.Embedding?.Values == null ||
                embeddingResponse.Embedding.Values.Length == 0)
            {
                throw new Exception("A API retornou um embedding vazio.");
            }

            return embeddingResponse.Embedding.Values;
        }

        #region DTOs

        private class EmbedRequest
        {
            [JsonPropertyName("content")]
            public Content Content { get; set; } = default!;
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public Part[] Parts { get; set; } = Array.Empty<Part>();
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string Text { get; set; } = string.Empty;
        }

        private class EmbedResponse
        {
            [JsonPropertyName("embedding")]
            public EmbeddingData? Embedding { get; set; }
        }

        private class EmbeddingData
        {
            [JsonPropertyName("values")]
            public float[] Values { get; set; } = Array.Empty<float>();
        }

        #endregion
    }
}
