using System.ComponentModel.DataAnnotations;

namespace Agenda2.Shared.DTOs;

public class RelationshipDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Il tipo di parentela è obbligatorio")]
    public string RelationshipType { get; set; } = string.Empty;

    public Guid RelativeId { get; set; }

    public string RelativeName { get; set; } = string.Empty;

    // Campo di supporto per il binding di Blazor
    public string RelativeIdString { get; set; } = string.Empty;
}