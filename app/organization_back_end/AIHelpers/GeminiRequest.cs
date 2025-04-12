namespace organization_back_end.AIHelpers;

public class GeminiRequest
{
    public Content[] Contents { get; set; }
    public GenerationConfig GenerationConfig { get; set; }
}