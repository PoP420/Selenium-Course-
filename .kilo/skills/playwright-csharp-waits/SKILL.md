---
name: playwright-csharp-waits
description: Playwright locators, auto-waiting, and Expect assertions in C# — ILocator strategies, WaitForSelectorState, strict mode, GetByRole/GetByLabel/GetByTestId, Expect() assertions with regex/strings, and SPA timing solutions. Use when writing C# Playwright interaction scripts, debugging locator issues, or building Phase 2 basics/ folder in C#.
---

# Playwright Locators, Waits, and Assertions (C#)

## Purpose

This skill covers Phase 2 (WebDriver Core) of the C# Playwright Mastery plan. It teaches how to reliably find elements using `ILocator`, handle timing in SPAs, and write assertions using Playwright's `Expect` pattern. Use it when writing low-level Playwright scripts in C#, debugging flaky tests, or translating Selenium patterns into Playwright C#.

## When to Apply

- User asks how to find an element, write a locator, or choose between locator strategies in C# Playwright.
- User asks about waits, `WaitForSelectorState`, or auto-waiting in Playwright.
- User asks about clicks, form fills, dropdowns in C# Playwright.
- User asks about `Expect()`, assertions, or strict mode violations.
- User asks why a test is flaky or elements aren't found in an Angular/React SPA.

---

## 1. Locator Strategies

### Locator hierarchy (prefer this order)

1. **`Page.GetByText(string)`** — matches visible text. Most readable.
2. **`Page.GetByRole(AriaRole, options)`** — matches ARIA roles. Most accessible, resilient to DOM changes.
3. **`Page.GetByLabel(string)`** — matches form labels and aria-labels.
4. **`Page.GetByTestId(string)`** — matches `data-testid` attributes (configurable).
5. **`Page.Locator(selector)`** — raw CSS or text selectors.
6. **`Page.Locator(selector, options)`** — CSS with Playwright pseudo-selectors (`:has()`, `:near()`, etc.).

### Text-based locators

```csharp
// Exact text match (case-insensitive by default)
await Page.GetByText("Submit").ClickAsync();

// Partial text match
await Page.GetByText("Save and continue").ClickAsync();

// Text with custom options
await Page.GetByText("Sign in", new() { Exact = true }).ClickAsync();

// Text filter on an existing locator
var form = Page.Locator("form");
await form.GetByText("Login").ClickAsync();
```

### Role-based locators

```csharp
// By ARIA role
await Page.GetByRole(AriaRole.Button, new() { NameString = "Submit" }).ClickAsync();
await Page.GetByRole(AriaRole.Heading, new() { NameString = "Welcome" }).ToBeVisibleAsync();
await Page.GetByRole(AriaRole.Link, new() { NameString = "Forgot password?" }).ClickAsync();

// By role with approximate match
await Page.GetByRole(AriaRole.Button, new() { NameString = "Submit", Exact = false })
    .ClickAsync();
```

### Label and test ID locators

```csharp
// By label text or aria-label
await Page.GetByLabel("Email address").FillAsync("user@example.com");
await Page.GetByLabel("Password", new() { Exact = false }).FillAsync("password123");

// By data-testid (requires configuration; default attribute is "data-testid")
await Page.GetByTestId("login-submit").ClickAsync();

// By CSS selector with data attributes (Playwright C# uses attribute selectors)
await Page.Locator("input[data-test='login-submit']").ClickAsync();
```

### CSS selectors

```csharp
using Microsoft.Playwright;

// ID
ILocator emailInput = Page.Locator("#email");

// Class
ILocator submitBtn = Page.Locator(".btnSubmit");

// Attribute
ILocator loginBtn = Page.Locator("[data-test='login-submit']");

// Combined
ILocator productCard = Page.Locator("a.card[data-test^='product-']");
```

### XPath (use sparingly)

```csharp
// Relative XPath — prefer over absolute
ILocator submitBtn = Page.Locator("//form//input[@type='submit']");

// XPath with text
ILocator link = Page.Locator("//a[contains(text(), 'Contact')]");
```

---

## 2. Auto-Waiting

