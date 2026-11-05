using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agenda2.Database.Entities;

public class Contact
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [MinLength(3), MaxLength(30)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MinLength(3), MaxLength(30)]
    public string Cognome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Telefono { get; set; } = string.Empty;

    public DateTime Dob { get; set; }

    public bool IsDeleted { get; set; } = false;

    public string FotoProfilo { get; set; } = string.Empty;

    public ICollection<Relationship> Relationships { get; set; } = new List<Relationship>();

    // Relazioni in entrata (es. Mario è il padre di Luca)
    public ICollection<Relationship> ReverseRelationships { get; set; } = new List<Relationship>();

    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime Update { get; set; } = DateTime.Now;

    // Navigation Property verso la tabella ponte
    public ICollection<ContactAddress> ContactAddresses { get; set; } = new List<ContactAddress>();
}