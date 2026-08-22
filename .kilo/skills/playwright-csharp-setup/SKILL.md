---
name: playwright-csharp-setup
description: C# Playwright project bootstrap — .NET SDK project layout, NuGet packages (Microsoft.Playwright.NUnit), browser installation (playwright install), TestBase inheriting from PageTest, and Maven-style src/main/src/test split. Use when creating a new C# Playwright project, verifying the environment, or implementing TestBase with setup/teardown.
---

# Playwright Setup (C#)

## Purpose

This skill covers environment setup for C# Playwright test projects. It teaches how to bootstrap a .NET solution with a main project (`src/main/csharp`) and a separate test project (`src/test/csharp`), install browser binaries, configure NUnit with `Microsoft.Playwright.NUnit`, and build a `TestBase` inheriting from `PageTest`. Use it when starting a new C# Playwright project, adding page objects, or wiring up the test runner.

## When to Apply

- User asks to set up Playwright in C#, create a .NET test project, or add NuGet packages.
- User needs a `TestBase` with `SetUp`/`TearDown` or browser context configuration.
- User asks about Playwright browser installation, `playwright install`, or driver version mismatches in C#.
- User asks about NUnit, `.csproj`, or solution structure.
- User wants the Maven-style `src/main` / `src/test` layout in C#.

---

## 1. Project Bootstrap

### Prerequisites

- .NET 10+ SDK
- Visual Studio 2022 / Rider / VS Code with C# extensions
- Playwright browser binaries (Chromium, Firefox, WebKit)

### Project layout (two-project, Maven-style)

```
PlaywrightCSharpTests/
  PlaywrightCSharpTests.csproj          ← main project (Playwright runtime + page objects)
  PlaywrightCSharpTests.Tests/
    PlaywrightCSharpTests.Tests.csproj  ← test project (references main, NUnit, Playwright.NUnit)
    src/
      test/
        csharp/
          Fixtures/
            TestBase.cs
          Tests/
            VerifySetupTest.cs
  src/
    main/
      csharp/
        Pages/
          BasePage.cs
          ContactPage.cs
          LoginPage.cs
        Utils/
          TestLogger.cs
  bin/
  obj/
```

### Main project (`PlaywrightCSharpTests.csproj`)

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
    <PackageReference Include="Microsoft.Playwright" Version="1.49.0" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="src\main\csharp\Pages\**\*.cs" />
    <Compile Include="src\main\csharp\Utils\**\*.cs" />
  </ItemGroup>

</Project>
```

- `EnableDefaultCompileItems=false` prevents the SDK from auto-including everything; we explicitly include only `src/main/csharp/**/*.cs`.
- `Microsoft.Playwright` provides the core `IPage`, `ILocator`, `IPlaywright` types.

### Test project (`PlaywrightCSharpTests.Tests/PlaywrightCSharpTests.Tests.csproj`)

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
    <PackageReference Include="Microsoft.Playwright.NUnit" Version="1.49.0" />
    <PackageReference Include="NUnit" Version="4.3.2" />
    <PackageReference Include="NUnit.Analyzers" Version="4.7.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="5.0.0" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\PlaywrightCSharpTests.csproj" />
  </ItemGroup>

  <ItemGroup>
    <Compile Include="src\test\csharp\**\*.cs" />
  </ItemGroup>

  <ItemGroup>
    <Using Include="NUnit.Framework" />
    <Using Include="Microsoft.Playwright.NUnit" />
    <Using Include="Microsoft.Playwright" />
  </ItemGroup>

</Project>
```

- `Microsoft.Playwright.NUnit` provides `PageTest` — the base class that auto-creates a browser, context, and `Page` for each test.
- `ProjectReference` links the test project to the main project.
- `NUnit3TestAdapter` discovers and runs NUnit tests via `dotnet test`.
- `coverlet.collector` enables code coverage.

### Solution file

```bash
dotnet new sln -n PlaywrightCSharpTests
dotnet new classlib -o PlaywrightCSharpTests
dotnet new nunit -o PlaywrightCSharpTests.Tests
dotnet sln add PlaywrightCSharpTests/PlaywrightCSharpTests.csproj
dotnet sln add PlaywrightCSharpTests.Tests/PlaywrightCSharpTests.Tests.csproj
dotnet add PlaywrightCSharpTests.Tests/PlaywrightCSharpTests.Tests.csproj reference PlaywrightCSharpTests/PlaywrightCSharpTests.csproj
```

