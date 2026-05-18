namespace Clinic.Infrastructure.LLMs;

public sealed class LlmOptions
{
    public string Provider { get; set; } = "";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}