Playwright's greatest strength is **auto-waiting**. Every action on a `Locator` (`ClickAsync`, `FillAsync`, `GetByText`, etc.) automatically waits for the element to be in the right state before performing the action.

```csharp
// These all implicitly wait:
await Page.Locator("#email").FillAsync("test@example.com");     // waits until visible + editable
await Page.Locator("[data-test='login-submit']").ClickAsync();  // waits until visible + stable + enabled
await Page.GetByRole(AriaRole.Button, new() { NameString = "Submit" }).ClickAsync();
```

No `Thread.Sleep()`, no `WebDriverWait`, no `ExpectedConditions`.

### When auto-waiting doesn't apply

Auto-waiting does **not** apply to:
- `Locator.CountAsync()` — returns immediately
- `Locator.InnerTextAsync()` — returns immediately if element exists
- `Locator.IsVisibleAsync()` — returns immediately

If you need to wait, use explicit waits:

---

## 3. Explicit Waits

### WaitForSelectorState — wait for element state

```csharp
using Microsoft.Playwright;

// Wait for element to be in DOM (attached)
await Page.Locator(".spiner").WaitForAsync(new() { State = WaitForSelectorState.Attached });

// Wait for element to be visible
await Page.Locator("a.card[data-test^='product-']").First
    .WaitForAsync(new() { State = WaitForSelectorState.Visible });

// Wait for element to be hidden/detached
await Page.Locator(".modal").WaitForAsync(new() { State = WaitForSelectorState.Hidden });

// Wait with custom timeout
await Page.Locator("#dynamic-element").WaitForAsync(
    new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
```

### WaitForLoadState — wait for page to reach a load state

```csharp
using Microsoft.Playwright;

// Wait for DOM content loaded
await Page.GotoAsync("https://example.com");
await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

// Wait for network to be idle (no requests for 500ms)
await Page.GotoAsync("https://example.com", new() { WaitUntil = WaitUntilState.NetworkIdle });

// Wait for load event (default)
await Page.WaitForLoadStateAsync(LoadState.Load);
```

> **Warning**: Some sites never reach `NetworkIdle` (analytics, websockets, polling). Use element-based waits as the primary strategy and `NetworkIdle` only as a fallback.

### SPA / Angular timing

Single-page apps render content dynamically after the initial DOM load. `Page.GotoAsync()` returns on the `load` event, but Angular may still be bootstrapping components.

**Pattern: navigate, then wait for a specific component to render:**

```csharp
public async Task NavigateAsync()
{
    await Page.GotoAsync("https://practicesoftwaretesting.com/");
    // Wait for product cards (Angular renders these after initial load)
    await Page.Locator("a.card[data-test^='product-']").First
        .WaitForAsync(new() { State = WaitForSelectorState.Visible });
}
```

This is more reliable than `NetworkIdle` and fails fast if the component doesn't render.

---

## 4. Expect Assertions

Playwright's `Expect` API provides fluent, auto-waiting assertions:

### Page assertions

```csharp
using Microsoft.Playwright;

// Title — exact match
await Expect(Page).ToHaveTitleAsync("Example Domain");

// Title — regex match (partial)
await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));

// URL — exact match
await Expect(Page).ToHaveURLAsync("https://example.com/");

// URL — regex
await Expect(Page).ToHaveURLAsync(new Regex(".*/product/.*"));
```

### Locator assertions

```csharp
// Visibility
await Expect(Page.Locator("#submit-btn")).ToBeVisibleAsync();
await Expect(Page.Locator(".hidden")).ToBeHiddenAsync();

// Text content
await Expect(Page.Locator("h1")).ToContainTextAsync("Welcome");
await Expect(Page.Locator("h1")).ToHaveTextAsync("Welcome");

// Attribute
await Expect(Page.Locator("#email")).ToHaveAttributeAsync("type", "email");
await Expect(Page.Locator("input")).ToHaveAttributeAsync("data-test", new Regex("login-.*"));

// Value
await Expect(Page.Locator("#email")).ToHaveValueAsync("test@example.com");

// Enabled/disabled
await Expect(Page.Locator("button[type='submit']")).ToBeEnabledAsync();
await Expect(Page.Locator("button[type='submit']")).ToBeDisabledAsync();

// Count
await Expect(Page.Locator("a.card")).ToHaveCountAsync(9);
```

