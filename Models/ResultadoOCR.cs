namespace MauiOCRFacturas.Models;

/// <summary>
/// Representa el resultado del analisis OCR de una factura.
/// </summary>
public class ResultadoOCR
{
    public int Id { get; set; }

    /// <summary>Fecha y hora del analisis</summary>
    public DateTime FechaAnalisis { get; set; } = DateTime.Now;

    /// <summary>Nombre del proveedor extraido</summary>
    public string? Proveedor { get; set; }

    /// <summary>Nombre del cliente extraido</summary>
    public string? Cliente { get; set; }

    /// <summary>Numero de factura extraido</summary>
    public string? NumeroFactura { get; set; }

    /// <summary>Total de la factura extraido</summary>
    public string? Total { get; set; }

    /// <summary>Texto completo extraido del documento</summary>
    public string TextoCompleto { get; set; } = string.Empty;

    /// <summary>Ruta local de la imagen capturada</summary>
    public string? RutaImagen { get; set; }

    /// <summary>Resumen legible para mostrar en lista</summary>
    public string Resumen => !string.IsNullOrEmpty(Proveedor)
        ? $"{Proveedor} - {Total ?? "Sin total"}"
        : $"Factura {FechaAnalisis:dd/MM/yyyy HH:mm}";
}
