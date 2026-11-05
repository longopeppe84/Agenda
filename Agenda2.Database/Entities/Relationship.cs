using System.ComponentModel.DataAnnotations;

namespace Agenda2.Database.Entities;

public class Relationship
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string RelationshipType { get; set; } = string.Empty; // es. "Padre", "Madre", "Fratello"

    // Il contatto di partenza (es. Luca)
    public Guid ContactId { get; set; }
    public Contact? Contact { get; set; }

    // Il contatto di arrivo (es. Mario)
    public Guid RelativeId { get; set; }
    public Contact? Relative { get; set; }

    // SOFT DELETE
    public bool IsDeleted { get; set; } = false;
}