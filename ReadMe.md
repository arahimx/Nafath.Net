# 🛡️ Nafath.Net SDK

[![Build & Test](https://github.com/you/Nafath.Net/actions/workflows/ci.yml/badge.svg)](https://github.com/you/Nafath.Net/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Nafath.Net.svg?label=NuGet)](https://www.nuget.org/packages/Nafath.Net/)
[![License: MIT](https://img.shields.io/badge/license-MIT-green.svg)](LICENSE)

**Nafath.Net** is a modern .NET SDK for integrating with the Nafath authentication system provided by SDAIA.


**Nafath.Net** is a clean and modern .NET SDK for integrating with the official [Nafath](https://www.nafath.sa/) authentication system provided by **SDAIA (Saudi Arabia)**.

---

## 🔧 Requirements

| Requirement         | Version                       |
| ------------------- | ----------------------------- |
| .NET SDK            | `8.0+`                        |
| Supported Platforms | .NET 8, 7                     |
| IDE                 | VS Code / Visual Studio 2022+ |

---

## 📦 Installation

Until it's published to NuGet, add a local project reference:

```bash
dotnet add reference ../Nafath.Net/Nafath.Net.csproj
```

Once available on NuGet:

```bash
dotnet add package Nafath.Net
```

---

## ⚙️ Setup & Configuration

### 1. Register the SDK in `Program.cs`

```csharp
builder.Services.AddNafath(options =>
{
    options.BaseUrl = "https://auth.nafath.sa";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
});
```

> Use **User Secrets**, environment variables, or Azure Key Vault for secrets.

---

## 🔌 Minimal API Example

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddNafath(options =>
{
    options.BaseUrl = "https://auth.nafath.sa";
    options.ClientId = builder.Configuration["Nafath:ClientId"];
    options.ClientSecret = builder.Configuration["Nafath:ClientSecret"];
});

var app = builder.Build();

app.MapPost("/nafath/start", async (INafathService nafath, string idNumber, string requestId) =>
{
    var result = await nafath.StartSessionAsync(idNumber, requestId);
    return Results.Ok(result);
});

app.MapGet("/nafath/verify/{transactionId}", async (INafathService nafath, string transactionId) =>
{
    var result = await nafath.VerifyStatusAsync(transactionId);
    return Results.Ok(result);
});

app.Run();
```

---

## 📤 Available Methods

### 🔹 `StartSessionAsync(string idNumber, string requestId)`

Initiates an authentication session.

### 🔹 `VerifyStatusAsync(string transactionId)`

Checks if the user accepted/rejected the request.

Both methods return strongly typed models with status and error information.

---

## 🛡️ Error Handling & Logging

Robust built-in error handling with structured logging:

* Network issues (timeouts, 500s)
* Unexpected API responses
* Logs both outbound payload and response content

Add breakpoints inside `NafathService.cs` to debug calls directly.

---

## 🧪 Sample Unit Test

```csharp
public class NafathServiceTests
{
    [Fact]
    public async Task StartSessionAsync_Returns_ValidResponse()
    {
        // Arrange
        var httpClient = new HttpClient(new MockHttpMessageHandler());
        var options = Options.Create(new NafathOptions { BaseUrl = "https://fake.api" });
        var logger = new LoggerFactory().CreateLogger<NafathService>();
        var service = new NafathService(httpClient, options, logger);

        // Act
        var result = await service.StartSessionAsync("1234567890", "test-req");

        // Assert
        Assert.NotNull(result);
        Assert.True(string.IsNullOrEmpty(result.Error));
    }
}
```

🔸 You can mock `HttpMessageHandler` using `Moq` or `RichardSzalay.MockHttp`.

---

Or use GitHub Actions to automate this (see `.github/workflows/ci.yml` example).

---

## 🤝 Contributing

We welcome contributions! Please:

* Submit clear PRs
* Open issues for feature requests or bugs
* Follow .NET and API design best practices

---

## 📜 License

MIT License © 2025 — Arahimx
---

## 📎 References

* [Nafath Portal](https://www.nafath.sa/)
* [SDAIA Developer Portal](https://developer.sdaia.gov.sa/)
* [ASP.NET Core Docs](https://learn.microsoft.com/en-us/aspnet/core)
