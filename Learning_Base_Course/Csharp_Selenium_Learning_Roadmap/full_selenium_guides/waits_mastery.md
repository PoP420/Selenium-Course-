# Waits & Synchronization Mastery

The #1 cause of flaky Selenium tests is bad timing. Master waits and your tests will be reliable enough to run in CI without random failures.

---

## The Three Wait Types

### 1. Implicit Wait — **Avoid**

```csharp
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
```

- Tells WebDriver to poll the DOM for up to N seconds before throwing `NoSuchElementException`.
- **Global** — applies to every `FindElement` call.
- **Dangerous** when mixed with explicit waits (they multiply!).
- **Legacy pattern.** Use explicit waits instead.

### 2. Explicit Wait — **Use this**

```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
IWebElement element = wait.Until(d => d.FindElement(By.Id("email")));
```

- Waits for a specific condition on a specific element.
- **Local scope** — only affects the element you're waiting for.
- Preferred for all dynamic content.
- **Selenium 4 standard** — uses lambda expressions, no `SeleniumExtras` package required.

### 3. Fluent Wait — **For advanced cases**

```csharp
var wait = new DefaultWait<IWebDriver>(driver)
{
    Timeout = TimeSpan.FromSeconds(30),
    PollingInterval = TimeSpan.FromMilliseconds(500)
};
wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

IWebElement element = wait.Until(d =>
{
    return d.FindElement(By.Id("email"));
});
```

- Custom polling interval.
- Can ignore specific exceptions.
- Use when elements appear in batches or with unusual timing.

---

## Explicit Wait Patterns (Selenium 4)

### Element State Conditions

| Condition | Lambda Pattern | Use When |
|---|---|---|
| Element exists in DOM | `wait.Until(d => d.FindElement(By.Id("...")))` | Element is present (may be hidden) |
| Element is visible | `wait.Until(d => { var el = d.FindElement(By.Id("...")); return el.Displayed ? el : null; })` | Element has size and is not `display:none` |
| Element is clickable | `wait.Until(d => { var el = d.FindElement(By.Id("...")); return el.Enabled && el.Displayed ? el : null; })` | Safe to click |
| Element is gone | `wait.Until(d => d.FindElements(By.CssSelector(".spinner")).Count == 0)` | Loading spinner disappeared |
| Text appears | `wait.Until(d => d.FindElement(By.Id("status")).Text.Contains("Success"))` | Text is present in element |
| Alert present | `wait.Until(d => _ = _driver.SwitchTo().Alert())` | JS alert/confirm/prompt is open |

---

## Common Wait Patterns

### Pattern 1: Wait for element, then interact

```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
IWebElement email = wait.Until(d => d.FindElement(By.Id("email")));
email.SendKeys("test@example.com");
```

### Pattern 2: Wait for clickable, then click

```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
IWebElement submit = wait.Until(d =>
{
    var el = d.FindElement(By.CssSelector(".btn-submit"));
    return el.Enabled && el.Displayed ? el : null;
});
submit.Click();
```

### Pattern 3: Wait for text to appear

```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
wait.Until(d => d.FindElement(By.Id("status")).Text.Contains("Success"));
```

### Pattern 4: Wait for element to disappear (spinner)

```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
wait.Until(d => d.FindElements(By.CssSelector(".spinner")).Count == 0);
```

### Pattern 5: Wait for page load (URL change)

```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
wait.Until(d => d.Url.Contains("/dashboard"));
```

---

## StaleElementReferenceException

### What causes it

The element you found was valid, but the DOM updated (page reload, AJAX, React/Vue re-render) and the reference is now stale.

### Fix

```csharp
// BAD: element goes stale after page update
IWebElement button = driver.FindElement(By.Id("submit"));
button.Click(); // may throw StaleElementReferenceException

// GOOD: re-find the element after each DOM change
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
IWebElement button = wait.Until(d =>
{
    var el = d.FindElement(By.Id("submit"));
    return el.Enabled && el.Displayed ? el : null;
});
button.Click();
```

---

## SPA / Angular / React Timing

### The problem

Single-page apps load content dynamically. `driver.Navigate().GoToUrl()` returns before the app finishes rendering.

### Solutions

1. **Wait for a specific element** — most reliable
    ```csharp
    wait.Until(d => d.FindElement(By.CssSelector(".app-root")).Displayed);
    ```

2. **Wait for URL to stabilize** — useful for route changes
    ```csharp
    wait.Until(d => d.Url.Contains("/dashboard"));
    ```

3. **Wait for JS ready state** — last resort
    ```csharp
    new WebDriverWait(driver, TimeSpan.FromSeconds(30)).Until(
        d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete")
    );
    ```

---

## Page Load Strategy

```csharp
var options = new ChromeOptions();
options.PageLoadStrategy = PageLoadStrategy.Eager; // DOMContentLoaded
// options.PageLoadStrategy = PageLoadStrategy.Normal; // full load (default)
// options.PageLoadStrategy = PageLoadStrategy.None; // return immediately
```

| Strategy | When to Use |
|---|---|
| `Normal` | Default. Wait for full page load. |
| `Eager` | SPAs where DOMContentLoaded is enough. Faster. |
| `None` | You'll handle all waits manually. Risky. |

---

## Anti-Patterns

### Thread.Sleep() — **NEVER**

```csharp
// BAD: waits full 5 seconds even if element is ready
Thread.Sleep(5000);

// GOOD: waits only as long as needed
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
wait.Until(d => d.FindElement(By.Id("email")).Displayed);
```

### Mixing Implicit + Explicit Waits

```csharp
// BAD: implicit wait + explicit wait = unpredictable total timeout
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
wait.Until(...); // can take up to 25 seconds!

// GOOD: explicit wait only
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
wait.Until(d => d.FindElement(By.Id("email")).Displayed);
```

---

## Best Practices Summary

1. **Explicit waits only** — never mix with implicit waits.
2. **Wait for the right condition** — visibility for interaction, presence for DOM check, invisibility for spinners.
3. **Wait before interaction** — never `FindElement` then `SendKeys` without a wait.
4. **Centralize timeouts** — one constant in `BasePage`.
5. **Use Page Object methods** — hide waits inside POM actions.
6. **Handle stale elements** — re-find after DOM changes.
7. **Prefer lambdas over `ExpectedConditions`** — `SeleniumExtras` is a legacy package; Selenium 4 `WebDriverWait` + lambdas is the modern standard.

---

## Interview Questions

1. "What's the difference between implicit and explicit waits? Why shouldn't you use both?"
2. "How do you handle a loading spinner that blocks interaction?"
3. "What causes StaleElementReferenceException and how do you fix it?"
4. "How do you wait for an element that's rendered by JavaScript after page load?"
5. "What's the page load strategy, and when would you use EAGER vs NORMAL?"
6. "Why did Selenium 4 remove `ExpectedConditions` from the core package?"
