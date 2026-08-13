using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace RepairDesk;

public static class PdfService
{
    public static string Generate(RepairRecord repair, ShopSettings shop, string? destination = null)
    {
        var folder = StorageConfig.GetPdfFolder();
        Directory.CreateDirectory(folder);
        destination ??= Path.Combine(folder, $"{repair.PracticeNumber}_{SafeName(repair.DisplayName)}.pdf");

        Document.Create(document => document.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(35);
            page.DefaultTextStyle(x => x.FontSize(10).FontColor("#172033"));
            page.Header().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(shop.ShopName).FontSize(22).Bold().FontColor("#5B5FEF");
                    c.Item().Text($"{shop.Address}  {shop.Phone}  {shop.Email}").FontSize(9);
                    if (!string.IsNullOrWhiteSpace(shop.VatNumber)) c.Item().Text($"P. IVA: {shop.VatNumber}").FontSize(9);
                });
                row.ConstantItem(170).AlignRight().Column(c =>
                {
                    c.Item().Text("SCHEDA RIPARAZIONE").Bold().FontSize(14);
                    c.Item().Text($"Pratica: {repair.PracticeNumber}");
                    c.Item().Text($"Data: {repair.CreatedAt:dd/MM/yyyy HH:mm}");
                });
            });
            page.Content().PaddingVertical(18).Column(column =>
            {
                Section(column, "CLIENTE", $"{repair.DisplayName}\nTelefono: {repair.Phone}\nEmail: {Value(repair.Email)}");
                Section(column, "DISPOSITIVO", $"{repair.Device}\nColore: {Value(repair.Color)}    IMEI/Seriale: {Value(repair.Imei)}");
                if (repair.AppointmentAt is not null) Section(column, "APPUNTAMENTO", repair.AppointmentAt.Value.ToString("dddd d MMMM yyyy 'alle' HH:mm", new System.Globalization.CultureInfo("it-IT")));
                Section(column, "RIPARAZIONE RICHIESTA", repair.RepairDescription);
                Section(column, "TIPOLOGIA INTERVENTO", Join(repair.RepairTypes));
                Section(column, "ACCESSORI CONSEGNATI", Join(repair.Accessories));
                Section(column, "STATO ALLA CONSEGNA", Join(repair.DeviceConditions));
                Section(column, "NOTE SULLE CONDIZIONI", Value(repair.ConditionNotes));
                column.Item().PaddingTop(24).Row(row =>
                {
                    row.RelativeItem().Column(c => { c.Item().Text("Firma cliente").Bold(); c.Item().PaddingTop(32).LineHorizontal(1).LineColor("#8892A6"); });
                    row.ConstantItem(40);
                    row.RelativeItem().Column(c => { c.Item().Text("Firma operatore").Bold(); c.Item().PaddingTop(32).LineHorizontal(1).LineColor("#8892A6"); });
                });
            });
            page.Footer().AlignCenter().Text("Il cliente dichiara che i dati e lo stato del dispositivo indicati sono corretti.").FontSize(8).FontColor(Colors.Grey.Medium);
        })).GeneratePdf(destination);
        return destination;
    }

    private static void Section(ColumnDescriptor column, string title, string value)
    {
        column.Item().PaddingBottom(10).Border(1).BorderColor("#E1E6F0").Padding(10).Column(c =>
        {
            c.Item().Text(title).Bold().FontColor("#5B5FEF");
            c.Item().PaddingTop(4).Text(value);
        });
    }
    private static string Join(List<string> values) => values.Count == 0 ? "Nessuno / non indicato" : string.Join(" • ", values);
    private static string Value(string value) => string.IsNullOrWhiteSpace(value) ? "Non indicato" : value;
    private static string SafeName(string value) => string.Concat(value.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
}
