using System.ComponentModel.DataAnnotations;

namespace Agenda2.Database.Entities;

public class Address
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;

    [MaxLength(10)]
    public string ZipCode { get; set; } = string.Empty;

    // Soft Delete: se true, l'indirizzo non è più valido
    public bool IsDeleted { get; set; } = false;

    // Navigation Property verso la tabella ponte
    public ICollection<ContactAddress> ContactAddresses { get; set; } = new List<ContactAddress>();
}