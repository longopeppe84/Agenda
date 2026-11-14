namespace Agenda2.Shared.DTOs;

public class OrderHistoryDto
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public List<OrderHistoryItemDto> Items { get; set; } = new List<OrderHistoryItemDto>();
}

public class OrderHistoryItemDto
{
    public string PizzaName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}