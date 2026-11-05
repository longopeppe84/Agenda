using Agenda2.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Agenda2.Database.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Tabelle esistenti
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<ContactAddress> ContactAddresses { get; set; }

    // NUOVA TABELLA per le relazioni parentali
    public DbSet<Relationship> Relationships { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================================================
        // CONFIGURAZIONE MOLTI-A-MOLTI PER GLI INDIRIZZI
        // =========================================================
        modelBuilder.Entity<ContactAddress>()
            .HasKey(ca => new { ca.ContactId, ca.AddressId });

        modelBuilder.Entity<ContactAddress>()
            .HasOne(ca => ca.Contact)
            .WithMany(c => c.ContactAddresses)
            .HasForeignKey(ca => ca.ContactId);

        modelBuilder.Entity<ContactAddress>()
            .HasOne(ca => ca.Address)
            .WithMany(a => a.ContactAddresses)
            .HasForeignKey(ca => ca.AddressId);


        // =========================================================
        // CONFIGURAZIONE RELAZIONI PARENTALI (Self-Referencing)
        // =========================================================

        // 1. Configurazione della relazione "in uscita" (Es. Luca -> Padre -> Mario)
        modelBuilder.Entity<Relationship>()
            .HasOne(r => r.Contact)
            .WithMany(c => c.Relationships)
            .HasForeignKey(r => r.ContactId)
            .OnDelete(DeleteBehavior.Restrict); // Evita cancellazioni a cascata fisiche

        // 2. Configurazione della relazione "in entrata" (Es. Mario -> È padre di -> Luca)
        modelBuilder.Entity<Relationship>()
            .HasOne(r => r.Relative)
            .WithMany(c => c.ReverseRelationships)
            .HasForeignKey(r => r.RelativeId)
            .OnDelete(DeleteBehavior.Restrict); // Evita cancellazioni a cascata fisiche
    }
}