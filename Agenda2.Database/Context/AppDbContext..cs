using Agenda2.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agenda2.Database.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Address> Addresses { get; set; } // NUOVO

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurazione della dipendenza (Cascade Delete)
        modelBuilder.Entity<Contact>()
            .HasMany(c => c.Addresses)
            .WithOne(a => a.Contact)
            .HasForeignKey(a => a.ContactId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}