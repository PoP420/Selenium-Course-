---
name: selenium-csharp-pom
description: Page Object Model (POM) patterns for Selenium in C# — page classes with locators as fields, action methods using lambda waits, no assertions inside page objects, and optional SpecFlow BDD layer. Use when building or refactoring a C# Selenium test framework, creating BasePage, or adding Gherkin feature files with SpecFlow.
---

# Selenium Page Object Model (POM) — C#

## Purpose

This skill teaches how to structure a maintainable Selenium framework using POM in C#. It covers page classes in `src/main/csharp`, test classes in `src/test/csharp`, shared `BasePage` helpers, and the Selenium 4 lambda-based wait pattern. Use it when building or refactoring a C# Selenium suite, designing page classes, or integrating BDD.

## When to Apply

- User asks how to structure a C# Selenium test framework or refactor into POM.
- User asks about `BasePage`, page classes, or where assertions belong.
- User asks about SpecFlow, `.feature` files, or C# step definitions.
- User asks how to avoid duplicated locators or wait logic across tests.
- User asks how to convert Java POM code to C#.

---

## 1. POM Principles

- **One class per page/screen.**
- **Locators as `By` class fields.**
- **Actions as methods** (e.g., `FillForm(...)`, `SubmitForm()`).
- **No assertions inside page objects.** Assertions stay in test classes.
- **Inheritance from `BasePage`** for shared waits/helpers (optional; page objects can also own their own `WebDriverWait`).

### Why this matters

- Locators change in one place, not scattered across tests.
- Business logic reads like English in the test class.
- Page classes become reusable across multiple test suites.

---

## 2. Project Structure

```
SeleniumCSharpTests/
  src/
    main/
      csharp/
        Pages/
          BasePage.cs
          ContactPage.cs
          LoginPage.cs
          RegisterPage.cs
        Utils/
          TestLogger.cs
    test/
      csharp/
        TestBase.cs
        ContactFormTest.cs
        LoginTest.cs
        RegisterTest.cs
```

---

## 3. BasePage Class

```csharp
// src/main/csharp/Pages/BasePage.cs
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class BasePage
{
    protected readonly IWebDriver driver;
    protected readonly WebDriverWait wait;
    private const int DefaultTimeout = 10;

    public BasePage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeout));
    }

    protected IWebElement Find(By locator)
    {
        return wait.Until(d => d.FindElement(locator));
    }

    protected void Click(By locator)
    {
        wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    protected void TypeText(By locator, string text)
    {
        var element = wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el.Displayed ? el : null;
        });
        element.Clear();
        element.SendKeys(text);
    }

    protected string GetText(By locator)
    {
        return wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el.Displayed ? el.Text : null;
        });
    }
}
```

**Key notes:**
- Uses Selenium 4 lambda waits — no `SeleniumExtras` package required.
- `Click` waits for both `Enabled` and `Displayed`.
- `TypeText` waits for `Displayed` before clearing and typing.
- `GetText` returns the text once the element is visible.

---

## 4. Page Class Example

```csharp
// src/main/csharp/Pages/ContactPage.cs
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class ContactPage : BasePage
{
    private readonly By firstNameLoc = By.Id("first_name");
    private readonly By lastNameLoc = By.Id("last_name");
    private readonly By emailLoc = By.Id("email");
    private readonly By subjectLoc = By.Id("subject");
    private readonly By messageLoc = By.Id("message");
    private readonly By btnSubmitLoc = By.ClassName("btnSubmit");
    private readonly By alertLoc = By.ClassName("alert");

    public ContactPage(IWebDriver driver) : base(driver) { }

    public void FillForm(string firstName, string lastName, string email, string message)
    {
        TypeText(firstNameLoc, firstName);
        TypeText(lastNameLoc, lastName);
        TypeText(emailLoc, email);

        var subjectElement = Find(subjectLoc);
        var subjectSelect = new SelectElement(subjectElement);
        subjectSelect.SelectByValue("webmaster");

        TypeText(messageLoc, message);
    }

    public void SubmitForm()
    {
        Click(btnSubmitLoc);
    }

    public string GetAlertText()
    {
        return GetText(alertLoc);
    }
}
```

```csharp
// src/main/csharp/Pages/LoginPage.cs
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class LoginPage : BasePage
{
    private readonly By emailLoc = By.Id("email");
    private readonly By passwordLoc = By.Id("password");
    private readonly By btnLoginLoc = By.CssSelector("[data-test='login-submit']");

    public LoginPage(IWebDriver driver) : base(driver) { }

    public void Login(string email, string password)
    {
        TypeText(emailLoc, email);
        TypeText(passwordLoc, password);
        Click(btnLoginLoc);
    }
}
```

