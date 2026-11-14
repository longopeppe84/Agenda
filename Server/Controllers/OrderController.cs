using Agenda2.Database.Context;
using Agenda2.Database.Entities;
using Agenda2.Server.Services;
using Agenda2.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agenda2.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PdfService _pdfService;

    public OrderController(AppDbContext context, PdfService pdfService)
    {
        _context = context;
        _pdfService = pdfService;
    }

    // 1. CREA UN NUOVO ORDINE
    [HttpPost]
    public async Task<IActionResult> CreateOrder(OrderDto orderDto)
    {
        if (orderDto.ContactId == Guid.Empty)
        {
            return BadRequest("Devi selezionare un contatto.");
        }

        var order = new Order
        {
            ContactId = orderDto.ContactId,
            OrderDate = DateTime.Now,
            TotalAmount = orderDto.Items.Sum(i => i.Price * i.Quantity)
        };

        foreach (var item in orderDto.Items)
        {
            order.OrderItems.Add(new OrderItem
            {
                PizzaId = item.PizzaId,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Ordine salvato con successo!", OrderId = order.Id });
    }

    // 2. RECUPERA LO STORICO ORDINI
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<OrderHistoryDto>>> GetOrderHistory()
    {
        var orders = await _context.Orders
            .Include(o => o.Contact)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Pizza)
            .Where(o => !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderHistoryDto
            {
                OrderId = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                ContactName = o.Contact != null ? $"{o.Contact.Nome} {o.Contact.Cognome}" : "Sconosciuto",
                Items = o.OrderItems.Select(oi => new OrderHistoryItemDto
                {
                    PizzaName = oi.Pizza != null ? oi.Pizza.Name : "Pizza sconosciuta",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            })
            .ToListAsync();

        return Ok(orders);
    }

    // 3. SCARICA LA RICEVUTA PDF
    [HttpGet("{id}/receipt")]
    public async Task<IActionResult> DownloadReceipt(Guid id)
    {
        try
        {
            var pdfBytes = await _pdfService.GenerateOrderReceiptAsync(id);
            return File(pdfBytes, "application/pdf", $"Ricevuta_Ordine_{id.ToString().Substring(0, 8)}.pdf");
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}