### Assertion with custom timeout

```csharp
await Expect(Page.Locator("#slow-element")).ToBeVisibleAsync(new() { Timeout = 10000 });
```

---

## 5. Strict Mode

By default, Playwright runs in **strict mode**. This means `Locator` methods that expect a single element will throw if multiple elements match:

```csharp
// If two <p> elements exist → Strict mode violation
await Expect(Page.Locator("p")).ToContainTextAsync("Hello");
```

**Fix — narrow the selector:**

```csharp
// Filter by text content
await Expect(Page.Locator("p", new() { HasText = "This domain is for use in" }))
    .ToContainTextAsync("documentation examples");

// Or use First/Last
await Expect(Page.Locator("p").First).ToContainTextAsync("documentation");

// Or use GetByText
await Expect(Page.GetByText("This domain is for use in documentation"))
    .ToBeVisibleAsync();
```

### Disabling strict mode (not recommended)

```csharp
await Page.Locator("p").First.ToContainTextAsync("...");  // OK
// Or use non-strict locator by adding .Locator("p").First
```

---

## 6. SelectElement / Dropdowns

Playwright handles dropdowns via `ILocator.SelectOptionAsync()`:

```csharp
// By value
await Page.Locator("#subject").SelectOptionAsync("webmaster");

// By label (visible text)
await Page.Locator("#country").SelectOptionAsync("United States");

// By value with value object
await Page.Locator("#subject").SelectOptionAsync(new() { Value = "webmaster" });

// By label with value object
await Page.Locator("#country").SelectOptionAsync(new() { Label = "Philippines" });
```

No `SelectElement` wrapper class needed — Playwright's API is built into `ILocator`.

---

## 7. Action Methods

Playwright's `ILocator` provides action methods that auto-wait:

```csharp
// Fill text (clears + types)
await Page.Locator("#email").FillAsync("user@example.com");

// Type character by character
await Page.Locator("#email").TypeAsync("user@example.com");

// Click
await Page.Locator("button[type='submit']").ClickAsync();

// Press keyboard key
await Page.Locator("#password").PressAsync("Enter");

// Select option in dropdown
await Page.Locator("#country").SelectOptionAsync("US");

// Check/uncheck checkbox
await Page.Locator("#terms").CheckAsync();
await Page.Locator("#newsletter").UncheckAsync();

// Hover
await Page.Locator(".menu-item").HoverAsync();
```

---

## 8. Common Pitfalls

| Pitfall | Fix |
|---|---|
| `thread.sleep` or `Thread.Sleep()` | Use `Locator.WaitForAsync()`, `Expect().ToBeVisibleAsync()`, or rely on auto-waiting. |
| Locator resolves to 0 elements on SPA | Wait for a specific element to render after `GotoAsync`, not just for page load. |
| Strict mode violation (multiple elements) | Use `.First`, `.Last`, `GetByText(...)`, or filter with `HasText`. |
| `GetByRole(AriaRole.Button, ...)` finds `input[type=submit]` | It does! Playwright maps `input[type=submit]` to ARIA role `button`. `NameString` matches `value` or `aria-label`. |
| `NetworkIdle` waits forever | Sites with websockets/analytics never idle. Use element-based waits instead. |
| `Locator.CountAsync()` returns 0 | Call `Locator.First.WaitForAsync()` before counting, or use `Expect(locator).ToHaveCountAsync(N)`. |
| Clicking before page renders | Playwright auto-waits for clickability, but if the element doesn't exist in DOM yet, it throws `TimeoutError`. Wait for the element to exist first. |

---

## 9. Milestone

A `src/test/csharp/` folder with 5+ test classes covering:
- Element locators (text, role, label, CSS, attribute)
- Click, fill, type, select actions
- Auto-waiting vs explicit waits (`WaitForAsync`, `WaitForLoadStateAsync`)
- `Expect` assertions (title, URL, visibility, text, attribute, count)
- SPA timing patterns (wait for Angular components)
- Strict mode handling

All tests use Playwright's auto-waiting and `Expect` assertions instead of `Thread.Sleep`.
