using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiOCRFacturas.Models;
using MauiOCRFacturas.Services;

namespace MauiOCRFacturas.ViewModels;

[ObservableObject]
public partial class MainViewModel
{
    private readonly DocumentIntelligenceService _ocrService;

    [ObservableProperty]
    private ImageSource? _imagenCapturada;

    [ObservableProperty]
    private string _resultadoTexto = "Capture o seleccione una imagen de factura para analizarla.";

    [ObservableProperty]
    private bool _isLoading = false;

    [ObservableProperty]
    private bool _hayResultado = false;

    [ObservableProperty]
    private ResultadoOCR? _ultimoResultado;

    // Lista estatica en memoria (sin base de datos para mantener simplicidad)
    public static List<ResultadoOCR> Historial { get; } = new();

    public MainViewModel(DocumentIntelligenceService ocrService)
    {
        _ocrService = ocrService;
    }

    [RelayCommand]
    private async Task TomarFotoAsync()
    {
        try
        {
            // Verificar permiso de camara
            var status = await Permissions.RequestAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Permiso denegado",
                    "Se necesita acceso a la camara para tomar fotos de facturas.", "OK");
                return;
            }

            var foto = await MediaPicker.Default.CapturePhotoAsync();
            if (foto is null) return;

            await ProcesarFotoAsync(foto);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al acceder a la camara: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SeleccionarImagenAsync()
    {
        try
        {
            var imagen = await MediaPicker.Default.PickPhotoAsync();
            if (imagen is null) return;

            await ProcesarFotoAsync(imagen);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al seleccionar imagen: {ex.Message}", "OK");
        }
    }

    private async Task ProcesarFotoAsync(FileResult foto)
    {
        try
        {
            IsLoading = true;
            HayResultado = false;
            ResultadoTexto = "Analizando documento con Azure Document Intelligence...";

            // Mostrar preview de la imagen
            ImagenCapturada = ImageSource.FromFile(foto.FullPath);

            // Analizar con Azure
            using var stream = await foto.OpenReadAsync();
            var resultado = await _ocrService.AnalizarDocumentoAsync(stream);
            resultado.RutaImagen = foto.FullPath;

            // Guardar en historial
            resultado.Id = Historial.Count + 1;
            Historial.Insert(0, resultado);

            UltimoResultado = resultado;
            ResultadoTexto = resultado.TextoCompleto;
            HayResultado = true;
        }
        catch (Exception ex)
        {
            ResultadoTexto = $"Error durante el analisis:\n{ex.Message}";
            await Shell.Current.DisplayAlert("Error de OCR",
                $"No se pudo analizar el documento:\n{ex.Message}", "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task VerHistorialAsync()
    {
        await Shell.Current.GoToAsync("//historial");
    }
}
