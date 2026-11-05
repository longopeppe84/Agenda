using System.ComponentModel.DataAnnotations;

namespace Agenda2.Shared.DTOs;

public class ContactDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Il nome è obbligatorio")]
    [MinLength(3, ErrorMessage = "Minimo 3 caratteri")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il cognome è obbligatorio")]
    public string Cognome { get; set; } = string.Empty;

    [Required(ErrorMessage = "L'email è obbligatoria")]
    [EmailAddress(ErrorMessage = "Formato email non valido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Il telefono è obbligatorio")]
    public string Telefono { get; set; } = string.Empty;

    public DateTime Dob { get; set; }

    // Lista degli indirizzi (in inglese perché nuova)
    public List<AddressDto> Addresses { get; set; } = new List<AddressDto>();

    
    public List<RelationshipDto> Relationships { get; set; } = new List<RelationshipDto>();
}