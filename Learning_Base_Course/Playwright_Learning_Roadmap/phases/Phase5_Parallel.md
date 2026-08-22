# Phase 5 — Parallel & Cross-Browser

**Time:** 4–5 days

---

## Objectives

- Speed up the test suite with NUnit parallel execution.
- Run tests across browsers (Chromium, Firefox, WebKit) via Playwright.
- Understand Playwright's browser context isolation for parallel safety.
- Measure and document speedup.

---

## Topics

### Parallel Execution with NUnit

Playwright's `Microsoft.Playwright.NUnit` is designed to be parallel-safe. Each test gets its own browser context, so parallel execution works out of the box.

Enable parallelization in your test project:

```xml
<!-- PlaywrightCSharpTests.Tests.csproj -->
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.0" />
  <PackageReference Include="NUnit" Version="4.3.2" />
  <PackageReference Include="NUnit3TestAdapter" Version="5.0.0" />
  <PackageReference Include="Microsoft.Playwright.NUnit" Version="1.49.0" />
</ItemGroup>
```

Add parallel config in `SetUp` or via project-level attribute:

```csharp
using NUnit.Framework;
using Microsoft.Playwright.NUnit;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class LoginTests : PlaywrightTest
{
    [Test]
    public async Task TestLoginFlow() { /* ... */ }
}
```

Run with:

```powershell
dotnet test
```

### Cross-Browser Testing

Override `BrowserType` to run the same suite across Chromium, Firefox, and WebKit.

```csharp
// Fixtures/BrowserFixture.cs
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Collections;

namespace PlaywrightCSharpTests.Tests;

public class BrowserFixture : PlaywrightTest
{
    [Test]
    [TestCase((string)"chromium")]
    [TestCase((string)"firefox")]
    [TestCase((string)"webkit")]
    public async Task TestLoginCrossBrowser(string browserName)
    {
        // BrowserType is set in the base fixture; override per test:
        // This is handled at the fixture level via BrowserType
    }
}
```

### Browser Context Isolation

Playwright creates a new browser context per test — this is why parallel execution is safe without shared state concerns:

```csharp
// Each test gets its own context automatically via PlaywrightTest
await Page.GotoAsync("https://practicesoftwaretesting.com");
// Page is isolated to this test's browser context
```

### Playwright Configuration

You can control browser selection via `playwright.config.cs` or via NUnit config. By default, `PlaywrightTest` uses Chromium. Override `BrowserType` via the `PlaywrightTest` property:

```csharp
[TestFixture]
public class MyTests : PlaywrightTest
{
    public MyTests()
    {
        BrowserType = "firefox";  // default is "chromium"
    }
}
```

Or use a custom factory pattern with `TestCaseSource`:

```csharp
[Test]
[TestCaseSource(nameof(Browsers))]
public async Task TestLogin(string browser)
{
    // PlaywrightTest sets up the browser based on BrowserType config
}

private static IEnumerable<string> Browsers()
{
    yield return "chromium";
    yield return "firefox";
    yield return "webkit";
}
```

---

## Practice

1. Run your Phase 4 suite sequentially and record the time.
2. Enable NUnit parallel execution and record the speedup.
3. Create a cross-browser test fixture that runs the same tests on Chromium, Firefox, and WebKit.
4. Run the same suite across all three browsers and verify all pass.

```csharp
// Tests/CrossBrowserTest.cs
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class CrossBrowserTest : PlaywrightTest
{
    [Test]
    [TestCase("chromium")]
    [TestCase("firefox")]
    [TestCase("webkit")]
    public async Task TestLoginPageLoads(string browserName)
    {
        BrowserType = browserName;
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/login");
        await Expect(Page).To_haveTitleAsync(new() { NameString = "Practice Software Testing" });
    }
}
```

---

## Skills to Load

- `playwright-csharp-parallel` — NUnit parallel config, Playwright browser contexts, cross-browser testing.

---

## Milestone

| Configuration | Time |
|---|---|
| Sequential, local (Chromium) | ___s |
| Parallel (NUnit), local (Chromium) | ___s |
| Parallel, cross-browser (Chromium + Firefox + WebKit) | ___s |

Document the before/after execution time in a table. Show the speedup percentage. Tests pass on all three browser engines.
