# Phase 3 — Debugging & Evidence Capture

**Time:** 3–4 days

---

## Objectives

- Understand Playwright's built-in debugging tools (tracing, video, console logs).
- Automatically capture screenshots and traces on test failure.
- Add structured logging so failures are traceable without re-running.
- Use Playwright Inspector to pause and debug interactively.

---

## Topics

### Playwright Inspector

The Playwright Inspector is an interactive debugger that lets you:

- Pause test execution.
- Inspect the DOM.
- Run locator expressions against the live page.
- Step through actions.

Launch it with:

```powershell
setx PLAYWRIGHT_DEBUG "1"
dotnet test --filter "FullyQualifiedName~ContactFormTest"
```

Or press `F1` in VS Code and select **"Playwright: Show Inspector"** while the test is paused.

### Tracing

Playwright can record a full **trace** for every test — a timeline of screenshots, DOM snapshots, console logs, and network activity. Traces are replayable in the Playwright Trace Viewer.

#### Enable tracing in `BaseTest`

```csharp
// Fixtures/CustomFixture.cs
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

[TestFixture]
public class CustomFixture : PlaywrightTest
{
    [SetUp]
    public async Task SetUp()
    {
        // Tracing is automatically enabled by PlaywrightTest base class.
        // Each test gets a trace file at test-results/<test-name).zip
    }

    [TearDown]
    public async Task TearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            // Trace is automatically stopped and saved on failure by PlaywrightTest.
            var tracePath = $"test-results/{TestContext.CurrentContext.Test.Name}-trace.zip";
            await Page.Context.Tracing.StopAsync(new() { Path = tracePath });
        }
        else
        {
            await Page.Context.Tracing.StopAsync();
        }
    }
}
```

#### View a trace

```powershell
npx playwright show-trace test-results/ContactFormTest-trace.zip
```

### Screenshot on Failure

```csharp
[TearDown]
public async Task TearDown()
{
    if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
    {
        var screenshotPath = $"test-results/{TestContext.CurrentContext.Test.Name}-{DateTime.Now:yyyyMMdd_HHmmss}.png";
        await Page.ScreenshotAsync(new() { Path = screenshotPath });
        TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
    }
}
```

> **Note:** `PlaywrightTest` base class already captures a screenshot on failure by default — just enable it via `EmitTraceFile = true` or the `Screenshot` configuration option.

### Console and Network Logging

```csharp
// Log console messages
page.on('console', msg => Console.WriteLine(msg.text));

// Log network errors
page.on('pageerror', error => Console.WriteLine(error));
```

In C#:

```csharp
Page.Console += (sender, e) => Console.WriteLine($"[console] {e.Message.Text}");
Page.PageError += (sender, e) => Console.WriteLine($"[pageerror] {e.Exception.Message}");
```

### Playwright Config (optional)

You can create a `playwright.config.cs` for shared settings:

```csharp
// playwright.config.cs
using Microsoft.Playwright.NUnit;

public class PlaywrightConfig : PlaywrightTest
{
    public static PlaywrightTestOptions Options { get; } = new()
    {
        Headless = false,
        Use =
        {
            TraceDir = "test-results/",
            Screenshot = ScreenshotOption.On,
            VideoDir = "test-results/videos/",
            BypassCSP = true,
        },
    };
}
```

---

## Practice

1. Set up tracing with a screenshot on failure in your `BaseTest`.
2. Run a passing test — confirm a trace file is saved.
3. Run a failing test (e.g., wrong locator) — confirm a screenshot + trace zip drops into `test-results/`.
4. Open the trace with `npx playwright show-trace` and inspect the DOM snapshot at the point of failure.
5. Add console and pageerror logging to `BaseTest` so failures are traceable in the NUnit output.

---

## Skills to Load

- `playwright-csharp-setup` — PlaywrightTest base class, tracing, screenshot hooks, logging.

---

## Milestone

Any test in your suite that fails automatically drops a trace file, timestamped screenshot, and console/page error log into `test-results/`. You can open the trace in the Playwright Trace Viewer to inspect the DOM state at the point of failure.
