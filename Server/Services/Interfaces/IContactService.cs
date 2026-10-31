using Agenda2.Shared;
using Agenda2.Shared.DTOs;

namespace Agenda2.Server.Services.Interfaces;

public interface IContactService
{
    Task<List<ContactDto>> GetAllAsync();
}