using Clinic.Application.DTOs;
using Clinic.Application.Interfaces;
using System.Text;
using System.Text.Json;

namespace Clinic.Application.Tools
{
    public sealed class SearchKnowledgeBaseTool(
        IKnowledgeDocumentRepository knowledgeDocumentRepository,
        ILLMProvider llmProvider) : IConversationTool
    {
        public string Name => "search_knowledge_base";

        public string Description => "Searches the clinic's internal knowledge base to answer questions about internal policies, procedures, clinical questions, or general guidelines.";

        public string ParametersJsonSchema => """
        {
          "type": "object",
          "properties": {
            "query": {
              "type": "string",
              "description": "The search query to find relevant knowledge base articles"
            }
          },
          "required": ["query"]
        }
        """;    
        public async Task<string> ExecuteAsync(string argumentsJson, ConversationContextDto? context = null, CancellationToken ct = default)
        {
            var args = JsonSerializer.Deserialize<Args>(argumentsJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var query = args?.Query ?? "";
            if (string.IsNullOrWhiteSpace(query))
                return JsonSerializer.Serialize(new { message = "No query provided." });

            var userMessageEmb = await llmProvider.GenerateEmbeddingAsync(query, ct);
            var docs = await knowledgeDocumentRepository.SearchByVectorAsync(userMessageEmb);

            var output = docs.Select(d => new
            {
                d.DocumentId,
                d.DocumentTitle,
                d.Content,                
            });

            return JsonSerializer.Serialize(output);
        }

        private sealed record Args(string? Query);
    }
}