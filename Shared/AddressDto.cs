using System.ComponentModel.DataAnnotations;

namespace Agenda2.Shared.DTOs;

public class AddressDto
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "La via è obbligatoria")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "La città è obbligatoria")]
    public string City { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;
}