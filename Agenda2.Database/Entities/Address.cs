using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agenda2.Database.Entities;
// creata nuova entità 
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

    // DEPENDENCY & NAVIGATION PROPERTY
    public Guid ContactId { get; set; }

    [ForeignKey(nameof(ContactId))]
    public Contact? Contact { get; set; }
}