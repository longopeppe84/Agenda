namespace Agenda2.Database.Entities;

public class ContactAddress
{
    public Guid ContactId { get; set; }
    public Contact? Contact { get; set; }

    public Guid AddressId { get; set; }
    public Address? Address { get; set; }

    // Soft Delete sul legame
    public bool IsDeleted { get; set; } = false;
}