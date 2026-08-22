# Phase 5 — Parallel Execution & Selenium Grid

**Time:** 4–5 days

---

## Objectives

- Speed up the test suite with NUnit parallel execution.
- Set up a local Selenium Grid with Docker.
- Run the same suite cross-browser (Chrome + Firefox) via Grid.
- Measure and document speedup.

---

## Topics

### Parallel Execution with NUnit

- Configure parallel execution in `nunit.config` or via `[Parallelizable]` attribute.
- Run with `dotnet test`.
- Fixture scope matters: keep driver setup in `[SetUp]` for parallel safety.
- Measure sequential vs. parallel execution time.

### Selenium Grid with Docker

- Start a standalone Grid: `docker run -d -p 4444:4444 --shm-size=2g selenium/standalone-chrome`.
- Start a full hub + node setup with Chrome and Firefox nodes.
- Access the Grid console at `http://localhost:4444/ui`.

### Pointing Tests at the Grid

- Replace `new ChromeDriver()` with `new RemoteWebDriver()`.
- Configure `ChromeOptions` or `FirefoxOptions` directly.
- Parametrize tests for cross-browser execution (`chrome`, `firefox`).

---

## Practice

1. Run your Phase 4 suite sequentially and record the time.
2. Enable NUnit parallel config and record the speedup.
3. Start the Selenium Grid in Docker.
4. Update your `TestBase` to switch between local and Grid based on an environment variable.
5. Run the same suite against the Grid with cross-browser parametrization.

```csharp
// TestBase.cs — Grid-aware setup
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;

namespace SeleniumCSharpTests;

public class TestBase
{
    protected IWebDriver driver;
    private static readonly string GridUrl = Environment.GetEnvironmentVariable("SELENIUM_GRID_URL") ?? "http://localhost:4444/wd/hub";
    private static readonly bool UseGrid = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("SELENIUM_GRID_URL"));

    [SetUp]
    public void SetUp()
    {
        if (UseGrid)
        {
            var browser = Environment.GetEnvironmentVariable("BROWSER") ?? "chrome";
            if (browser.Equals("firefox", StringComparison.OrdinalIgnoreCase))
            {
                var options = new OpenQA.Selenium.Firefox.FirefoxOptions();
                driver = new RemoteWebDriver(new Uri(GridUrl), options);
            }
            else
            {
                var options = new ChromeOptions();
                driver = new RemoteWebDriver(new Uri(GridUrl), options);
            }
        }
        else
        {
            var options = new ChromeOptions();
            driver = new ChromeDriver(options);
        }
        driver.Manage().Window.Maximize();
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Dispose();
    }
}
```

```csharp
// Tests/CrossBrowserTest.cs
using NUnit.Framework;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class CrossBrowserTest : TestBase
{
    [Test]
    [TestCase("chrome")]
    [TestCase("firefox")]
    public void TestLoginCrossBrowser(string browser)
    {
        Environment.SetEnvironmentVariable("BROWSER", browser);
        // Same test logic runs on both browsers via Grid
    }
}
```

---

## Skills to Load

- `selenium-parallel-grid` — NUnit parallel config, Selenium Grid with Docker, cross-browser testing.

---

## Milestone

| Configuration | Time |
|---|---|
| Sequential, local | ___s |
| Parallel (NUnit), local | ___s |
| Parallel, Grid (Chrome + Firefox) | ___s |

Document the before/after execution time in a table. Show the speedup percentage.
