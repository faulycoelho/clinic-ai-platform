using Clinic.Application.Interfaces;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
         

        public async Task<float[]> GenerateEmbeddingAsync(
            string text,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("O texto não pode ser vazio.", nameof(text));

            var endpoint =
                $"https://generativelanguage.googleapis.com/v1beta/models/{_llmOptions.Model}:embedContent?key={_llmOptions.ApiKey}";

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
