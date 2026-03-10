# MauiOCRFacturas - OCR de Facturas con Azure Document Intelligence

## Practica: Desarrollo de Aplicaciones Inteligentes con Azure Cognitive Services y .NET MAUI

### Proyecto 2: OCR de Facturas con Azure Document Intelligence

---

## Descripcion General

Aplicacion movil desarrollada con **.NET MAUI** que permite al usuario fotografiar una factura o documento con la camara del telefono, procesarlo mediante **Azure Document Intelligence (Form Recognizer)**, y almacenar los datos extraidos localmente en la aplicacion.

---

## Arquitectura General

```
MauiOCRFacturas/
|-- App.xaml / App.xaml.cs          # Punto de entrada de la aplicacion
|-- AppShell.xaml / AppShell.xaml.cs # Navegacion con TabBar
|-- MauiProgram.cs                  # Configuracion DI y servicios
|-- MauiOCRFacturas.csproj          # Proyecto MAUI (Android, iOS, Windows)
|
|-- Models/
|   |-- ResultadoOCR.cs             # Modelo de datos del resultado OCR
|
|-- Services/
|   |-- DocumentIntelligenceService.cs  # Integracion con Azure
|
|-- ViewModels/
|   |-- MainViewModel.cs            # Logica de captura y analisis
|   |-- HistorialViewModel.cs       # Logica del historial
|
|-- Views/
    |-- MainPage.xaml / .cs         # Pantalla principal (camara + resultado)
    |-- HistorialPage.xaml / .cs    # Pantalla de historial
```

---

## Servicios de Azure Utilizados

| Servicio | Uso |
|----------|-----|
| **Azure Document Intelligence** | OCR inteligente de facturas usando el modelo `prebuilt-invoice` |

### Campos extraidos automaticamente:
- Nombre del proveedor (VendorName)
- Nombre del cliente (CustomerName)
- Numero de factura (InvoiceId)
- Fecha de factura (InvoiceDate)
- Total (InvoiceTotal)
- Subtotal (SubTotal)
- Impuestos (TotalTax)
- Items/Lineas de la factura (Items > Description, Amount)

---

## Configuracion

### Prerrequisitos
- Visual Studio 2022+ con workload .NET MAUI
- Cuenta de Azure con recurso de Azure Document Intelligence creado
- .NET 9 SDK

### Configurar credenciales de Azure

Abre `Services/DocumentIntelligenceService.cs` y reemplaza:

```csharp
private const string Endpoint = "https://TU-RECURSO.cognitiveservices.azure.com/";
private const string ApiKey = "TU_CLAVE_DE_API_AQUI";
```

Con tu endpoint y clave del recurso de Azure Document Intelligence.

### Paquetes NuGet
- `Azure.AI.FormRecognizer` 4.1.0
- `CommunityToolkit.Mvvm` 8.2.2
- `Microsoft.Maui.Controls` (incluido con MAUI)

---

## Funcionalidades

1. **Captura de imagen**: Toma foto con la camara o selecciona desde galeria
2. **Analisis OCR**: Envia la imagen a Azure Document Intelligence
3. **Visualizacion**: Muestra los datos extraidos de la factura
4. **Historial**: Almacena todas las facturas analizadas en la sesion
5. **Limpieza**: Permite borrar el historial

---

## Permisos requeridos

- **Android**: `CAMERA`, `READ_EXTERNAL_STORAGE`
- **iOS**: `NSCameraUsageDescription`, `NSPhotoLibraryUsageDescription`

---

## Patron de Arquitectura

La aplicacion sigue el patron **MVVM** (Model-View-ViewModel):
- **Model**: `ResultadoOCR` - datos del analisis
- **View**: Paginas XAML con data binding
- **ViewModel**: Logica de negocio con CommunityToolkit.Mvvm
- **Service**: `DocumentIntelligenceService` - capa de abstraccion de Azure

---

## Evaluacion

| Criterio | Implementado |
|----------|-------------|
| Funcionalidad (30%) | Captura, analisis y visualizacion completos |
| Uso correcto de Azure (30%) | Azure Document Intelligence con modelo prebuilt-invoice |
| Calidad del codigo (10%) | MVVM, DI, Source Generators de CommunityToolkit |
| Documentacion (30%) | README detallado + estructura clara |
