# Phase 2 — WebDriver Core: Locators, Actions, Waits

**Time:** 4–5 days

---

## Objectives

- Master Selenium locator strategies in C#.
- Use explicit waits instead of `Thread.Sleep()`.
- Perform common user interactions (clicks, form fills, dropdowns, hover, drag, alerts, tabs, iframes).
- Build a `Tests/` folder with standalone test classes for each interaction type.

---

## Topics

### Locators

- `By.Id`, `By.Name`, `By.CssSelector`, `By.XPath`, `By.LinkText`, `By.ClassName`.
- Priority order and when to use each.
- Writing both CSS and XPath for the same element.
- Locator hygiene: avoid absolute XPath, prefer relative selectors.

### Waits

- `WebDriverWait` + lambda conditions (explicit waits) — **Selenium 4 standard, no extra package required**.
- Common conditions: element visible, element clickable, text present, alert present.
- Fluent wait for custom polling / ignored exceptions.
- **Never use `Thread.Sleep()`** — build this discipline now.
- Avoid mixing implicit wait with explicit wait unless you fully understand the precedence rules.

### Actions API

- Clicks and form fills (`Clear()`, `SendKeys()`).
- Dropdowns (`SelectElement` class).
- `Actions` class for hover, drag-and-drop, key combos.
- Alert handling (`Accept()`, `Dismiss()`, `Text`).
- Window/tab switching (`WindowHandles`).
- Iframe switching (`SwitchTo().Frame()`, `SwitchTo().DefaultContent()`).

---

## Practice

Build 5–8 small test classes in `Tests/`. Each should use NUnit assertions.

**Important:** The course target site `practicesoftwaretesting.com` is a modern Angular SPA. Tests must:
- Run in **headed mode** (no `--headless=new`).
- Use **explicit waits** for every element interaction.
- Use a **temp user-data-dir** to avoid profile lock issues.

```csharp
// Tests/TestBase.cs
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Tests;

public class TestBase
{
    protected IWebDriver driver;
    protected WebDriverWait wait;

    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        driver = new ChromeDriver(options);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Dispose();
    }
}
```

```csharp
// Tests/ContactFormTest.cs
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

### Test classes to build

1. **Contact form submit** — fill + submit + assert success message
2. **Login form** — enter credentials, submit, assert redirect or error
3. **Dropdown selection** — select by value, index, visible text
4. **Hover menu** — hover over nav item, click submenu
5. **Drag and drop** — drag element to drop zone
6. **JavaScript alert** — trigger alert, accept/dismiss, assert text
7. **New tab** — open link in new tab, switch, assert content
8. **Iframe interaction** — switch to iframe, interact, switch back

---

## Skills to Load

- `selenium-locators-waits` — locators, waits, Actions API, alerts, iframes, window switching.

---

## Milestone

A `Tests/` folder with 5–8 test classes, each covering one interaction type. Every test uses NUnit assertions for pass/fail and explicit waits instead of `Thread.Sleep()`. All tests pass with `dotnet test`.
