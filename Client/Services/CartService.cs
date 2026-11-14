using Agenda2.Shared.DTOs;

namespace Agenda2.Client.Services;

public class CartService
{
    private readonly List<PizzaDto> _items = new();

    public IReadOnlyList<PizzaDto> Items => _items;

    // NUOVO: Ricorda l'ID del contatto selezionato per l'ordine
    public Guid SelectedContactId { get; set; } = Guid.Empty;

    // Evento per aggiornare la UI
    public event Action? OnChange;

    public void AddPizza(PizzaDto pizza)
    {
        _items.Add(pizza);
        NotifyStateChanged();
    }

    public void RemovePizza(PizzaDto pizza)
    {
        _items.Remove(pizza);
        NotifyStateChanged();
    }

    public decimal GetTotal() => _items.Sum(p => p.Price);

    // NUOVO: Svuota il carrello e resetta il contatto
    public void Clear()
    {
        _items.Clear();
        SelectedContactId = Guid.Empty;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}