```csharp
// src/main/csharp/Pages/RegisterPage.cs
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class RegisterPage : BasePage
{
    private readonly By firstNameLoc = By.Id("first_name");
    private readonly By lastNameLoc = By.Id("last_name");
    private readonly By birthDateLoc = By.Id("dob");
    private readonly By countryLoc = By.CssSelector("[data-test='country']");
    private readonly By postalCodeLoc = By.Id("postal_code");
    private readonly By houseNumberLoc = By.Id("house_number");
    private readonly By streetLoc = By.Id("street");
    private readonly By cityLoc = By.Id("city");
    private readonly By stateLoc = By.Id("state");
    private readonly By phoneLoc = By.Id("phone");
    private readonly By emailLoc = By.Id("email");
    private readonly By passwordLoc = By.Id("password");
    private readonly By btnSubmitLoc = By.ClassName("btnSubmit");

    public RegisterPage(IWebDriver driver) : base(driver) { }

    public void Register(string firstName, string lastName, string birthDate, string postalCode,
        string houseNumber, string street, string city, string state, string phone, string email, string password)
    {
        TypeText(firstNameLoc, firstName);
        TypeText(lastNameLoc, lastName);
        TypeText(birthDateLoc, birthDate);

        var countryElement = Find(countryLoc);
        var countrySelect = new SelectElement(countryElement);
        countrySelect.SelectByValue("PH");

        TypeText(postalCodeLoc, postalCode);
        TypeText(houseNumberLoc, houseNumber);
        TypeText(streetLoc, street);
        TypeText(cityLoc, city);
        TypeText(stateLoc, state);
        TypeText(phoneLoc, phone);
        TypeText(emailLoc, email);
        TypeText(passwordLoc, password);
    }

    public void SubmitForm()
    {
        Click(btnSubmitLoc);
    }
}
```

---

## 5. Test Class (Assertions Here)

```csharp
// src/test/csharp/ContactFormTest.cs
using NUnit.Framework;
using SeleniumCSharpTests.Pages;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class ContactFormTest : TestBase
{
    [Test]
    public void TestContactFormSubmit()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/contact");

        var contactPage = new ContactPage(driver);

        contactPage.FillForm("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
        contactPage.SubmitForm();

        Assert.That(contactPage.GetAlertText().Trim(),
            Is.EqualTo("Thanks for your message! We will contact you shortly."));
    }
}
```

**Key notes:**
- Tests extend `TestBase` for `driver` and `wait`.
- Tests instantiate page objects, call action methods, and assert results.
- No locators or wait logic in tests.

---

## 6. Refactoring from Phase 2

Take every test from Phase 2 (raw Selenium calls) and convert it:

1. Create a `Pages/` folder under `src/main/csharp/`.
2. Create `BasePage` with shared wait/click/type/getText helpers.
3. Create one page class per page/screen.
4. Move locators and actions into the page class.
5. Keep only assertions and test flow in `src/test/csharp/`.

This refactor is the actual learning — don't skip it.

---

## 7. Optional: SpecFlow (BDD) Layer

Add SpecFlow to write Gherkin feature files that drive the same page objects.

### NuGet packages

```
SpecFlow
SpecFlow.NUnit
```

### Directory structure

```
src/test/csharp/
  Features/
    ContactForm.feature
  StepDefinitions/
    ContactSteps.cs
```

### Feature file

```gherkin
# src/test/csharp/Features/ContactForm.feature
Feature: Contact form

  Scenario: Successful contact form submission
    Given I am on the contact page
    When I fill the contact form with valid data
    And I submit the form
    Then I should see "Thanks for your message! We will contact you shortly."
```

### Step definitions

```csharp
// src/test/csharp/StepDefinitions/ContactSteps.cs
using NUnit.Framework;
using OpenQA.Selenium;
using SeleniumCSharpTests.Pages;
using TechTalk.SpecFlow;

namespace SeleniumCSharpTests.StepDefinitions;

[Binding]
public class ContactSteps
{
    private readonly IWebDriver driver;
    private ContactPage contactPage;

    public ContactSteps(IWebDriver driver)
    {
        this.driver = driver;
    }

    [Given("I am on the contact page")]
    public void GivenIAmOnTheContactPage()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/contact");
        contactPage = new ContactPage(driver);
    }

    [When("I fill the contact form with valid data")]
    public void WhenIFillTheContactFormWithValidData()
    {
        contactPage.FillForm("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
    }

    [When("I submit the form")]
    public void WhenISubmitTheForm()
    {
        contactPage.SubmitForm();
    }

    [Then("I should see {string}")]
    public void ThenIShouldSee(string message)
    {
        Assert.That(contactPage.GetAlertText().Trim(), Is.EqualTo(message));
    }
}
```

---

## 8. Multi-Page Flow Example

```csharp
// src/test/csharp/CheckoutTest.cs
[TestFixture]
public class CheckoutTest : TestBase
{
    [Test]
    public void TestLoginSearchAddToCartCheckout()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/auth/login");
        var loginPage = new LoginPage(driver);
        loginPage.Login("customer", "password");

        // Continue through InventoryPage, CartPage, CheckoutPage...
    }
}
```

Each page transition initializes the next page object, keeping the test readable.

---

## 9. Common Pitfalls

| Pitfall | Fix |
|---|---|
| Assertions inside page objects | Move assertions to test classes. Page objects return data; tests assert. |
| Locators duplicated | Define locators as `By` fields on the page object only. |
| Driver not passed to page objects | Pass `driver` via `BasePage` constructor. |
| Page objects know about other page objects | They can, but keep coupling loose; prefer returning the next page object from an action method. |
| Hardcoded waits inside page objects | Use `BasePage`'s `WebDriverWait` for all waits. |
| Using `ExpectedConditions` in C# | Use Selenium 4 lambdas instead — `SeleniumExtras` is legacy. |

---

## 10. Milestone

- `Pages/BasePage.cs` with shared wait/click/type/getText helpers using lambdas.
- 3+ page classes (e.g., `LoginPage`, `ContactPage`, `RegisterPage`) in `src/main/csharp/`.
- `src/test/csharp/` contains only assertions and flow logic.
- A 3+ page test suite runs with a single `dotnet test` command.
- Optional: SpecFlow feature files driving the same page objects.
