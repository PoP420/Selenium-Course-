# Phase 1 — Playwright vs. Selenium vs. Katalon

**Time:** 2–3 days

---

## Objectives

- Understand Playwright's architecture compared to Selenium and Katalon.
- Identify when Playwright is the right tool vs. alternatives.
- Write a comparison document you can reference in interviews.
- Know which questions to ask a client before recommending a framework.

---

## Topics

### Playwright Architecture

- **Built on CDP (Chrome DevTools Protocol)** — communicates directly with the browser via DevTools, not the legacy WebDriver JSONWire protocol.
- **Single install, multiple browsers** — `playwright install` downloads Chromium, Firefox, and WebKit. No separate driver binaries.
- **Language bindings** — C#, Java, Python, JavaScript/TypeScript.
- **Built-in test runner** — `playwright test` (TypeScript). For C#, use `Microsoft.Playwright.NUnit` which wraps Playwright in an NUnit-compatible base class.
- **Auto-waiting** — every action automatically waits for the element to be ready (visible, stable, enabled). No manual `WebDriverWait`.
- **Browser contexts** — isolated sessions within a single browser instance. Perfect for parallel tests without separate browser processes.
- **Tracing** — built-in timeline of everything that happened during a test: screenshots, DOM snapshots, console logs, network.

### Selenium

- **WebDriver protocol (W3C)** — language-agnostic protocol, browser-vendor implementations.
- **Selenium Manager** — auto-resolves drivers (Selenium 4.6+).
- **Language bindings** — Java, Python, C#, JavaScript, Ruby.
- **Grid** — distributed execution across browsers/machines.
- **Manual waits** — requires explicit `WebDriverWait` + `ExpectedConditions` or implicit waits. No auto-waiting.

### Katalon

- **Low-code IDE** — record and playback, built on Selenium/Appium.
- **Enterprise focus** — reporting, test management, CI plugins.
- **Less flexible** — harder to customize at the code level.

### Decision Matrix

| Factor | Selenium | Playwright | Katalon |
|---|---|---|---|
| Legacy browser support | Yes | No | Partial |
| Language flexibility | High | Moderate | Low |
| Auto-wait | No (manual) | Yes | Partial |
| CI/CD integration | Excellent | Excellent | Good |
| Enterprise reporting | DIY | DIY | Built-in |
| Learning curve | Steep | Moderate | Low |
| Community / jobs | Very high | Growing | Moderate |
| Tracing / Debugging | DIY | Built-in | Built-in |
| Visual testing | DIY (Sikuli, etc.) | Built-in | Built-in |
| API testing | Separate (RestAssured) | Built-in | Built-in |

---

## Key Architecture Differences for Interviews

1. **Auto-waiting** — Playwright waits for *any* action to succeed before proceeding. Selenium requires you to wait manually. This alone eliminates 70% of flaky tests.

2. **Locators** — Selenium uses `By.id("...")` and searches at interaction time. Playwright uses `Locator` objects (`page.Locator("...")`) which are lazy and retried automatically.

3. **Browser management** — Selenium delegates to WebDriver (one driver per browser). Playwright downloads and manages browser binaries + drivers in one step.

4. **Context isolation** — Playwright's `IBrowserContext` gives true incognito-level isolation without spawning new browser processes. Selenium needs separate driver instances.

5. **Tracing** — Playwright can record a full trace per test, replayable in a UI tool. Selenium requires third-party tools (Allure, ExtentReports).

---

## Practice

1. Write a **1-page comparison doc**: Playwright vs Selenium — architecture, waits, locators, browser management, debugging, when to pick each. Save it as `docs/framework-comparison.md` in your repo.
2. Practice the basics:

```csharp
using Microsoft.Playwright;

await using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync();
var page = await browser.NewPageAsync();
await page.GotoAsync("https://example.com");
Console.WriteLine(await page.TitleAsync());
await browser.CloseAsync();
```

Or with NUnit + `Microsoft.Playwright.NUnit`:

```csharp
[Test]
public async Task VerifyChromiumLaunches()
{
    await Page.GotoAsync("https://example.com");
    await Expect(Page).To_haveTitleAsync(new() { NameString = "Example Domain" });
}
```

3. Interview prep: practice explaining in 30 seconds why you chose Playwright for a project (modern SPAs, auto-waiting, built-in tracing, cross-browser parallelism).

---

## Skills to Load

- `playwright-csharp-setup` — for environment verification and Playwright browser installation.

---

## Milestone

Can explain, unprompted, why a team would pick Playwright over Selenium (modern web apps, auto-waiting, built-in debugging) and vice versa (legacy browsers, existing WebDriver infra). Have a written comparison doc saved.
