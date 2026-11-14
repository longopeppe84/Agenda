using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda2.Shared.DTOs;

public class OrderDto
{
    public Guid ContactId { get; set; }
    public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

public class OrderItemDto
{
    public Guid PizzaId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}