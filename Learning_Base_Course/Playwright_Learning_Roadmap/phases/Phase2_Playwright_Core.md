# Phase 2 — Playwright Core: Locators, Actions, Auto-Waiting

**Time:** 4–5 days

---

## Objectives

- Master Playwright locator strategies in C#.
- Understand and rely on auto-waiting — no manual waits needed.
- Perform common user interactions (clicks, fills, hovers, drag-and-drop, dropdowns, keyboard, file upload).
- Build a `Tests/` folder with standalone test classes for each interaction type.

---

## Topics

### Locators

Playwright uses **Locator** objects, not `By` finders. Locators are:

- **Lazy** — they don't touch the DOM until you perform an action or assertion.
- **Auto-retried** — Playwright re-queries the DOM if the element isn't ready yet.
- **First-class** — you can filter, chain, and assert on them directly.

#### Locator strategies

| Strategy | API | Use When |
|---|---|---|
| `page.Locator()` | `page.Locator("css=button.submit")` | CSS selector (default) |
| `page.GetByRole()` | `page.GetByRole("button", new() { Name = "Submit" })` | Accessibility-first — most resilient |
| `page.GetByTestId()` | `page.GetByTestId("submit-btn")` | `data-testid` / `data-test` attributes |
| `page.GetByText()` | `page.GetByText("Forgot password?")` | Text content matching |
| `page.GetByLabel()` | `page.GetByLabel("Email")` | Form label matching |
| `page.GetByPlaceholder()` | `page.GetByPlaceholder("Enter email")` | Placeholder attribute |
| `page.GetByAltText()` | `page.GetByAltText("Company logo")` | Image alt text |
| `page.GetByTitle()` | `page.GetByTitle("Help")` | Title attribute |
| `page.Locator("xpath=...")` | `page.Locator("xpath=//div[@class='card']")` | XPath (fallback) |

#### Locator chaining and filtering

```csharp
// Find the first button inside a specific card
var card = page.Locator(".card").First;
await card.Locator("button").ClickAsync();

// Filter by text
var buttons = page.Locator("button").FirstChild().Locator("text=Submit");

// nth element
await page.Locator("li.item").Nth(2).ClickAsync();
```

### Auto-Waiting

Playwright auto-waits on **every** action and assertion. You almost never need `WaitForSelectorAsync` or manual timeouts.

```csharp
// These auto-wait for the element to be ready:
await page.ClickAsync("#submit");          // waits until clickable
await page.FillAsync("#email", "test@x.com"); // waits until editable
await page.Locator(".alert").IsVisibleAsync(); // waits until stable
```

For cases where you need to wait for a specific condition:

```csharp
await page.Locator(".spinner").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
await page.Locator(".dynamic-content").ScrollIntoViewIfNeededAsync();
```

**Never use `Task.Delay()` — build this discipline now.**

### Actions

Playwright has high-level actions that auto-wait:

- `ClickAsync()` — click element (waits for clickable).
- `FillAsync()` — fill input (clears + types, waits for editable).
- `HoverAsync()` — hover over element.
- `DragAndDropAsync()` — drag and drop.
- `EvaluateAsync()` — run JavaScript.
- `ScrollAsync()` / `ScrollIntoViewIfNeededAsync()` — scrolling.

#### Dropdowns

```csharp
// Select by value
await page.SelectOptionAsync("select[name='country']", new() { Value = "us" });

// Select by label
await page.SelectOptionAsync("select[name='country']", new() { Label = "United States" });

// Select by index
await page.SelectOptionAsync("select[name='country']", new() { Index = 2 });
```

#### Keyboard

```csharp
await page.Keyboard.PressAsync("Control+A");
await page.Keyboard.TypeAsync("Hello World");
await page.Keyboard.PressAsync("Enter");
```

#### File upload

```csharp
await page.Locator("input[type='file']").SetInputFilesAsync("path/to/file.pdf");
```

### Frames and pop-ups

```csharp
// Frames
var frame = page.Frames.First(f => f.Name == "payment-frame");
await frame.FillAsync("#card-number", "4111111111111111");

// Pop-ups (new tab)
var popup = await page.Context.NewPageAsync();
await popup.GotoAsync("https://example.com");
```

---

## Practice

Build 5–8 small test classes in `Tests/`. Each should extend `PlaywrightTest` and use Playwright NUnit fixtures.

**Important:** The course target site `practicesoftwaretesting.com` is a modern Angular SPA. Playwright handles this better than Selenium because of auto-waiting, but you should still:

- Run in **headed mode** (`Headless = false`) for debugging.
- Use **browser contexts** for isolation.
- Use **`page.Context.Tracing`** for debugging.

```csharp
// Tests/ContactFormTest.cs
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class ContactFormTest : PlaywrightTest
{
    [Test]
    public async Task TestContactFormSubmit()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/contact");

        await Page.Locator("#first_name").FillAsync("John");
        await Page.Locator("#last_name").FillAsync("Smith");
        await Page.Locator("#email").FillAsync("john.smith@example.com");
        await Page.SelectOptionAsync("#subject", new() { Value = "webmaster" });
        await Page.Locator("#message").FillAsync(
            "Hello My name is John Smith, Please make sure we have got 50 characters.");
        await Page.Locator(".btnSubmit").ClickAsync();

        await Expect(Page.Locator(".alert")).ToContainTextAsync(
            "Thanks for your message! We will contact you shortly.");
    }
}
```

### Test classes to build

1. **Contact form submit** — fill + submit + assert success message
2. **Login form** — enter credentials, submit, assert redirect or error
3. **Dropdown selection** — select by value, index, label
4. **Hover menu** — hover over nav item, click submenu
5. **Drag and drop** — drag element to drop zone
6. **JavaScript dialog** — trigger dialog, accept/dismiss
7. **New tab** — open link in new tab, assert content
8. **File upload** — upload a file via input

---

## Skills to Load

- `playwright-csharp-locators-waits` — locators, auto-waiting, actions, frames, file upload.

---

## Milestone

A `Tests/` folder with 5–8 test classes, each covering one interaction type. Every test uses `Expect()` assertions for pass/fail and auto-waiting instead of `Task.Delay()`. All tests pass with `dotnet test`.
