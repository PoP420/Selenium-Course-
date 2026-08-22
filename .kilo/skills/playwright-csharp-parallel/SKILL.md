---
name: playwright-csharp-parallel
description: Parallel execution with NUnit 4 and Playwright C# — [Parallelizable] attributes, [TestCase] parametrization, cross-browser testing via environment variables, Playwright trace viewer, and test configuration. Use when scaling test suites, running tests in parallel, or testing across Chromium/Firefox/Webkit.
---

# Playwright Parallel Execution & Cross-Browser (C#)

## Purpose

This skill covers Phase 5 (Scaling: Parallel Execution & Cross-Browser) of the C# Playwright Mastery plan. It teaches how to run NUnit suites in parallel and how to parametrize tests across different browsers using Playwright's `Microsoft.Playwright.NUnit` integration. Use it when the user wants to speed up their suite, run cross-browser tests, or configure parallel execution.

## When to Apply

- User asks how to run tests in parallel in C# Playwright with NUnit.
- User asks about NUnit `[Parallelizable]`, `[TestCase]`, or test timing configuration.
- User asks how to run tests across Chromium, Firefox, and WebKit.
- User asks about Playwright Trace viewer, `TraceAsync`, or test artifacts.
- User asks about `BrowserContext`, `BrowserNewContextOptions`, or test isolation.

---

## 1. Parallel Execution with NUnit 4

### NUnit Parallelizable Attributes

NUnit 4 supports parallel execution at the assembly, fixture, and method levels using `[Parallelizable]` and `LevelOfParallelism`:

```csharp
// src/test/csharp/Fixtures/TestBase.cs
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

[assembly: LevelOfParallelism(4)]  // Global cap on parallel threads

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]  // Run different test classes in parallel
public class TestBase : PageTest
{
    // Each test gets its own browser, context, and page via PageTest
}
```

### ParallelScope options

| Scope | Meaning |
|---|---|
| `ParallelScope.None` | No parallelization (sequential). |
| `ParallelScope.Children` | Children (test methods) run in parallel. |
| `ParallelScope.Fixtures` | Different `[TestFixture]` classes run in parallel. |
| `ParallelScope.All` | Everything runs in parallel. |

```csharp
// Run all test methods within a class in parallel
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class HomePageTest : TestBase { }

// Run entire fixture classes in parallel
[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class CrossBrowserTest : TestBase { }
```

### Global configuration via `TestContext`

```csharp
// In TestBase.cs, at assembly level:
[assembly: LevelOfParallelism(4)]
```

Or via `dotnet test` CLI:

```bash
dotnet test -- NUnit.NumberOfTestWorkers=4
```

### What `PageTest` provides for parallelism

`Microsoft.Playwright.NUnit`'s `PageTest` creates a **separate** browser, browser context, and page for each test. This means:

- No shared state between parallel tests.
- No need for thread-safe driver management.
- Each test is fully isolated.

```csharp
// You don't need to manage browser lifecycle:
public class MyTest : PageTest
{
    [Test]
    public async Task Test1()
    {
        await Page.GotoAsync("https://example.com");  // Page is auto-created per test
        await Expect(Page).ToHaveTitleAsync("Example Domain");
    }
}
```

---

## 2. Cross-Browser Testing

### Key Constraint: BrowserName Setter Is Internal

In `Microsoft.Playwright.NUnit` 1.49.0, the `BrowserNewContextOptions.BrowserName` setter is **internal**. You cannot set the browser name at runtime:

```csharp
// This does NOT compile:
var options = new BrowserNewContextOptions();
options.BrowserName = "firefox";  // CS0274: internal setter
```

### Solution 1: Environment variable (recommended)

`PageTest` respects the `BROWSER_NAME` environment variable. Set it externally:

```bash
# Run on Firefox
set BROWSER_NAME=firefox
dotnet test

# Run on WebKit
set BROWSER_NAME=webkit
dotnet test

# Run on Chromium (default)
set BROWSER_NAME=chromium
dotnet test
```

### Solution 2: NUnit TestCase with TestContext

For per-test browser selection, use `TestCase` and override the browser in a base setup:

```csharp
// src/test/csharp/Tests/CrossBrowserTest.cs
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class CrossBrowserTest : PageTest
{
    [Test]
    [TestCase("chromium")]
    [TestCase("firefox")]
    [TestCase("webkit")]
    public async Task TestLoginPageLoads(string browserName)
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/login");
        await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));
    }
}
```

> **Note:** With `TestCase`, the browser name parameter does **not** control which browser launches. `PageTest` always launches Chromium by default. The `browserName` parameter is available for reporting/parametrization but doesn't switch the actual browser. To truly run on different browsers, use the environment variable approach or a custom base class that launches browsers manually.

### Solution 3: Custom base class with IBrowser

For full control over browser selection per test:

