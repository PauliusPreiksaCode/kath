using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;
using File = organization_back_end.Entities.File;

namespace organization_back_end;

public class SystemContext : IdentityDbContext<User>
{
    public SystemContext()
    {
    }
    
    public SystemContext(DbContextOptions<SystemContext> options) : base(options)
    {
    }
    
    public virtual DbSet<Entry> Entries { get; set; }
    public virtual DbSet<File> Files { get; set; }
    public virtual DbSet<Group> Groups { get; set; }
    public virtual DbSet<Licence> Licences { get; set; }
    public virtual DbSet<LicenceLedgerEntry> LicenceLedgerEntries { get; set; }
    public virtual DbSet<LicencedUser> LicencedUsers { get; set; }
    public virtual DbSet<NoteBook> NoteBooks { get; set; }
    public virtual DbSet<NotebookEntry> NotebookEntries { get; set; }
    public virtual DbSet<Organization> Organizations { get; set; }
    public virtual DbSet<OrganizationOwner> OrganizationOwners { get; set; }
    public virtual DbSet<OrganizationUser> OrganizationUsers { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Session> Session { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LicencedUser>()
            .HasBaseType<User>();

        builder.Entity<OrganizationOwner>()
            .HasBaseType<LicencedUser>();
        
        builder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<User>("User")
            .HasValue<LicencedUser>("LicencedUser")
            .HasValue<OrganizationOwner>("OrganizationOwner");

        builder.Entity<File>()
            .HasOne(f => f.Entry)
            .WithOne(e => e.File)
            .HasForeignKey<Entry>(e => e.FileId) 
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<Entry>()
            .HasOne(e => e.File)
            .WithOne(f => f.Entry)
            .HasForeignKey<Entry>(e => e.FileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Entry>()
            .HasOne(e => e.Group)
            .WithMany(g => g.Entries)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Entry>()
            .HasOne(e => e.LicencedUser)
            .WithMany()
            .HasForeignKey(e => e.LicencedUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<NotebookEntry>()
            .HasOne(ne => ne.NoteBook)
            .WithMany(n => n.Entries) 
            .HasForeignKey(ne => ne.NoteBookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<NoteBook>()
            .HasMany(nb => nb.Entries)
            .WithOne(ne => ne.NoteBook)
            .HasForeignKey(ne => ne.NoteBookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Group>()
            .HasOne(g => g.NoteBook)
            .WithOne()
            .HasForeignKey<Group>(g => g.NoteBookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Group>()
            .HasMany(g => g.Entries)
            .WithOne(e => e.Group)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Organization>()
            .HasMany(o => o.Groups)
            .WithOne(g => g.Organization)
            .HasForeignKey(g => g.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Organization>()
            .HasMany(o => o.Users)
            .WithOne(o => o.Organization)
            .HasForeignKey(o => o.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Organization>()
            .HasOne(o => o.Owner)
            .WithMany(o => o.Organizations)
            .HasForeignKey(o => o.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Licence>()
            .HasMany(l => l.LicenceLedgerEntries)
            .WithOne(l => l.Licence)
            .HasForeignKey(l => l.LicenceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Payment>()
            .HasOne(p => p.LicenceLegerEntry)
            .WithOne(l => l.Payment)
            .HasForeignKey<Payment>(l => l.LicenceLegerEntryId)
            .OnDelete(DeleteBehavior.Restrict); 

        builder.Entity<LicenceLedgerEntry>()
            .HasOne(l => l.Licence)
            .WithMany(l => l.LicenceLedgerEntries)
            .HasForeignKey(l => l.LicenceId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<LicenceLedgerEntry>()
            .HasOne(l => l.Payment)
            .WithOne(p => p.LicenceLegerEntry)
            .HasForeignKey<LicenceLedgerEntry>(p => p.PaymentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LicenceLedgerEntry>()
            .HasOne(l => l.LicencedUser)
            .WithMany(l => l.LicenceLedgerEntries)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<OrganizationOwner>()
            .HasMany(o => o.Organizations)
            .WithOne(o => o.Owner)
            .HasForeignKey(o => o.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LicencedUser>()
            .HasMany(l => l.LicenceLedgerEntries)
            .WithOne(l => l.LicencedUser)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LicencedUser>()
            .HasMany(l => l.OrganizationUsers)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LicencedUser>()
            .HasMany(l => l.Entries)
            .WithOne(l => l.LicencedUser)
            .HasForeignKey(l => l.LicencedUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Entity<OrganizationUser>()
            .HasOne(o => o.Organization)
            .WithMany(o => o.Users)
            .HasForeignKey(o => o.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OrganizationUser>()
            .HasOne(o => o.User)
            .WithMany(o => o.OrganizationUsers)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}