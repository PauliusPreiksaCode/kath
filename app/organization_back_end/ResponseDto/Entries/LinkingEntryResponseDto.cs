namespace organization_back_end.ResponseDto.Entries;

public class LinkingEntryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; }
    public DateTime CreationDate { get; set; }
}