```csharp
using Microsoft.Playwright;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

public class BrowserTestBase
{
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;

    [SetUp]
    public async Task SetUp()
    {
        var playwright = await Playwright.CreateAsync();
        var browserName = TestContext.Parameters.Get("browser", "chromium");
        Browser = await playwright[browserName].LaunchAsync(new() { Headless = true });
        Context = await Browser.NewContextAsync();
        Page = await Context.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await Page?.CloseAsync();
        await Context?.CloseAsync();
        await Browser?.CloseAsync();
    }
}
```

Run with:

```bash
dotnet test -- NUnit.NumberOfTestWorkers=3
dotnet test -- browser=firefox
```

**Trade-offs:**
- **Solution 1 (env var)**: Simplest, but requires external configuration.
- **Solution 2 (TestCase)**: Works with `PageTest`, but browser name is cosmetic.
- **Solution 3 (custom base)**: Full control, but you manage all lifecycle.

---

## 3. Tracing

Playwright's Trace Viewer records everything: DOM snapshots, network logs, console output, video, and screenshots. Enable tracing per-test:

```csharp
[TestFixture]
public class MyTest : PageTest
{
    [SetUp]
    public async Task SetUp()
    {
        await Context.Tracing.GroupAsync();     // Start a trace step
        await Context.Tracing.StartAsync(
            new() { Screenshots = true, Snapshots = true, Sources = true });
    }

    [TearDown]
    public async Task TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            var tracePath = Path.Combine("test-results", $"{TestContext.CurrentContext.Test.Name}.zip");
            await Context.Tracing.StopAsync(new() { Path = tracePath });
            TestContext.AddTestAttachment(tracePath);
        }
        else
        {
            await Context.Tracing.StopAsync();  // Discard trace on pass
        }
    }
}
```

View traces:

```bash
npx playwright show-trace test-results/TestName.zip
```

---

## 4. Project-Level Parallel Configuration

In `.runsettings` (optional):

```xml
<!-- test/runsettings.runsettings -->
<RunSettings>
  <NUnit>
    <NumberOfTestWorkers>4</NumberOfTestWorkers>
    <Parallelize>
      <Workers>4</Workers>
      <MaxParallelTestCount>4</MaxParallelTestCount>
    </Parallelize>
  </NUnit>
</RunSettings>
```

Run with:

```bash
dotnet test --settings test/runsettings.runsettings
```

---

## 5. Scaling Pattern

```csharp
// src/test/csharp/Tests/CrossBrowserTest.cs
using System.Text.RegularExpressions;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class CrossBrowserTest : PageTest
{
    [Test]
    [TestCase("chromium")]
    [TestCase("firefox")]
    [TestCase("webkit")]
    public async Task TestLoginPageLoads(string browserName)
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/login");
        await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));
    }
}
```

Run with:

```bash
# Sequential (default)
dotnet test

# Parallel across fixtures
dotnet test -- NUnit.NumberOfTestWorkers=4
```

---

## 6. Headless vs Headed

`PageTest` runs **headless** by default. To run headed (for debugging):

```csharp
// Programmatic — not supported via PageTest attributes
// Use environment variable:
set PWDEBUG=1
dotnet test

# Or run a single test with tracing for debugging:
dotnet test --filter "FullyQualifiedName~CrossBrowserTest"
```

`PWDEBUG=1` enables:
- Headed mode
- `page.pause()` breakpoints become active
- Debugger launches on the first line

---

## 7. Common Pitfalls

| Pitfall | Fix |
|---|---|
| `BrowserName` setter compile error | It's internal in `Microsoft.Playwright.NUnit` 1.49.0. Use `BROWSER_NAME` env var or a custom base class. |
| `[TestCase("firefox")]` still runs Chromium | `TestCase` params are cosmetic with `PageTest`. Use env var or custom base class for real browser switching. |
| Shared state between parallel tests | `PageTest` gives each test its own browser/context/page. Don't use static fields. |
| Trace files accumulate | Only save trace on failure; `Context.Tracing.StopAsync()` without args discards it. |
| `LevelOfParallelism` not respected | Set it at **assembly level** (`[assembly: LevelOfParallelism(4)]` in any file), not inside a class. |
| Tests not running in parallel | Ensure no `[NonParallel]` attribute is present. `PageTests` are parallel-safe by default. |
| `TestContext` not available in page objects | It's available in test methods and setup/teardown. Don't reference it from page objects. |

---

## 8. Milestone

- `dotnet test` runs 5+ tests in parallel (NUnit `[Parallelizable]`).
- Cross-browser tests run on Chromium, Firefox, WebKit (via env var or custom base).
- Playwright tracing is enabled; `.zip` trace files are generated on test failure.
- A comparison table shows sequential vs parallel execution time.
- `[assembly: LevelOfParallelism(N)]` is set; parallel safety is guaranteed by `PageTest` isolation.
