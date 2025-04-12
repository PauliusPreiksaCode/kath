using organization_back_end.AIHelpers;

namespace organization_back_end.Interfaces;

public interface IAIService
{
    Task<GeminiResponse> GetResponseAsync(string promptText);
}