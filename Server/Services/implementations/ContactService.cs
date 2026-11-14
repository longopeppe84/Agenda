using Agenda2.Database.Context;
using Agenda2.Database.Entities;
using Agenda2.Database.Repositories.Interfaces;
using Agenda2.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Agenda2.Server.Services;

public class ContactService
{
    private readonly IContactRepository _repository;
    private readonly AppDbContext _context;

    public ContactService(IContactRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<IEnumerable<ContactDto>> GetAllContactsAsync()
    {
        var contacts = await _repository.GetAllAsync();
        return contacts.Select(MapToDto);
    }

    public async Task<ContactDto?> GetContactByIdAsync(Guid id)
    {
        var contact = await _repository.GetByIdAsync(id);
        return contact == null ? null : MapToDto(contact);
    }

    public async Task<ContactDto> CreateContactAsync(ContactDto contactDto)
    {
        var contact = new Contact
        {
            Nome = contactDto.Nome,
            Cognome = contactDto.Cognome,
            Email = contactDto.Email,
            Telefono = contactDto.Telefono,
            Dob = contactDto.Dob,
            FotoProfilo = "default_user.png",
            Created = DateTime.Now,
            Update = DateTime.Now
        };

        // 1. Indirizzi
        foreach (var addrDto in contactDto.Addresses)
        {
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Street == addrDto.Street && a.City == addrDto.City && a.ZipCode == addrDto.ZipCode && !a.IsDeleted);

            if (existingAddress == null)
            {
                existingAddress = new Address { Street = addrDto.Street, City = addrDto.City, ZipCode = addrDto.ZipCode, IsDeleted = false };
                _context.Addresses.Add(existingAddress);
                await _context.SaveChangesAsync();
            }
            contact.ContactAddresses.Add(new ContactAddress { AddressId = existingAddress.Id, IsDeleted = false });
        }

        // 2. Relazioni
        foreach (var relDto in contactDto.Relationships)
        {
            if (relDto.RelativeId == Guid.Empty) continue;
            contact.Relationships.Add(new Relationship
            {
                RelationshipType = relDto.RelationshipType,
                RelativeId = relDto.RelativeId,
                IsDeleted = false
            });
        }

        var created = await _repository.CreateAsync(contact);
        return MapToDto(created);
    }

    public async Task UpdateContactAsync(Guid id, ContactDto contactDto)
    {
        // 1. Carichiamo il contatto con TRACKING ATTIVO
        var contact = await _context.Contacts
            .Include(c => c.ContactAddresses)
            .Include(c => c.Relationships)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null) return;

        // 2. Aggiorniamo i campi base
        contact.Nome = contactDto.Nome;
        contact.Cognome = contactDto.Cognome;
        contact.Email = contactDto.Email;
        contact.Telefono = contactDto.Telefono;
        contact.Dob = contactDto.Dob;
        contact.Update = DateTime.Now;

        // 3. Salviamo subito i campi base
        await _context.SaveChangesAsync();

        // ==========================================
        // 4. GESTIONE INDIRIZZI (Confronto)
        // ==========================================
        // Rimuoviamo dalla lista degli indirizzi esistenti quelli che NON sono più nel DTO
        foreach (var ca in contact.ContactAddresses.ToList())
        {
            // Se l'indirizzo non è presente nel DTO (per ID), lo marchiamo come cancellato
            if (!contactDto.Addresses.Any(a => a.Id == ca.AddressId))
            {
                ca.IsDeleted = true;
            }
        }
        await _context.SaveChangesAsync();

        // Aggiungiamo i nuovi indirizzi (quelli con ID vuoto o non ancora presenti)
        foreach (var addrDto in contactDto.Addresses)
        {
            // Se ha già un ID, significa che è un indirizzo esistente, lo saltiamo
            if (addrDto.Id != Guid.Empty) continue;

            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Street == addrDto.Street && a.City == addrDto.City && a.ZipCode == addrDto.ZipCode && !a.IsDeleted);

            if (existingAddress == null)
            {
                existingAddress = new Address { Street = addrDto.Street, City = addrDto.City, ZipCode = addrDto.ZipCode, IsDeleted = false };
                _context.Addresses.Add(existingAddress);
                await _context.SaveChangesAsync();
            }

            // Controlliamo se il legame esiste già (anche se cancellato, lo riattiviamo)
            var existingLink = contact.ContactAddresses
                .FirstOrDefault(ca => ca.AddressId == existingAddress.Id);

            if (existingLink != null)
            {
                existingLink.IsDeleted = false; // Riattiviamo
            }
            else
            {
                contact.ContactAddresses.Add(new ContactAddress { AddressId = existingAddress.Id, IsDeleted = false });
            }
        }

        // ==========================================
        // 5. GESTIONE RELAZIONI (Confronto)
        // ==========================================
        // Rimuoviamo le relazioni che non sono più nel DTO
        foreach (var rel in contact.Relationships.ToList())
        {
            if (!contactDto.Relationships.Any(r => r.RelativeId == rel.RelativeId && r.RelationshipType == rel.RelationshipType))
            {
                rel.IsDeleted = true;
            }
        }
        await _context.SaveChangesAsync();

        // Aggiungiamo le nuove relazioni
        foreach (var relDto in contactDto.Relationships)
        {
            if (relDto.RelativeId == Guid.Empty) continue;

            var existingRel = contact.Relationships
                .FirstOrDefault(r => r.RelativeId == relDto.RelativeId && r.RelationshipType == relDto.RelationshipType);

            if (existingRel != null)
            {
                existingRel.IsDeleted = false; // Riattiviamo
            }
            else
            {
                contact.Relationships.Add(new Relationship
                {
                    RelationshipType = relDto.RelationshipType,
                    RelativeId = relDto.RelativeId,
                    IsDeleted = false
                });
            }
        }

        // 6. Salvataggio finale
        await _context.SaveChangesAsync();
    }

    public async Task DeleteContactAsync(Guid id)
    {
        var contact = await _context.Contacts
            .Include(c => c.ContactAddresses)
            .Include(c => c.Relationships)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact != null)
        {
            contact.IsDeleted = true;
            foreach (var ca in contact.ContactAddresses) ca.IsDeleted = true;
            foreach (var rel in contact.Relationships) rel.IsDeleted = true;

            await _context.SaveChangesAsync();
        }
    }

    private ContactDto MapToDto(Contact c)
    {
        return new ContactDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Cognome = c.Cognome,
            Email = c.Email,
            Telefono = c.Telefono,
            Dob = c.Dob,
            Addresses = c.ContactAddresses.Where(ca => !ca.IsDeleted).Select(ca => new AddressDto
            {
                Id = ca.Address!.Id,
                Street = ca.Address.Street,
                City = ca.Address.City,
                ZipCode = ca.Address.ZipCode
            }).ToList(),
            Relationships = c.Relationships
                .Where(r => !r.IsDeleted && (r.Relative == null || !r.Relative.IsDeleted))
                .Select(r => new RelationshipDto
                {
                    Id = r.Id,
                    RelationshipType = r.RelationshipType,
                    RelativeId = r.RelativeId,
                    RelativeIdString = r.RelativeId.ToString(),
                    RelativeName = r.Relative != null ? $"{r.Relative.Nome} {r.Relative.Cognome}" : "Sconosciuto"
                }).ToList()
        };
    }
}