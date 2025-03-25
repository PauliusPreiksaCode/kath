using Microsoft.AspNetCore.Mvc;
using organization_back_end.RequestDtos.Entry;
using organization_back_end.ResponseDto.Entries;

namespace organization_back_end.Interfaces;

public interface IEntryService
{
    Task<ICollection<EntryResponseDto>> GetEntries(Guid organizationId, Guid groupId);
    Task<FileStreamResult> DownloadFile(Guid groupId, Guid entryId);
    Task CreateEntry(AddEntryRequest request, string licencedUserId);
    Task UpdateEntry(UpdateEntryRequest request, string userId);
    Task DeleteEntry(Guid entryId, Guid groupId, string userId);
    Task DeleteFile(Guid entryId, Guid groupId, string userId);
    Task<IEnumerable<LinkingEntryResponseDto>> LinkingEntries(Guid organizationId, Guid? entryToExclude);
    Task<IEnumerable<GraphEntitiesResponse>> GetGraphEntities(Guid organizationId);
    Task<EntryViewResponseDto> GetEntry(Guid entryId, Guid organizationId);
}