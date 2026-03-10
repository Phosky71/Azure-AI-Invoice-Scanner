using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiOCRFacturas.Models;
using System.Collections.ObjectModel;

namespace MauiOCRFacturas.ViewModels;

[ObservableObject]
public partial class HistorialViewModel
{
    [ObservableProperty]
    private ObservableCollection<ResultadoOCR> _facturas = new();

    [ObservableProperty]
    private ResultadoOCR? _facturaSeleccionada;

    [ObservableProperty]
    private bool _hayFacturas = false;

    public HistorialViewModel()
    {
        CargarHistorial();
    }

    private void CargarHistorial()
    {
        Facturas.Clear();
        foreach (var item in MainViewModel.Historial)
            Facturas.Add(item);

        HayFacturas = Facturas.Count > 0;
    }

    [RelayCommand]
    private void ActualizarHistorial()
    {
        CargarHistorial();
    }

    [RelayCommand]
    private async Task SeleccionarFacturaAsync(ResultadoOCR factura)
    {
        if (factura is null) return;
        FacturaSeleccionada = factura;

        await Shell.Current.DisplayAlert(
            $"Factura #{factura.Id}",
            factura.TextoCompleto,
            "Cerrar");
    }

    [RelayCommand]
    private async Task LimpiarHistorialAsync()
    {
        bool confirmar = await Shell.Current.DisplayAlert(
            "Confirmar",
            "Se eliminaran todos los registros del historial. Continuar?",
            "Si, borrar", "Cancelar");

        if (!confirmar) return;

        MainViewModel.Historial.Clear();
        CargarHistorial();
    }

    [RelayCommand]
    private async Task VolverAsync()
    {
        await Shell.Current.GoToAsync("//main");
    }
}
