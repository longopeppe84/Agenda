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
        var contact = await _context.Contacts
            .Include(c => c.ContactAddresses)
                .ThenInclude(ca => ca.Address)
            .Include(c => c.Relationships)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contact == null) return;

        // 1. Aggiorniamo i campi base
        contact.Nome = contactDto.Nome;
        contact.Cognome = contactDto.Cognome;
        contact.Email = contactDto.Email;
        contact.Telefono = contactDto.Telefono;
        contact.Dob = contactDto.Dob;
        contact.Update = DateTime.Now;

        // ==========================================
        // 2. GESTIONE INDIRIZZI (Solo se presenti nel DTO)
        // ==========================================

        // A) Rimuoviamo i legami che non sono più nel DTO (Soft Delete)
        foreach (var existingCa in contact.ContactAddresses.ToList())
        {
            // Se l'indirizzo non è presente nel DTO, lo marchiamo come cancellato
            if (!contactDto.Addresses.Any(a => a.Id == existingCa.AddressId))
            {
                existingCa.IsDeleted = true;
            }
        }

        // B) Aggiungiamo i nuovi indirizzi
        foreach (var addrDto in contactDto.Addresses)
        {
            // Se ha un ID, è un indirizzo esistente che vogliamo mantenere. Saltiamolo.
            if (addrDto.Id != Guid.Empty) continue;

            // Se non ha ID, è un nuovo indirizzo
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Street == addrDto.Street && a.City == addrDto.City && a.ZipCode == addrDto.ZipCode && !a.IsDeleted);

            if (existingAddress == null)
            {
                existingAddress = new Address { Street = addrDto.Street, City = addrDto.City, ZipCode = addrDto.ZipCode, IsDeleted = false };
                _context.Addresses.Add(existingAddress);
                await _context.SaveChangesAsync();
            }

            // Controlliamo se il legame esiste già (magari riattiviamo uno cancellato)
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
        // 3. GESTIONE RELAZIONI (Solo se presenti nel DTO)
        // ==========================================

        // A) Rimuoviamo le relazioni che non sono più nel DTO
        foreach (var existingRel in contact.Relationships.ToList())
        {
            // Se la relazione non è presente nel DTO (per RelativeId), la marchiamo come cancellata
            if (!contactDto.Relationships.Any(r => r.RelativeId == existingRel.RelativeId && r.RelationshipType == existingRel.RelationshipType))
            {
                existingRel.IsDeleted = true;
            }
        }

        // B) Aggiungiamo le nuove relazioni
        foreach (var relDto in contactDto.Relationships)
        {
            if (relDto.RelativeId == Guid.Empty) continue;

            // Controlliamo se esiste già una relazione attiva identica
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

        // 4. Salvataggio finale
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
            Relationships = c.Relationships.Where(r => !r.IsDeleted).Select(r => new RelationshipDto
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