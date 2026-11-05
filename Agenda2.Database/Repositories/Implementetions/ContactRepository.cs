using Agenda2.Database.Context;
using Agenda2.Database.Entities;
using Agenda2.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Agenda2.Database.Repositories.Implementations;

public class ContactRepository : IContactRepository
{
    private readonly AppDbContext _context;

    public ContactRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Contact>> GetAllAsync()
    {
        return await _context.Contacts
            .Where(c => !c.IsDeleted)
            .Include(c => c.ContactAddresses.Where(ca => !ca.IsDeleted)).ThenInclude(ca => ca.Address)
            .Include(c => c.Relationships.Where(r => !r.IsDeleted)).ThenInclude(r => r.Relative)
            .ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(Guid id)
    {
        return await _context.Contacts
            .Include(c => c.ContactAddresses.Where(ca => !ca.IsDeleted)).ThenInclude(ca => ca.Address)
            .Include(c => c.Relationships.Where(r => !r.IsDeleted)).ThenInclude(r => r.Relative)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task UpdateAsync(Contact contact)
    {
        // Il salvataggio è gestito dal Service
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact != null)
        {
            contact.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }
}