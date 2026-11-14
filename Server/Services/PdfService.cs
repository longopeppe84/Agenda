using Agenda2.Database.Context;
using Agenda2.Database.Entities;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Agenda2.Server.Services;

public class PdfService
{
    private readonly AppDbContext _context;

    public PdfService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateOrderReceiptAsync(Guid orderId)
    {
        // 1. Recupera l'ordine con tutti i dettagli
        var order = await _context.Orders
            .Include(o => o.Contact)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Pizza)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
            throw new Exception("Ordine non trovato.");

        // 2. Configura la licenza di QuestPDF (Community)
        QuestPDF.Settings.License = LicenseType.Community;

        // 3. Genera il PDF (SENZA EMOJI PER EVITARE ERRORI)
        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Header
                page.Header().Column(col =>
                {
                    col.Item().Text("Agenda2 - Ricevuta Ordine").FontSize(18).Bold().FontColor(Colors.Red.Medium);
                    col.Item().Text($"Data: {order.OrderDate:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"Cliente: {order.Contact?.Nome} {order.Contact?.Cognome}").FontSize(12).Bold();
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                // Content
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text("Dettaglio Pizze:").FontSize(12).Bold().Underline();

                    col.Item().PaddingTop(5).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3); // Pizza
                            columns.RelativeColumn(1); // Qta
                            columns.RelativeColumn(2); // Prezzo
                        });

                        // Intestazione tabella
                        table.Header(header =>
                        {
                            header.Cell().Text("Pizza").Bold();
                            header.Cell().AlignCenter().Text("Q.ta").Bold();
                            header.Cell().AlignRight().Text("Prezzo").Bold();
                        });

                        // Righe
                        foreach (var item in order.OrderItems)
                        {
                            table.Cell().Text(item.Pizza?.Name ?? "Sconosciuta");
                            table.Cell().AlignCenter().Text(item.Quantity.ToString());
                            table.Cell().AlignRight().Text($"€ {item.Price:F2}");
                        }
                    });
                });

                // Footer
                page.Footer().Column(col =>
                {
                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingTop(5).AlignRight().Text($"Totale: € {order.TotalAmount:F2}").FontSize(14).Bold().FontColor(Colors.Green.Medium);
                    col.Item().PaddingTop(10).AlignCenter().Text("Grazie per il tuo ordine!").FontSize(10).Italic();
                });
            });
        }).GeneratePdf();

        return pdfBytes;
    }
}