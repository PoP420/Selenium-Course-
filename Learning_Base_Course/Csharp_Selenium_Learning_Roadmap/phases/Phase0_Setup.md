# Phase 0 — Environment Setup

**Time:** Day 0 (1 session)

---

## Objectives

- Install .NET 8+ SDK, Visual Studio 2022 / Rider / VS Code, and Git.
- Verify Selenium Manager auto-resolves browser drivers.
- Create a working NUnit project with Selenium via NuGet.
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

Install Google Chrome from [google.com/chrome](https://www.google.com/chrome/). Selenium Manager will auto-resolve the matching ChromeDriver.

---

## Project Setup

### Option A: Use the existing project

```powershell
cd C:\Users\ajdpe\LearningsAI\SeleniumCodebaseCourse\SeleniumCSharpTests
```

### Option B: Create your own project

```powershell
mkdir SeleniumCSharpTests
cd SeleniumCSharpTests
dotnet new nunit -n SeleniumCSharpTests
cd SeleniumCSharpTests
```

Add Selenium packages:
```powershell
dotnet add package Selenium.WebDriver
dotnet add package Selenium.WebDriver.ChromeDriver
```

Restore and build:
```powershell
dotnet restore
dotnet build
```

---

## First Test

Replace `UnitTest1.cs` with:

```csharp
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumCSharpTests;

[TestFixture]
public class VerifySetupTests
{
    [Test]
    public void Chrome_Should_Launch_And_Navigate_To_Example()
    {
        using var driver = new ChromeDriver();
        driver.Manage().Window.Maximize();
        driver.Navigate().GoToUrl("https://example.com");
        
        Assert.That(driver.Title, Does.Contain("Example"));
    }
}
```

Run:
```powershell
dotnet test --filter "FullyQualifiedName~VerifySetupTests"
```

---

## Troubleshooting

| Issue | Fix |
|---|---|
| `dotnet` not recognized | Add .NET SDK to System PATH, restart PowerShell |
| Chrome blocked by Cloudflare | Run in headed mode, use a fresh temp profile |
| Element not found on `practicesoftwaretesting.com` | Site is an Angular SPA — use explicit waits, not headless with mismatched user-agent |
| CDP warning for Chrome 151 | Informational only. Ignore unless you use DevTools features. |

---

## Skills to Load

- `selenium-setup` — NuGet bootstrap, Selenium Manager, verification scripts.

---

## Milestone

`dotnet test` runs successfully with no manual driver configuration. Selenium Manager resolved the driver automatically.
