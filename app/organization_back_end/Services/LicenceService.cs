using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using organization_back_end.Enums;
using organization_back_end.Interfaces;
using organization_back_end.RequestDtos.Licences;
using Session = Stripe.Checkout.Session;

namespace organization_back_end.Services;

public class LicenceService : ILicenceService
{
    private readonly SystemContext _context;
    private readonly UserManager<User> _userManager;

    public LicenceService(SystemContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task RemoveLicence(Guid licenceLedgerId, string userId)
    {
        var ledgerEntries = await _context.LicenceLedgerEntries
            .Where(l => l.Id.Equals(licenceLedgerId) && l.UserId.Equals(userId) && l.IsActive)
            .ToListAsync();

        foreach (var ledgerEntry in ledgerEntries)
        {
            ledgerEntry.IsActive = false;
            await _context.SaveChangesAsync();
        }
        
        await UpdateUserRoles(userId);
    }
    
    public async Task TransferLicence(string userId, string newUserId, Guid ledgerEntryId)
    {
        var oldLedgerEntry = await _context.LicenceLedgerEntries
            .FirstOrDefaultAsync(l => l.UserId.Equals(userId) && l.Id.Equals(ledgerEntryId));
        
        if (oldLedgerEntry is null)
            return;
        
        var newLedgerEntry = new LicenceLedgerEntry
        {
            Id = Guid.NewGuid(),
            PurchaseDate = oldLedgerEntry.PurchaseDate,
            IsActive = oldLedgerEntry.IsActive,
            PaymentStatus = LicencePaymentStatus.Received,
            LicenceId = oldLedgerEntry.LicenceId,
            UserId = newUserId
        };
        
        await _context.LicenceLedgerEntries.AddAsync(newLedgerEntry);
        
        oldLedgerEntry.IsActive = false;
        oldLedgerEntry.PaymentStatus = LicencePaymentStatus.Transferred;
        
        await _context.SaveChangesAsync();
        
        await UpdateUserRoles(userId);
        await UpdateUserRoles(newUserId);
    }
    
    public async Task GenerateLicense(CreateLicenceRequest request)
    {
        var licence = new Licence
        {
            Id = new Guid(),
            Name = request.Name,
            Price = request.Price,
            Type = request.Type,
            Duration = request.Duration
        };
        
        await _context.Licences.AddAsync(licence);
        await _context.SaveChangesAsync();
    }
    
    public async Task<ICollection<Licence>> GetAllLicences()
    {
        var licences = await _context.Licences.ToListAsync();
        return licences;
    }
    
    public async Task<Licence?> GetLicenceById(Guid id)
    {
        var licence = await _context.Licences.FirstOrDefaultAsync(l => l.Id.Equals(id));
        return licence;
    }
    
    public async Task<Guid> CreateInitialLicenceLedgerEntry(Licence licence, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Guid.Empty;
        
        var ledgerEntry = new LicenceLedgerEntry
        {
            Id = Guid.NewGuid(),
            PurchaseDate = DateTime.Now,
            IsActive = false,
            PaymentStatus = LicencePaymentStatus.None,
            LicenceId = licence.Id,
            Licence = licence,
            UserId = userId
        };
        
        await _context.LicenceLedgerEntries.AddAsync(ledgerEntry);
        await _context.SaveChangesAsync();
        return ledgerEntry.Id;
    }

    public async Task UpdateLicenceLedgerEntry(Guid id, LicencePaymentStatus status, Session session)
    {
        var ledgerEntry = await _context.LicenceLedgerEntries
            .Include(x => x.Licence)
            .FirstOrDefaultAsync(l => l.Id.Equals(id));
        
        if (ledgerEntry is null)
            return;
        
        if (status == LicencePaymentStatus.Paid)
        {
            ledgerEntry.IsActive = true;
            ledgerEntry.PaymentStatus = LicencePaymentStatus.Paid;

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                PaymentDate = session.Created,
                Amount = (decimal)session.AmountTotal / 100,
                PaymentNumberStripe = session.PaymentIntentId,
                LicenceLegerEntryId = ledgerEntry.Id
            };
            
            await _context.Payments.AddAsync(payment);
            ledgerEntry.Payment = payment;
            
            var user = await _userManager.FindByIdAsync(ledgerEntry.UserId);
            if (ledgerEntry.Licence.Type == LicenceType.Organization)
            {
                if (!(await _userManager.IsInRoleAsync(user, Roles.OrganizationOwner)))
                {
                    await _userManager.AddToRoleAsync(user, Roles.OrganizationOwner);
                    
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE AspNetUsers SET UserType = {0} WHERE Id = {1}", 
                        "OrganizationOwner", ledgerEntry.UserId
                    );
                }
            }
            else
            {
                if (!(await _userManager.IsInRoleAsync(user, Roles.LicencedUser)))
                {
                    await _userManager.AddToRoleAsync(user, Roles.LicencedUser);
                    
                    await _context.Database.ExecuteSqlRawAsync(
                        "UPDATE AspNetUsers SET UserType = {0} WHERE Id = {1}", 
                        "LicencedUser", ledgerEntry.UserId
                    );
                }
            }
            
        } 
        else if (status == LicencePaymentStatus.Unpaid)
        {
            ledgerEntry.IsActive = false;
            ledgerEntry.PaymentStatus = LicencePaymentStatus.Unpaid;
        }
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<ICollection<LicenceLedgerEntry>> GetLicenceLedgerEntries(string userId)
    {
        return userId.IsNullOrEmpty() ? 
            await _context.LicenceLedgerEntries.ToListAsync() :
            await _context.LicenceLedgerEntries
                .Include(l => l.Licence)
                .Where(l => l.UserId.Equals(userId))
                .OrderByDescending(l => l.PurchaseDate)
                .ToListAsync();
    }

