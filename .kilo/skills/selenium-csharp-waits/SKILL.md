---
name: selenium-csharp-waits
description: Selenium WebDriver waits and locators in C# — By strategies, WebDriverWait with lambda conditions (no ExpectedConditions), SelectElement dropdowns, Actions API, alerts, windows, iframes, and shadow DOM. Use when writing C# Selenium interaction scripts, debugging flaky tests, or building the Phase 2 basics/ folder in C#.
---

# Selenium Locators, Actions, and Waits (C#)

## Purpose

This skill covers Phase 2 (WebDriver Core) of the C# Selenium Mastery plan. It teaches how to reliably find elements, interact with them, and handle timing issues without `Thread.Sleep()`. The key difference from Java is that C# uses Selenium 4 lambda-based waits instead of `ExpectedConditions`. Use it when writing low-level Selenium scripts in C#, debugging flaky tests, or translating Java interaction patterns into C#.

## When to Apply

- User asks how to find an element, write a locator, or choose between CSS and XPath in C#.
- User asks about waits, `WebDriverWait`, or lambda conditions in C#.
- User asks about clicks, form fills, dropdowns, `Actions` class, alerts, tabs, iframes in C#.
- User asks why a test is flaky or timing-dependent.
- User asks how to convert Java `ExpectedConditions` code to C#.

---

## 1. Locator Strategies

### Priority order

1. `By.Id` — fastest, most stable.
2. `By.Name` — stable if names are unique and semantic.
3. `By.CssSelector` — fast, readable, supports class/attribute/structural patterns.
4. `By.XPath` — most powerful, supports text/structure, but slower and more brittle.
5. `By.LinkText` / `By.PartialLinkText` — only for `<a>` tags.
6. `By.ClassName` — useful when class is unique; avoid if class is shared by many elements.

### Writing locators

```csharp
using OpenQA.Selenium;

driver.FindElement(By.Id("username"));
driver.FindElement(By.Name("email"));
driver.FindElement(By.CssSelector("button[type='submit']"));
driver.FindElement(By.XPath("//button[contains(text(), 'Submit')]"));
driver.FindElement(By.LinkText("Forgot password?"));
driver.FindElement(By.ClassName("btn-primary"));
```

### CSS vs XPath for the same element

```csharp
// Example: primary submit button
IWebElement cssButton = driver.FindElement(By.CssSelector("form > button.btn-primary"));
IWebElement xpathButton = driver.FindElement(By.XPath("//form/button[contains(@class, 'btn-primary')]"));
```

Practice writing both for every element in your Phase 2 scripts.

### Locator hygiene

- Avoid absolute XPath (`/html/body/div[2]/div[1]`) — it breaks on any DOM change.
- Prefer relative XPath (`//button[contains(text(), 'Submit')]`) or CSS selectors.
- Group locators as `private readonly By` fields at the top of page classes.

---

## 2. Waits

### Never use `Thread.Sleep()`

`Thread.Sleep()` pauses for a fixed duration regardless of whether the element is ready. It makes tests slow and flaky.

### Implicit wait (global, avoid mixing with explicit)

```csharp
driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
```

- Tells WebDriver to poll the DOM for up to 10 seconds when finding elements.
- **Do not mix** implicit and explicit waits — it causes unpredictable timeout math.

### Explicit wait with lambdas (recommended — Selenium 4 standard)

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

// Wait for element to exist in DOM
IWebElement element = wait.Until(d => d.FindElement(By.Id("email")));

// Wait for element to be visible
IWebElement visibleElement = wait.Until(d =>
{
    var el = d.FindElement(By.Id("email"));
    return el.Displayed ? el : null;
});

// Wait for element to be clickable
IWebElement clickableElement = wait.Until(d =>
{
    var el = d.FindElement(By.CssSelector("button[type='submit']"));
    return el.Enabled && el.Displayed ? el : null;
});
```

**Why lambdas?**
- `ExpectedConditions` was removed from Selenium 4 core in C#.
- `SeleniumExtras.WaitHelpers` is a legacy community package.
- Lambdas are type-safe, composable, and require no extra package.

### Common lambda wait patterns

| Pattern | Code |
|---|---|
| Element exists | `wait.Until(d => d.FindElement(By.Id("...")))` |
| Element visible | `wait.Until(d => { var el = d.FindElement(By.Id("...")); return el.Displayed ? el : null; })` |
| Element clickable | `wait.Until(d => { var el = d.FindElement(By.Id("...")); return el.Enabled && el.Displayed ? el : null; })` |
| Element gone | `wait.Until(d => d.FindElements(By.CssSelector(".spinner")).Count == 0)` |
| Text appears | `wait.Until(d => d.FindElement(By.Id("status")).Text.Contains("Success"))` |
| Alert present | `wait.Until(d => _ = d.SwitchTo().Alert())` |

### Fluent wait (advanced)

```csharp
using OpenQA.Selenium.Support.UI;

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

## 3. Actions API

### Clicks and form fills

```csharp
using OpenQA.Selenium;

IWebElement username = driver.FindElement(By.Id("username"));
username.Clear();
username.SendKeys("admin");

driver.FindElement(By.CssSelector("button[type='submit']")).Click();
```

