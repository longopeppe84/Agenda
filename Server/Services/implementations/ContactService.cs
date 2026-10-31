using Agenda2.Database.Entities;
using Agenda2.Database.Repositories.Interfaces;
using Agenda2.Shared.DTOs;

namespace Agenda2.Server.Services;

public class ContactService
{
    private readonly IContactRepository _repository;

    public ContactService(IContactRepository repository)
    {
        _repository = repository;
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
            Update = DateTime.Now,
            // Mappiamo i nuovi indirizzi
            Addresses = contactDto.Addresses.Select(a => new Address
            {
                Street = a.Street,
                City = a.City,
                ZipCode = a.ZipCode
            }).ToList()
        };

        var created = await _repository.CreateAsync(contact);
        return MapToDto(created);
    }

    public async Task UpdateContactAsync(Guid id, ContactDto contactDto)
    {
        var contact = await _repository.GetByIdAsync(id);
        if (contact != null)
        {
            contact.Nome = contactDto.Nome;
            contact.Cognome = contactDto.Cognome;
            contact.Email = contactDto.Email;
            contact.Telefono = contactDto.Telefono;
            contact.Dob = contactDto.Dob;
            contact.Update = DateTime.Now;

            // Svuotiamo e reinseriamo gli indirizzi (semplice ma efficace per ora)
            contact.Addresses.Clear();
            foreach (var addr in contactDto.Addresses)
            {
                contact.Addresses.Add(new Address
                {
                    Street = addr.Street,
                    City = addr.City,
                    ZipCode = addr.ZipCode
                });
            }

            await _repository.UpdateAsync(contact);
        }
    }

    public async Task DeleteContactAsync(Guid id)
    {
    //    await _repository.DeleteAsync(id);
       var contact= await _repository.GetByIdAsync(id);

        contact.IsDeleted= true;
        await _repository.SaveAsync();
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
            // Mappiamo gli indirizzi per il Client
            Addresses = c.Addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Street = a.Street,
                City = a.City,
                ZipCode = a.ZipCode
            }).ToList()
        };
    }
}