    public async Task ValidateLicenceExpiration(string userId)
    {
        var activeLedgerEntries = await _context.LicenceLedgerEntries.Where(l => l.UserId.Equals(userId) && l.IsActive)
            .Include(l => l.Licence)
            .ToListAsync();
        
        bool changed = false;
        
        foreach (var ledgerEntry in activeLedgerEntries)
        {
            DateTime expirationDate = ledgerEntry.PurchaseDate.AddDays(ledgerEntry.Licence.Duration);
            
            if (expirationDate < DateTime.Now)
            {
                changed = true;
                ledgerEntry.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
        
        if (changed)
            await UpdateUserRoles(userId);
    }

    private async Task UpdateUserRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        
        if (user is null)
            return;
        
        var hasActiveOrganizationLicence = await _context.LicenceLedgerEntries
            .AnyAsync(l => l.UserId.Equals(userId) && l.IsActive && l.Licence.Type == LicenceType.Organization);
        var hasOrganizationRole = await _userManager.IsInRoleAsync(user, Roles.OrganizationOwner);
        
        if (hasOrganizationRole)
        {
            if (!hasActiveOrganizationLicence)
            {
                await _userManager.RemoveFromRoleAsync(user, Roles.OrganizationOwner);
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE AspNetUsers SET UserType = {0} WHERE Id = {1}", 
                    "LicencedUser", userId
                );
            }
        }
        else
        {
            if(hasActiveOrganizationLicence)
            {
                await _userManager.AddToRoleAsync(user, Roles.OrganizationOwner);
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE AspNetUsers SET UserType = {0} WHERE Id = {1}", 
                    "OrganizationOwner", userId
                );
            }
        }
        
        
        var hasActiveUserLicence = await _context.LicenceLedgerEntries
            .AnyAsync(l => l.UserId.Equals(userId) && l.IsActive && l.Licence.Type == LicenceType.User);
        
        var hasLicenceRole = await _userManager.IsInRoleAsync(user, Roles.LicencedUser);
        if (hasLicenceRole)
        {
            if (!hasActiveUserLicence && !hasOrganizationRole)
            {
                await _userManager.RemoveFromRoleAsync(user, Roles.LicencedUser);
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE AspNetUsers SET UserType = {0} WHERE Id = {1}", 
                    "User", userId
                );
            }
        }
        else
        {
            if(hasActiveUserLicence && !hasOrganizationRole)
            {
                await _userManager.AddToRoleAsync(user, Roles.LicencedUser);
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE AspNetUsers SET UserType = {0} WHERE Id = {1}", 
                    "LicencedUser", userId
                );
            }
        }
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> HasRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return false;
        
        return await _userManager.IsInRoleAsync(user, role);
    }
}