- `Clear()` before `SendKeys()` to ensure no residual text.
- Use waits before clicking (see patterns above).

### Dropdowns

```csharp
using OpenQA.Selenium.Support.UI;

IWebElement dropdown = driver.FindElement(By.Id("country"));
SelectElement select = new SelectElement(dropdown);
select.SelectByText("United States");
// or
select.SelectByValue("US");
// or
select.SelectByIndex(2);
```

### Actions class (hover, drag, key combos)

```csharp
using OpenQA.Selenium.Interactions;

Actions actions = new Actions(driver);

// Hover
actions.MoveToElement(driver.FindElement(By.Id("menu"))).Perform();

// Drag and drop
IWebElement source = driver.FindElement(By.Id("draggable"));
IWebElement target = driver.FindElement(By.Id("droppable"));
actions.DragAndDrop(source, target).Perform();

// Key combos
actions.KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).Perform();
```

### Alerts

```csharp
using OpenQA.Selenium;

// Wait for alert
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
IAlert alert = wait.Until(d => _ = d.SwitchTo().Alert());

string alertText = alert.Text;
alert.Accept();      // OK
alert.Dismiss();     // Cancel
alert.SendKeys("some text"); // for prompts
alert.Accept();
```

### Window / tab switching

```csharp
string originalWindow = driver.CurrentWindowHandle;

// Click that opens a new tab/window
driver.FindElement(By.Id("open-new-tab")).Click();

var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
wait.Until(d => d.WindowHandles.Count > 1);

foreach (string windowHandle in driver.WindowHandles)
{
    if (!windowHandle.Equals(originalWindow))
    {
        driver.SwitchTo().Window(windowHandle);
        break;
    }
}

// Now interact with the new window
Console.WriteLine(driver.Title);

// Close and switch back
driver.Close();
driver.SwitchTo().Window(originalWindow);
```

### Iframe switching

```csharp
using OpenQA.Selenium;

// Switch to iframe by locator
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
wait.Until(d =>
{
    try
    {
        d.SwitchTo().Frame(By.Id("payment-frame"));
        return true;
    }
    catch (NoSuchFrameException)
    {
        return false;
    }
});

// Interact with elements inside iframe
driver.FindElement(By.Id("card-number")).SendKeys("4111111111111111");

// Switch back to main document
driver.SwitchTo().DefaultContent();
```

### Shadow DOM (Selenium 4+)

```csharp
// Find the shadow host
IWebElement shadowHost = driver.FindElement(By.CssSelector("my-custom-element"));

// Get the shadow root
ISearchContext shadowRoot = shadowHost.GetShadowRoot();

// Find elements inside shadow DOM
IWebElement innerButton = shadowRoot.FindElement(By.CssSelector(".inner-button"));
innerButton.Click();
```

---

## 4. Phase 2 Script Template

Each test class should follow this shape:

```csharp
// src/test/csharp/ContactFormTest.cs
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class ContactFormTest : TestBase
{
    [Test]
    public void TestContactFormSubmit()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/contact");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

        var firstName = wait.Until(d => d.FindElement(By.Id("first_name")));
        var lastName = wait.Until(d => d.FindElement(By.Id("last_name")));
        var email = wait.Until(d => d.FindElement(By.Id("email")));
        var subject = wait.Until(d => d.FindElement(By.Id("subject")));
        var message = wait.Until(d => d.FindElement(By.Id("message")));
        var btnSubmit = wait.Until(d =>
        {
            var el = d.FindElement(By.ClassName("btnSubmit"));
            return el.Enabled && el.Displayed ? el : null;
        });

        firstName.SendKeys("John");
        lastName.SendKeys("Smith");
        email.SendKeys("john.smith@example.com");

        var subjectSelect = new SelectElement(subject);
        subjectSelect.SelectByValue("webmaster");

        message.SendKeys("Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
        btnSubmit.Click();

        var alert = wait.Until(d => d.FindElement(By.ClassName("alert")));
        Assert.That(alert.Text.Trim(), Is.EqualTo("Thanks for your message! We will contact you shortly."));
    }
}
```

---

## 5. SPA / Angular / React Timing

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

## 6. Page Load Strategy

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

## 7. Common Pitfalls

| Pitfall | Fix |
|---|---|
| `Thread.Sleep()` used instead of waits | Replace with `WebDriverWait` + lambda. |
| `NoSuchElementException` after page loads | Wait for visibility or clickable before interacting. |
| Click intercepted / not clickable | Wait for `Enabled && Displayed`; ensure no overlay covers the element. |
| `StaleElementReferenceException` | Re-locate the element after a page update; don't cache elements across waits. |
| Switching to wrong iframe | Verify you're in the correct iframe before interacting; switch back with `DefaultContent()`. |
| Using `ExpectedConditions` in C# | Use Selenium 4 lambdas — `SeleniumExtras` is legacy. |

---

## 8. Milestone

A `src/test/csharp/` folder with 5–8 test classes, each covering one interaction type (login, form fill, dropdown, hover, drag, alert, window switch, iframe). Every test uses NUnit assertions for pass/fail and explicit waits (lambdas) instead of `Thread.Sleep()`.
