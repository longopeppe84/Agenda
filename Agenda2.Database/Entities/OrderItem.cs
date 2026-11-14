using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agenda2.Database.Entities;

public class OrderItem
{
    [Key]
    public Guid Id { get; set; }

    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; }

    public Guid OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }

    public Guid PizzaId { get; set; }
    [ForeignKey(nameof(PizzaId))]
    public Pizza? Pizza { get; set; }
}