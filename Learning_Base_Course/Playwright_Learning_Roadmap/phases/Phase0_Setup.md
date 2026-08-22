# Phase 0 — Environment Setup

**Time:** Day 0 (1 session)

---

## Objectives

- Install .NET 8+ SDK, Visual Studio 2022 / Rider / VS Code with C# extensions, and Git.
- Install Playwright browser binaries (Chromium, Firefox, WebKit) via `playwright install`.
- Create a working NUnit project with `Microsoft.Playwright.NUnit`.
- Run your first passing test with zero manual driver configuration.

---

## Verified Setup (Windows 11)

### .NET SDK

Check .NET is installed:

```powershell
dotnet --version
```

If not, download from [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) (choose .NET 8 SDK or later).

### IDE

Install one of:

- Visual Studio 2022 Community Edition (with ".NET desktop development" workload)
- JetBrains Rider
- VS Code with C# Dev Kit extension

### Browsers

**Chrome is not required separately** — Playwright downloads its own Chromium, Firefox, and WebKit binaries. See below.

---

## Project Setup

### Option A: Use the existing project

```powershell
cd C:\Users\ajdpe\LearningsAI\SeleniumCodebaseCourse\PlaywrightCSharpTests
```

### Option B: Create your own project

```powershell
mkdir PlaywrightCSharpTests
cd PlaywrightCSharpTests
dotnet new nunit -n PlaywrightCSharpTests.Tests
dotnet sln add PlaywrightCSharpTests.Tests
dotnet add PlaywrightCSharpTests.Tests package Microsoft.Playwright.NUnit
dotnet build
```

The `Microsoft.Playwright.NUnit` package pulls in:

- `Microsoft.Playwright` — core browser automation (Chromium, Firefox, WebKit).
- `Microsoft.Playwright.NUnit` — `PlaywrightTest` base class, automatic page/browser/context fixtures.

### Install browser binaries

After restoring packages, run:

```powershell
dotnet build
playwright install
```

This downloads Chromium, Firefox, and WebKit binaries into a local cache (typically under `%USERPROFILE%\.cache\ms-playwright` on Windows). **No manual driver downloads are needed** — Playwright bundles its own browser binaries and browser-specific drivers, all managed by Playwright itself.

---

## Project Structure

```
PlaywrightCSharpTests/
├── PlaywrightCSharpTests.slnx
├── PlaywrightCSharpTests.csproj          ← main project (empty or shared utilities)
├── PlaywrightCSharpTests.Tests/
│   ├── PlaywrightCSharpTests.Tests.csproj
│   ├── playwright.config.cs              ← optional Playwright config
│   └── src/test/csharp/
│       ├── Fixtures/
│       │   └── CustomFixture.cs          ← shared setup/teardown
│       └── Tests/
│           └── VerifySetupTest.cs
├── .github/workflows/tests.yml
└── README.md
```

The **main project** (`PlaywrightCSharpTests.csproj`) holds page objects and utilities. The **test project** references it and runs tests.

---

## First Test

```csharp
// src/test/csharp/Tests/VerifySetupTest.cs
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class VerifySetupTest : PlaywrightTest
{
    [Test]
    public async Task VerifyChromiumLaunches()
    {
        await Page.GotoAsync("https://example.com");
        await Expect(Page).To_haveTitleAsync(new() { NameString = "Example Domain" });
    }
}
```

Key differences from Selenium:

- Tests extend `PlaywrightTest` (from `Microsoft.Playwright.NUnit`), not `TestBase`.
- `Page` is injected automatically — no manual `new ChromeDriver()`.
- No `WebDriverWait` needed — every `Page.GotoAsync()` and interaction auto-waits.
- Assertions use `Expect(...).To_haveTitleAsync(...)` with built-in retry.

Run:

```powershell
dotnet test --filter "FullyQualifiedName~VerifySetupTest"
```

---

## Troubleshooting

| Issue | Fix |
|---|---|
| `playwright install` fails | Ensure NuGet packages are restored (`dotnet build`). Check network access. |
| Browser window blocked by Cloudflare | Playwright uses `--no-sandbox` by default in CI; locally, try `Headless = false` and a real Chrome profile. |
| `Page` is null in tests | Ensure test class extends `PlaywrightTest` and uses `[Test]` from NUnit. |
| `Expect` method not found | Ensure `Microsoft.Playwright.NUnit` NuGet is installed in the test project. |
| Tests not discovered | Ensure test class names end with `Test` and methods have `[Test]`. |

---

## Skills to Load

- `playwright-csharp-setup` — .NET + Playwright project bootstrap, browser installation, PlaywrightTest base class.

---

## Milestone

`dotnet test --filter "FullyQualifiedName~VerifySetupTest"` runs successfully with no manual driver configuration. Playwright downloaded its own browsers and auto-resolved everything.
