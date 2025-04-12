namespace organization_back_end.AIHelpers;

public class GenerationConfig
{
    public double Temperature { get; set; }
    public int TopK { get; set; }
    public double TopP { get; set; }
    public int MaxOutputTokens { get; set; }
    public required string ResponseMimeType { get; set; }
}