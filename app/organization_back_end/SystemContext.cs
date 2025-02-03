using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using organization_back_end.Auth.Model;
using organization_back_end.Entities;

namespace organization_back_end;

public class SystemContext : IdentityDbContext<User>
{
    public SystemContext()
    {
    }
    
    public virtual DbSet<Session> Session { get; set; }

    public SystemContext(DbContextOptions<SystemContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}