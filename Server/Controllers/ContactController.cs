using Agenda2.Server.Services;
using Agenda2.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Agenda2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ContactService _contactService;

    public ContactController(ContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContactDto>>> Get()
    {
        return Ok(await _contactService.GetAllContactsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactDto>> GetById(Guid id)
    {
        var contact = await _contactService.GetContactByIdAsync(id);
        if (contact == null) return NotFound();
        return Ok(contact);
    }

    [HttpPost]
    public async Task<ActionResult<ContactDto>> Create(ContactDto contactDto)
    {
        var created = await _contactService.CreateContactAsync(contactDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ContactDto contactDto)
    {
        // Forza l'ID nel DTO per evitare errori di tracking
        contactDto.Id = id;

        await _contactService.UpdateContactAsync(id, contactDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _contactService.DeleteContactAsync(id);
        return NoContent();
    }
}