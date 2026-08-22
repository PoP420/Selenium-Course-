---
name: selenium-csharp-setup
description: C# Selenium project bootstrap — .NET SDK project layout, NuGet packages, NUnit 4 configuration, ChromeOptions with temp profile, solution file, and Maven-style src/main/src/test split. Use when creating a new C# Selenium project, verifying the environment, or implementing TestBase with setup/teardown.
---

# Selenium Setup (C#)

## Purpose

This skill covers environment setup for C# Selenium test projects. It teaches how to bootstrap a .NET solution with a main project (`src/main/csharp`) and a separate test project (`src/test/csharp`), configure NUnit, and build a `TestBase` with driver lifecycle management. Use it when starting a new C# Selenium project, adding page objects, or wiring up the test runner.

## When to Apply

- User asks to set up Selenium in C#, create a .NET test project, or add NuGet packages.
- User needs a `TestBase` with `SetUp`/`TearDown` or ChromeOptions configuration.
- User asks about Selenium Manager, ChromeDriver, or driver version mismatches in C#.
- User asks about NUnit, `.csproj`, or solution structure.
- User wants the Maven-style `src/main` / `src/test` layout in C#.

---

## 1. Project Bootstrap

### Prerequisites

- .NET 10+ SDK
- Visual Studio 2022 / Rider / VS Code with C# extensions
- Chrome browser installed

### Option B: Two-project layout (recommended)

Mirrors Maven's `src/main` / `src/test` separation with a main project and a separate test project.

```
SeleniumCSharpTests/
  SeleniumCSharpTests.slnx
  SeleniumCSharpTests.csproj          ← main project (Selenium runtime only)
  src/
    main/
      csharp/                         ← page objects, base pages, utilities
  SeleniumCSharpTests.Tests/
    SeleniumCSharpTests.Tests.csproj  ← test project (references main)
    src/
      test/
        csharp/
          TestBase.cs
          ContactFormTest.cs
  bin/
  obj/
```

### Main project (`SeleniumCSharpTests.csproj`)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Selenium.WebDriver" Version="4.47.0" />
    <PackageReference Include="Selenium.Support" Version="4.47.0" />
    <PackageReference Include="Selenium.WebDriver.ChromeDriver" Version="151.0.7922.13800" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="src\main\csharp\**\*.cs" />
  </ItemGroup>

</Project>
```

- `EnableDefaultCompileItems=false` prevents the SDK from auto-including everything; we explicitly include only `src/main/csharp/**/*.cs`.
- `Selenium.Support` provides `SelectElement` and `WebDriverWait`.
- `Selenium.WebDriver.ChromeDriver` pins the ChromeDriver version matching Chrome 151.

### Test project (`SeleniumCSharpTests.Tests/SeleniumCSharpTests.Tests.csproj`)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.0" />
    <PackageReference Include="NUnit" Version="4.3.2" />
    <PackageReference Include="NUnit.Analyzers" Version="4.7.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="5.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SeleniumCSharpTests.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="src\test\csharp\**\*.cs" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="NUnit.Framework" />
  </ItemGroup>

</Project>
```

- `ProjectReference` links the test project to the main project.
- `NUnit3TestAdapter` discovers and runs NUnit tests via `dotnet test`.
- `coverlet.collector` enables code coverage.

### Solution file

```bash
dotnet new sln -n SeleniumCSharpTests
dotnet sln add SeleniumCSharpTests.csproj
dotnet sln add SeleniumCSharpTests.Tests\SeleniumCSharpTests.Tests.csproj
```

Run with:

```bash
dotnet test SeleniumCSharpTests.slnx
```

---

## 2. Selenium Manager

Selenium 4.6+ ships with Selenium Manager, which auto-detects the installed browser version and downloads the matching driver binary.

### What it solves

- No manual `chromedriver` downloads.
- No version mismatch between browser and driver.

### Where it can still fail

- **Offline environments** with no cached driver binary.
- **Custom driver paths** required by corporate policies.
- **Browser not installed** or installed in a non-standard location.

### Override driver path (when needed)

```csharp
var options = new ChromeOptions();
options.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
// Selenium Manager handles this by default; only override if required
var service = ChromeDriverService.CreateDefaultService(@"C:\path\to\driver");
var driver = new ChromeDriver(service, options);
```

Prefer letting Selenium Manager handle this unless you have a specific reason to override.

---

## 3. TestBase Class

```csharp
// src/test/csharp/TestBase.cs
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Tests;

public class TestBase
{
    protected IWebDriver driver;
    protected WebDriverWait wait;

    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        driver = new ChromeDriver(options);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Dispose();
    }
}
```

### ChromeOptions patterns

```csharp
var options = new ChromeOptions();

// Run headed (required for Angular SPAs)
options.AddArgument("--start-maximized");

// Temp profile to avoid profile lock issues
string tempProfile = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "chrome-profile-" + Guid.NewGuid());
options.AddArgument("--user-data-dir=" + tempProfile);

// Disable automation flag
options.AddArgument("--disable-blink-features=AutomationControlled");
```

All test classes extend `TestBase` to inherit `driver` and `wait`.

---

## 4. Verification Script

```csharp
// src/test/csharp/VerifySetupTest.cs
using NUnit.Framework;
using OpenQA.Selenium;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class VerifySetupTest : TestBase
{
    [Test]
    public void VerifyChromeLaunches()
    {
        driver.Navigate().GoToUrl("https://example.com");
        Assert.That(driver.Title, Does.Contain("Example"));
    }
}
```

Run with:

```bash
dotnet test --filter "FullyQualifiedName~VerifySetupTest"
```

If it runs without manual driver setup, Selenium Manager is working.

---

## 5. Logging

Use `Microsoft.Extensions.Logging` or a simple static logger for traceability.

```csharp
// src/main/csharp/Utils/TestLogger.cs
using Microsoft.Extensions.Logging;

namespace SeleniumCSharpTests.Utils;

public static class TestLogger
{
    private static ILoggerFactory _factory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
    });

    public static ILogger CreateLogger<T>() => _factory.CreateLogger<T>();
}
```

In tests:

```csharp
private static readonly ILogger _logger = TestLogger.CreateLogger<ContactFormTest>();

[Test]
public void TestContactFormSubmit()
{
    _logger.LogInformation("Navigating to contact page");
    driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/contact");
    // ...
}
```

---

## 6. Common Pitfalls

| Pitfall | Fix |
|---|---|
| Driver version mismatch | Let Selenium Manager handle it; don't manually download drivers unless required. |
| Browser window too small for responsive elements | Call `options.AddArgument("--start-maximized")` or `driver.Manage().Window.Maximize()`. |
| Tests leave browser processes running | Ensure `driver?.Dispose()` is in `[TearDown]`. |
| Profile lock on repeated runs | Use a temp `--user-data-dir` per test run. |
| `CS0246` for Selenium types | Verify `Selenium.WebDriver` and `Selenium.Support` packages are in the main project, not the test project. |
| Tests not discovered | Ensure test class names end with `Test` and methods have `[Test]`. |

---

## 7. Milestone

- `dotnet test` runs `VerifySetupTest` successfully with no manual driver configuration.
- `TestBase` provides `driver` and `wait` to all tests via inheritance.
- Main project compiles page objects; test project compiles tests and references main.
- Solution file `SeleniumCSharpTests.slnx` builds both projects cleanly.
