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
    public DbSet<Relationship> Relationships { get; set; }

    // Tabella Pizze
    public DbSet<Pizza> Pizzas { get; set; }

    // NUOVE TABELLE per gli Ordini
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

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
        // CONFIGURAZIONE RELAZIONI PARENTALI
        // =========================================================
        modelBuilder.Entity<Relationship>()
            .HasOne(r => r.Contact)
            .WithMany(c => c.Relationships)
            .HasForeignKey(r => r.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Relationship>()
            .HasOne(r => r.Relative)
            .WithMany(c => c.ReverseRelationships)
            .HasForeignKey(r => r.RelativeId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // CONFIGURAZIONE ORDINI E DETTAGLI
        // =========================================================
        // Un Ordine appartiene a un Contatto
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Contact)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.ContactId)
            .OnDelete(DeleteBehavior.Restrict);

        // Un Dettaglio Ordine appartiene a un Ordine
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Un Dettaglio Ordine si riferisce a una Pizza
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Pizza)
            .WithMany()
            .HasForeignKey(oi => oi.PizzaId)
            .OnDelete(DeleteBehavior.Restrict);

        // =========================================================
        // CONFIGURAZIONE E SEEDING PIZZE
        // =========================================================
        modelBuilder.Entity<Pizza>().HasData(
            new Pizza { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Margherita", Ingredients = "Pomodoro, Mozzarella, Basilico", Price = 5.50m, ImageUrl = "🍕" },
            new Pizza { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Diavola", Ingredients = "Pomodoro, Mozzarella, Salame Piccante", Price = 7.00m, ImageUrl = "🌶️" },
            new Pizza { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Capricciosa", Ingredients = "Pomodoro, Mozzarella, Prosciutto, Funghi, Carciofi", Price = 8.50m, ImageUrl = "🍄" },
            new Pizza { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Quattro Formaggi", Ingredients = "Mozzarella, Gorgonzola, Fontina, Parmigiano", Price = 8.00m, ImageUrl = "🧀" }
        );
    }
}