Run with:

```bash
dotnet test
```

---

## 2. Browser Installation

Playwright downloads browser binaries separately from the NuGet package. After restoring packages, install the browsers:

### Method 1: MSBuild target (automatic on build)

The `Microsoft.Playwright` package includes an MSBuild target that runs `playwright install` during build. This happens automatically when you build:

```bash
dotnet build
```

If the build succeeds but browsers are missing, the target may have been skipped due to `EnableDefaultCompileItems=false`. In that case, use Method 2.

### Method 2: PowerShell script (manual)

The NuGet package ships a `playwright.ps1` script. After build, find it in the output directory and run it:

```powershell
# Navigate to the test project's bin output
cd PlaywrightCSharpTests.Tests/bin/Debug/net10.0

# The script expects Microsoft.Playwright.dll in the same directory
# Copy it from the NuGet package if needed:
cp (Join-Path (Get-Item (Get-Package Microsoft.Playwright).Source) "lib/netstandard2.0/Microsoft.Playwright.dll") .

# Run install
. .\playwright.ps1 install chromium
```

### Method 3: .NET global tool (alternative)

```bash
dotnet tool install --global Microsoft.Playwright.Cli
playwright install
```

### Installing specific browsers

```bash
playwright install          # all browsers (chromium, firefox, webkit)
playwright install chromium # chromium only
playwright install-deps     # system dependencies (Linux only)
```

### Where browsers are stored

| OS      | Path |
|---------|------|
| Windows | `%USERPROFILE%\AppData\Local\ms-playwright\` |
| macOS   | `~/Library/Caches/ms-playwright/` |
| Linux   | `~/.cache/ms-playwright/` |

---

## 3. TestBase Class

`Microsoft.Playwright.NUnit` provides `PageTest` as the base class. It handles the full browser lifecycle:

1. Launches a browser (Chromium by default)
2. Creates an isolated browser context
3. Creates a new `IPage`
4. Cleans up after each test

You only need to extend `PageTest`:

```csharp
// src/test/csharp/Fixtures/TestBase.cs
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightCSharpTests.Utils;

namespace PlaywrightCSharpTests.Tests;

public class TestBase : PageTest
{
    [SetUp]
    public async Task BaseSetUp()
    {
        TestLogger.LogInformation($"Starting test: {TestContext.CurrentContext.Test.Name}");
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            TestLogger.LogWarning($"Test FAILED: {TestContext.CurrentContext.Test.Name}");

            var testName = TestContext.CurrentContext.Test.Name;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
            var screenshotPath = Path.Combine("test-results", $"{safeName}_{timestamp}.png");

            Directory.CreateDirectory("test-results");
            await Page.ScreenshotAsync(new() { Path = screenshotPath });
            TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
        }
    }
}
```

### What `PageTest` provides

| Property / Method | Description |
|---|---|
| `Page` | The `IPage` instance for the current test — ready to use in every test method. |
| `Browser` | The `IBrowser` instance. |
| `BrowserContext` | The isolated `IBrowserContext` for this test. |
| `Page.GotoAsync()` | Navigate to a URL. |
| `Expect(Page)` | Fluent assertion helpers: `ToHaveTitleAsync`, `ToHaveURLAsync`, `ToHaveScreenshotAsync`. |
| `Expect(locator)` | Fluent assertion helpers: `ToBeVisibleAsync`, `ToContainTextAsync`, `ToHaveAttributeAsync`, etc. |

All test classes extend `TestBase` to inherit logging, screenshots-on-failure, and the `Page` property.

### Important: `BrowserName` setter is internal

The `BrowserNewContextOptions.BrowserName` setter is **internal** in `Microsoft.Playwright.NUnit` 1.49.0. You cannot set it at runtime per-test:

```csharp
// This does NOT work:
var options = new BrowserNewContextOptions();
options.BrowserName = "firefox";  // Compile error: setter is internal

