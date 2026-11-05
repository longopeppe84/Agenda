using Agenda2.Database.Entities;

namespace Agenda2.Database.Repositories.Interfaces;

public interface IContactRepository
{
    Task<IEnumerable<Contact>> GetAllAsync();
    Task<Contact?> GetByIdAsync(Guid id);
    Task<Contact> CreateAsync(Contact contact);
    Task UpdateAsync(Contact contact);
    Task DeleteAsync(Guid id);
}