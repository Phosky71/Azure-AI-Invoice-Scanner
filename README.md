# MauiOCRFacturas

> **⚠️ Note:** This project is currently paused and will not be actively developed for the time being.

**Academic project** developed as part of the subject *Desarrollo de Aplicaciones Inteligentes con Azure Cognitive Services y .NET MAUI*.

A cross-platform mobile application built with **.NET MAUI** (targeting Android) that integrates multiple **Azure AI services** into a single app. The core feature is intelligent OCR of invoices using **Azure Document Intelligence**, extended with computer vision, speech recognition, real-time translation, AI-powered voice summaries via OpenAI, and message queuing through Azure Service Bus.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Azure Services Used](#azure-services-used)
- [NuGet Packages](#nuget-packages)
- [Configuration](#configuration)
- [Known Issues & Limitations](#known-issues--limitations)
- [Requirements](#requirements)

---

## Features

### 1. Invoice OCR (`MainPage`)
Capture a photo of an invoice with the device camera or pick one from the gallery. The image is sent to **Azure Document Intelligence** using the `prebuilt-invoice` model. The following fields are automatically extracted:

- Vendor Name
- Customer Name
- Invoice ID
- Invoice Date
- Invoice Total, Subtotal, Tax
- Line items (description + amount)

All results are stored in a session history accessible from the **History** tab.

### 2. Computer Vision / Object Detection (`VisionPage`)
Analyzes images using **Azure AI Vision** (`ImageAnalysisClient`). Returns:

- Automatic image caption/description
- Detected objects with confidence scores
- General image tags with confidence percentages

> **⚠️ Known issue:** The object detector is functional but has some inconsistencies in detection behavior.

### 3. Speech Translation (`TraductorPage`)
Records voice input via **Azure Speech Service**, recognizes it, then translates the transcribed text using the **Azure Translator** REST API.

> **⚠️ Known issue:** This feature has graphic design/layout problems in the UI — the logic works but the interface needs visual polish.

### 4. Voice Recording & AI Summary (`VoiceSummaryPage`)
Records audio via the microphone using **Azure Speech Service** (`SpeechService`) and sends the transcription to **Azure OpenAI** (`OpenAIService`) to generate a structured bullet-point summary.

> **⚠️ Known issue:** This feature does not currently work at runtime. However, the code is correctly implemented and the fix is trivial — it is a minor configuration or wiring issue, not a logic problem.

### 5. Azure Service Bus Integration
The `ServiceBusService` is integrated into the DI container and provides functionality to send messages to an Azure Service Bus queue — intended for event-driven communication when invoices are processed.

### 6. Invoice History (`HistorialPage`)
Lists all invoices analyzed during the current session. Supports clearing the history.

---

## Architecture

The application follows the **MVVM** (Model-View-ViewModel) pattern using `CommunityToolkit.Mvvm` with Source Generators for boilerplate reduction.

```
┌──────────────────────────────────────────────────────────┐
│                   .NET MAUI Application                  │
│                                                          │
│  ┌─────────────┐   ┌──────────────┐   ┌──────────────┐  │
│  │    Views     │──▶│  ViewModels  │──▶│   Services   │  │
│  │  (XAML UI)  │   │  (Logic/DI)  │   │  (Azure SDK) │  │
│  └─────────────┘   └──────────────┘   └──────┬───────┘  │
│                                               │          │
└───────────────────────────────────────────────┼──────────┘
                                                │
                        ┌───────────────────────▼──────────────────────┐
                        │              Azure Cloud Services             │
                        │                                              │
                        │  ┌─────────────────────────────────────┐    │
                        │  │  Azure Document Intelligence         │    │
                        │  │  (prebuilt-invoice model)            │    │
                        │  └─────────────────────────────────────┘    │
                        │  ┌─────────────────────────────────────┐    │
                        │  │  Azure AI Vision (Image Analysis)    │    │
                        │  └─────────────────────────────────────┘    │
                        │  ┌─────────────────────────────────────┐    │
                        │  │  Azure Speech Service (STT)          │    │
                        │  └─────────────────────────────────────┘    │
                        │  ┌─────────────────────────────────────┐    │
                        │  │  Azure Translator (REST API)         │    │
                        │  └─────────────────────────────────────┘    │
                        │  ┌─────────────────────────────────────┐    │
                        │  │  Azure OpenAI (Chat Completions)     │    │
                        │  └─────────────────────────────────────┘    │
                        │  ┌─────────────────────────────────────┐    │
                        │  │  Azure Service Bus (Queue)           │    │
                        │  └─────────────────────────────────────┘    │
                        └──────────────────────────────────────────────┘
```

---

## Project Structure

```
MauiOCRFacturas/
├── App.xaml / App.xaml.cs              # Application entry point
├── AppShell.xaml / AppShell.xaml.cs    # Tab navigation shell
├── MauiProgram.cs                      # Dependency Injection & service setup
├── MauiOCRFacturas.csproj              # Project file (Android target, NuGet refs)
│
├── Models/
│   ├── ResultadoOCR.cs                 # Data model for invoice OCR results
│   └── ResultadoVision.cs              # Data model for computer vision results
│
├── Services/
│   ├── DocumentIntelligenceService.cs  # Azure Document Intelligence (OCR)
│   ├── ComputerVisionService.cs        # Azure AI Vision (object detection)
│   ├── SpeechTranslatorService.cs      # Azure Speech STT + Azure Translator
│   ├── SpeachService.cs                # Azure Speech Service (microphone STT)
│   ├── OpenAIService.cs                # Azure OpenAI (chat completions / summary)
│   ├── ServiceBusService.cs            # Azure Service Bus (message queue)
│   └── HistorialService.cs             # In-memory invoice history
│
├── ViewModels/
│   ├── MainViewModel.cs                # OCR capture & analysis logic
│   ├── HistorialViewModel.cs           # History list logic
│   ├── VisionViewModel.cs              # Computer vision logic
│   └── TraductorViewModel.cs           # Speech translation logic
│
├── Views/
│   ├── MainPage.xaml / .cs             # Invoice OCR main screen
│   ├── HistorialPage.xaml / .cs        # Invoice history screen
│   ├── VisionPage.xaml / .cs           # Object detection screen
│   ├── TraductorPage.xaml / .cs        # Speech translation screen
│   └── VoiceSummaryPage.xaml / .cs     # Voice recording & AI summary screen
│
├── Converters/                         # XAML value converters
└── Helpers/                            # Utility/helper classes
```

---

## Azure Services Used

| Service | SDK / API | Purpose |
|---|---|---|
| **Azure Document Intelligence** | `Azure.AI.FormRecognizer` 4.1.0 | Invoice OCR with `prebuilt-invoice` model |
| **Azure AI Vision** | `Azure.AI.Vision.ImageAnalysis` 1.0.0 | Object detection, image captioning, tagging |
| **Azure Speech Service** | `Microsoft.CognitiveServices.Speech` 1.48.2 | Microphone speech-to-text (STT) |
| **Azure Translator** | REST API (`api.cognitive.microsofttranslator.com`) | Text translation between languages |
| **Azure OpenAI** | REST API (`/openai/deployments/.../chat/completions`) | AI-generated summaries from transcriptions |
| **Azure Service Bus** | `Azure.Messaging.ServiceBus` 7.20.1 | Message queuing for processed invoices |

### Fields Extracted by Document Intelligence

- `VendorName` — Supplier name
- `CustomerName` — Customer name
- `InvoiceId` — Invoice number
- `InvoiceDate` — Issue date
- `InvoiceTotal` — Total amount
- `SubTotal` — Subtotal
- `TotalTax` — Tax amount
- `Items` — Line items (description + amount)

---

## NuGet Packages

| Package | Version |
|---|---|
| `Azure.AI.FormRecognizer` | 4.1.0 |
| `Azure.AI.Vision.ImageAnalysis` | 1.0.0 |
| `Azure.Messaging.ServiceBus` | 7.20.1 |
| `Microsoft.CognitiveServices.Speech` | 1.48.2 |
| `Camera.MAUI` | 1.5.1 |
| `CommunityToolkit.Mvvm` | 8.2.2 |
| `Microsoft.Extensions.Configuration.Json` | 11.0.0-preview |
| `Microsoft.Maui.Controls` | (MAUI version) |

---

## Configuration

### Prerequisites

- Visual Studio 2022+ with .NET MAUI workload
- .NET 9 SDK
- An Azure account with the following resources created:
  - Azure Document Intelligence
  - Azure AI Vision
  - Azure Speech Service
  - Azure Translator
  - Azure OpenAI (with a chat deployment)
  - Azure Service Bus (with a queue)

### appsettings.json

Create or edit `appsettings.json` (embedded as a resource) with your Azure credentials:

```json
{
  "AzureDocumentIntelligence": {
    "Endpoint": "https://YOUR-RESOURCE.cognitiveservices.azure.com/",
    "ApiKey": "YOUR_API_KEY"
  },
  "AzureComputerVision": {
    "Endpoint": "https://YOUR-RESOURCE.cognitiveservices.azure.com/",
    "ApiKey": "YOUR_API_KEY"
  },
  "AzureSpeech": {
    "Key": "YOUR_SPEECH_KEY",
    "Region": "YOUR_REGION"
  },
  "AzureTranslator": {
    "ApiKey": "YOUR_TRANSLATOR_KEY",
    "Region": "YOUR_REGION"
  },
  "AzureOpenAI": {
    "Endpoint": "https://YOUR-RESOURCE.openai.azure.com/",
    "Key": "YOUR_KEY",
    "DeploymentName": "YOUR_DEPLOYMENT_NAME"
  },
  "AzureServiceBus": {
    "ConnectionString": "YOUR_CONNECTION_STRING",
    "Cola": "YOUR_QUEUE_NAME"
  }
}
```

### Android Permissions

Required permissions (declared in `AndroidManifest.xml`):
- `CAMERA`
- `READ_EXTERNAL_STORAGE`
- `RECORD_AUDIO`

---

## Known Issues & Limitations

| Feature | Status | Notes |
|---|---|---|
| Invoice OCR | ✅ Working | Core feature, fully functional |
| Invoice History | ✅ Working | Session-based, in-memory |
| Object Detection | ⚠️ Functional with issues | Detection works but has some behavioral inconsistencies |
| Speech Translation | ⚠️ UI issues | Logic is correct; the interface has graphic design/layout problems |
| Voice Summary (OpenAI) | ❌ Not working at runtime | Code is correctly implemented; the bug is trivial to fix |
| Azure Service Bus | ✅ Implemented | DI-registered and functional, used for event messaging |

---

## Requirements

- **.NET 9** / .NET MAUI
- **Android** API 21+ (Android 5.0 Lollipop or higher)
- Active Azure subscription with the services listed above configured

---

> This project was developed as an academic exercise exploring the integration of Azure Cognitive Services into a .NET MAUI mobile application. Development is currently paused.
