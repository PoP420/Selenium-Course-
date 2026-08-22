# Phase 3 — Debugging & Evidence Capture

**Time:** 3–4 days

---

## Objectives

- Catch and log Selenium exceptions meaningfully in C#.
- Automatically capture screenshots on test failure via NUnit `SetUp` / `TearDown`.
- Add structured logging so failures are traceable without re-running.
- Generate NUnit test reports.

---

## Topics

### Exception Handling

- `NoSuchElementException` — element not found in DOM.
- `WebDriverTimeoutException` — explicit wait timed out.
- `ElementClickInterceptedException` — overlay or modal blocking click.
- `StaleElementReferenceException` — element detached from DOM after page update.

### Screenshot on Failure

Implement via NUnit `SetUp` / `TearDown`:

```csharp
// TestBase.cs
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;

namespace SeleniumCSharpTests;

public class TestBase
{
    protected IWebDriver driver;
    private const string ResultsDir = "test-results";

    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        var tempProfile = Path.Combine(Path.GetTempPath(), "chrome-profile-" + Guid.NewGuid());
        Directory.CreateDirectory(tempProfile);
        options.AddArgument($"--user-data-dir={tempProfile}");
        options.AddArgument("--disable-blink-features=AutomationControlled");
        
        driver = new ChromeDriver(options);
        driver.Manage().Window.Maximize();
    }

    [TearDown]
    public void TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
        {
            TakeScreenshot(TestContext.CurrentContext.Test.Name);
        }
        
        driver?.Dispose();
    }

    protected void TakeScreenshot(string testName)
    {
        try
        {
            Directory.CreateDirectory(ResultsDir);
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
            var screenshotPath = Path.Combine(ResultsDir, $"{safeName}_{timestamp}.png");
            
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(screenshotPath);
            TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
            TestContext.Progress.WriteLine($"Screenshot saved: {screenshotPath}");
        }
        catch (Exception ex)
        {
            TestContext.Progress.WriteLine($"Failed to capture screenshot: {ex.Message}");
        }
    }
}
```

### NUnit Test Reports

```powershell
dotnet test
```

---

## Practice

1. Add the `TakeScreenshot` method above to your `TestBase`.
2. Run a passing test — confirm no screenshot is created.
3. Run a failing test (e.g., wrong locator) — confirm a timestamped PNG drops into `test-results/`.
4. Add a C# logger (`Microsoft.Extensions.Logging` or `Serilog`) to `TestBase` and log driver start/stop events.

---

## Skills to Load

- `selenium-setup` — NUnit setup, screenshot-on-failure hooks, C# logging.

---

## Milestone

Any test in your suite that fails automatically drops a timestamped screenshot + log entry into `test-results/`. `dotnet test` produces readable test results.
