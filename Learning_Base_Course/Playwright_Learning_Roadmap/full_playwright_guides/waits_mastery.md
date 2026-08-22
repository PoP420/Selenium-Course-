# Waits & Auto-Waiting Mastery

The #1 cause of flaky tests is bad timing. In Playwright, auto-waiting eliminates 70% of timing issues automatically — but you still need to understand when and how to wait explicitly.

---

## Auto-Waiting — Playwright's Core Superpower

Playwright **automatically waits** before every action and assertion. You almost never need a manual wait.

```csharp
// All of these auto-wait:
await page.ClickAsync("#submit");            // waits until clickable
await page.FillAsync("#email", "test@x.com"); // waits until editable
await page.Locator(".alert").IsVisibleAsync(); // waits until stable
await Expect(locator).ToBeVisibleAsync();    // retries assertion until timeout
```

**Why it works:** Playwright doesn't just wait for the element to exist — it waits until the element is in a usable state (visible, enabled, not animating, stable).

---

## When Auto-Waiting Applies

| Action | What Playwright Waits For |
|---|---|
| `ClickAsync` | Element is visible, stable, not obscured, enabled |
| `FillAsync` | Element is editable (not disabled, visible) |
| `HoverAsync` | Element is visible, stable |
| `SelectOptionAsync` | The select element is visible, enabled |
| `CheckAsync` | Checkbox is visible, enabled, not already checked |
| `SetInputFilesAsync` | File input is visible, enabled |
| `keyboard.PressAsync` | Previous action on the element is complete |

## When It Does NOT Apply

- Reading a value without interacting (e.g., `InnerTextAsync()`, `GetAttributeAsync()`)
- Evaluating custom JavaScript
- `Locator.CountAsync()` — returns immediately
- `Locator.IsVisibleAsync()` — checks current state, doesn't wait for disappearance

For these cases, use `WaitForAsync` or `Expect`.

---

## Explicit Waits — `WaitForAsync`

Use `WaitForAsync` when you need to wait for a specific condition that isn't an action.

```csharp
using Microsoft.Playwright;

// Wait for element to be visible
await page.Locator(".spinner").WaitForAsync(
    new() { State = WaitForSelectorState.Hidden });

// Wait for element to appear in DOM
await page.Locator(".dynamic-content").WaitForAsync();

// Wait for text to appear
await page.Locator(".status").GetByText("Success").First
    .WaitForAsync(new() { State = WaitForSelectorState.Visible });
```

### WaitForSelectorState options

| State | When to Use |
|---|---|
| `Attached` | Element exists in DOM (may be hidden) |
| `Detached` | Element is no longer in DOM |
| `Visible` | Element is in DOM and visible (has size, not `display:none`) |
| `Hidden` | Element is gone or hidden |

---

## `Expect` Assertions — Auto-Retried

Playwright's `Expect` assertions retry until they pass or time out. This is often better than waiting for a locator to appear.

```csharp
using static Microsoft.Playwright.NUnit.Playwright;

// Wait for element AND verify it contains the right text
await Expect(page.Locator(".alert")).ToContainTextAsync("Success");

// Wait for URL to change
await Expect(page).To_haveURLAsync(new Uri("https://example.com/dashboard"));

// Wait for title
await Expect(page).To_haveTitleAsync(new() { NameString = "Dashboard" });

// Wait for element to be visible
await Expect(page.Locator("#submit")).ToBeVisibleAsync();

// Wait for element to be hidden
await Expect(page.Locator(".loading-spinner")).ToBeHiddenAsync();

// Wait for element to be enabled
await Expect(page.Locator("#submit")).ToBeEnabledAsync();
```

---

## Soft Assertions

By default, `Expect` assertions throw on failure and stop the test. You can set soft assertions to collect multiple failures:

```csharp
// Make Expect soft — doesn't throw, just records
Expect(page.Locator(".alert")).SetSoft();

// Now these won't throw immediately
await Expect(page.Locator(".alert")).ToContainTextAsync("Success");
await Expect(page.Locator(".badge")).ToBeVisibleAsync();
```

---

## Page Load Timing

### The problem

Single-page apps render content dynamically. `GotoAsync()` returns before the app finishes rendering.

### Solutions

1. **`Expect` an element to be visible** — most reliable:
   ```csharp
   await page.GotoAsync("https://practicesoftwaretesting.com");
   await Expect(page.Locator(".app-root")).ToBeVisibleAsync();
   ```

2. **Wait for a specific network request to complete**:
   ```csharp
   await page.GotoAsync("https://example.com", wait: WaitUntilState.NetworkIdle);
   ```

3. **Wait for URL to stabilize** — useful for route changes:
   ```csharp
   await Expect(page).To_haveURLAsync(new Uri("https://example.com/dashboard"));
   ```

---

## Common Wait Patterns

### Pattern 1: Wait for spinner to disappear, then interact

```csharp
await page.Locator(".loading-spinner").WaitForAsync(
    new() { State = WaitForSelectorState.Hidden });
await page.Locator("#submit").ClickAsync();
```

### Pattern 2: Wait for text to appear

```csharp
await page.GotoAsync("https://example.com");
await Expect(page.Locator(".status")).ToContainTextAsync("Loaded");
await page.Locator("#submit").ClickAsync();
```

### Pattern 3: Wait for URL change after navigation

```csharp
await page.Locator("a[href='/dashboard']").ClickAsync();
await Expect(page).To_haveURLAsync(new Uri("https://example.com/dashboard"));
```

### Pattern 4: Wait for API response

```csharp
var responseTask = page.WaitForResponseAsync(resp =>
    resp.Url.Contains("/api/data") && resp.Status == 200);
await page.Locator("#load-data").ClickAsync();
var response = await responseTask;
var json = await response.JsonAsync();
```

---

## Anti-Patterns

### `Task.Delay()` — **NEVER**

```csharp
// BAD: waits full 5 seconds even if element is ready
await Task.Delay(5000);

// GOOD: waits only as long as needed
await Expect(page.Locator(".alert")).ToBeVisibleAsync();
```

### Hardcoded timeouts everywhere

```csharp
// BAD: hardcoded 10 seconds everywhere
await page.Locator(".btn").ClickAsync(new() { Timeout = 10000 });

// GOOD: rely on default timeout (30s); use Expect for assertions
await Expect(page.Locator(".btn")).ToBeVisibleAsync();
```

### Mixing implicit and explicit waits

Playwright doesn't have implicit waits like Selenium. Auto-waiting is built-in and you don't need to configure anything.

---

## Best Practices Summary

1. **Trust auto-waiting** — let Playwright handle element readiness on actions.
2. **Use `Expect` for assertions** — they auto-retry and give the best failure diagnostics.
3. **Use `WaitForAsync` sparingly** — only when you need to wait without asserting.
4. **Avoid `Task.Delay()`** — it's always wrong.
5. **Wait for network idle on navigation-heavy pages** — `wait: WaitUntilState.NetworkIdle`.
6. **Wait for the right state** — `Visible` for interaction, `Hidden` for spinners, `Attached` for DOM presence.

---

## Interview Questions

1. "How does Playwright's auto-waiting differ from Selenium's explicit waits?"
2. "When would you use `WaitForAsync` vs `Expect`?"
3. "How do you handle a loading spinner that blocks interaction?"
4. "How do you wait for a specific API response?"
5. "What's the difference between `WaitForSelectorState.Visible` and `WaitForSelectorState.Attached`?"
6. "How do soft assertions differ from regular assertions in Playwright?"
