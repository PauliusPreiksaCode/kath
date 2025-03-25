using organization_back_end.Entities;
using organization_back_end.Enums;
using organization_back_end.RequestDtos.Licences;
using Session = Stripe.Checkout.Session;

namespace organization_back_end.Interfaces;

public interface ILicenceService
{
    Task RemoveLicence(Guid licenceLedgerId, string userId);
    Task TransferLicence(string userId, string newUserId, Guid ledgerEntryId);
    Task GenerateLicense(CreateLicenceRequest request);
    Task<ICollection<Licence>> GetAllLicences();
    Task<Licence?> GetLicenceById(Guid id);
    Task<Guid> CreateInitialLicenceLedgerEntry(Licence licence, string userId);
    Task UpdateLicenceLedgerEntry(Guid id, LicencePaymentStatus status, Session session);
    Task<ICollection<LicenceLedgerEntry>> GetLicenceLedgerEntries(string userId);
    Task ValidateLicenceExpiration(string userId);
    Task<bool> HasRole(string userId, string role);
}