namespace organization_back_end.AIHelpers;

public class GeminiResponse
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
    public dynamic Data { get; set; }
}