// Instead, configure the browser via environment variables or test context.
```

To change the browser, use `SetPlaywrightScriptInjection` or environment variables. For per-test browser selection, see the **playwright-csharp-parallel** skill.

---

## 4. Verification Script

```csharp
// src/test/csharp/Tests/VerifySetupTest.cs
using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class VerifySetupTest : TestBase
{
    [Test]
    public async Task VerifyChromiumLaunches()
    {
        await Page.GotoAsync("https://example.com");
        await Expect(Page).ToHaveTitleAsync("Example Domain");
    }

    [Test]
    public async Task VerifyPageContent()
    {
        await Page.GotoAsync("https://example.com");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { NameString = "Example Domain" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("p", new() { HasText = "This domain is for use in" }))
            .ToContainTextAsync("This domain is for use in");
    }
}
```

Run with:

```bash
dotnet test --filter "FullyQualifiedName~VerifySetupTest"
```

If both tests pass, the Playwright setup is complete.

---

## 5. Logging

Use a simple static logger for traceability:

```csharp
// src/main/csharp/Utils/TestLogger.cs
using System;
using System.Diagnostics;

namespace PlaywrightCSharpTests.Utils;

public static class TestLogger
{
    public static void Log(LogLevel level, string message)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        Console.WriteLine($"[{timestamp}] [{level}] {message}");
        Debug.WriteLine($"[{timestamp}] [{level}] {message}");
    }

    public static void LogInformation(string message) => Log(LogLevel.Information, message);
    public static void LogWarning(string message) => Log(LogLevel.Warning, message);
    public static void LogError(string message) => Log(LogLevel.Error, message);
}

public enum LogLevel
{
    Information,
    Warning,
    Error
}
```

---

## 6. SPA / Angular Timing

Single-page apps (Angular, React, Vue) render content dynamically via JavaScript. `Page.GotoAsync()` returns once the `load` event fires, but the SPA may still be rendering components.

### Problem

```
Page loaded (DOM ready) → Angular still bootstrapping → locators not found → ElementHandleError
```

### Solutions

1. **Wait for a specific element** — most reliable:
   ```csharp
   await HomePage.ProductCard.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
   ```

2. **Use `WaitUntilState.NetworkIdle`** (waits for 500ms with no network activity):
   ```csharp
   await Page.GotoAsync(url, new() { WaitUntil = WaitUntilState.NetworkIdle });
   ```
   > **Warning**: Some sites (e.g., Cloudflare challenge, analytics beacons) never reach network idle. Use element-based waits as the primary strategy.

3. **Auto-waiting in locators** — Playwright's `Locator` methods (`ClickAsync`, `FillAsync`, `GetByText`) automatically wait for the element to be actionable. You rarely need explicit waits.

---

## 7. Common Pitfalls

| Pitfall | Fix |
|---|---|
| `playwright.ps1 : Could not find file 'Microsoft.Playwright.dll'` | Copy the DLL from the NuGet package (`lib/netstandard2.0/`) to the output directory, or use `dotnet tool install --global Microsoft.Playwright.Cli`. |
| Browser binary not found (`headless_shell.exe` missing) | Run `playwright install` after build. |
| Locators return 0 elements on SPA | Wait for a specific Angular component to render with `Locator.WaitForAsync()`. |
| `BrowserName` setter compile error | The setter is internal in `Microsoft.Playwright.NUnit` 1.49.0. Don't try to set it at runtime; configure via environment or test parameters. |
| `Thread.Sleep` instead of waiting | Use `Locator.WaitForAsync()`, `Expect().ToBeVisibleAsync()`, or `Page.WaitForLoadStateAsync()`. |
| `CS0246` for Playwright types | Verify `Microsoft.Playwright` and `Microsoft.Playwright.NUnit` are in the test project's `PackageReference`. |
| Assertions inside page objects | Assertions belong in test classes only. Page objects return data; tests assert. |

---

## 8. Milestone

- `dotnet build` compiles both projects with 0 errors, 0 warnings.
- `dotnet test` runs `VerifySetupTest` successfully.
- `TestBase` extends `PageTest` and provides `SetUp`/`TearDown` with logging and screenshots-on-failure.
- Browser binaries (Chromium) are installed and accessible.
- Solution file builds cleanly.
