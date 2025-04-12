using System.Text;
using System.Text.Json;
using organization_back_end.AIHelpers;
using organization_back_end.Interfaces;

namespace organization_back_end.Services;

public class GeminiAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string? _apiKey;
    private readonly string? _apiUrl;

    public GeminiAIService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _apiKey = configuration["GeminiAI:ApiKey"];
        _apiUrl = configuration["GeminiAI:ApiUrl"];
    }

    public async Task<GeminiResponse> GetResponseAsync(string promptText)
    {
        try
        {
            // Create the request
            var request = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = promptText }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.2,
                    maxOutputTokens = 2048,
                    responseMimeType = "application/json"
                }
            };

            var requestJson = JsonSerializer.Serialize(request);
            HttpContent requestContent = new StringContent(
                requestJson, 
                Encoding.UTF8, 
                "application/json"
            );
            
            var response = await _httpClient.PostAsync($"{_apiUrl}?key={_apiKey}", requestContent);
            
            

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Error: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiApiResponse>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var structured = ParseJsonResponse(geminiResponse);
            return structured;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling Gemini API: {ex.Message}");
            throw;
        }
    }

    private GeminiResponse ParseJsonResponse(GeminiApiResponse apiResponse)
    {
        try
        {
            var responseText = apiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text;
                
            if (string.IsNullOrEmpty(responseText))
            {
                return new GeminiResponse
                {
                    Success = false,
                    ErrorMessage = "Empty response from Gemini API"
                };
            }

            try
            {
                return JsonSerializer.Deserialize<GeminiResponse>(responseText, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return new GeminiResponse
                {
                    Success = true,
                    Data = responseText
                };
            }
        }
        catch (Exception ex)
        {
            return new GeminiResponse
            {
                Success = false,
                ErrorMessage = $"Failed to parse Gemini response: {ex.Message}"
            };
        }
    }
}












