namespace organization_back_end.ResponseDto.Entries;

public class GraphEntitiesResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<Guid>? LinkedEntries { get; set; }
}