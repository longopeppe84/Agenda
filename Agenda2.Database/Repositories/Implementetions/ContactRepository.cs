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
            .Include(c => c.Addresses)
            .Where(c=>c.IsDeleted==false)
            
            .ToListAsync();
    }

    public async Task<Contact?> GetByIdAsync(Guid id)
    {
        return await _context.Contacts
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contact> CreateAsync(Contact contact)
    {
        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();
        return contact;
    }

    public async Task UpdateAsync(Contact contact)
    {
        _context.Entry(contact).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var contact = await _context.Contacts.FindAsync(id);
        if (contact != null)
        {
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}