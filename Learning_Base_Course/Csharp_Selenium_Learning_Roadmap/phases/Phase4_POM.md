# Phase 4 — Page Object Model (POM)

**Time:** 5–6 days

---

## Objectives

- Understand POM principles and why they matter for maintainability in C#.
- Build a `BasePage` class with shared wait/click/type helpers.
- Refactor all Phase 2–3 test classes into POM classes.
- Optional: layer SpecFlow (Gherkin) on top to write feature files driving the same page objects.

---

## POM Principles

- **One class per page/screen.**
- **Locators as `By` class attributes.**
- **Actions as methods** (e.g., `LoginAs(string username, string password)`).
- **No assertions inside page objects.** Assertions stay in test classes.
- **Inheritance from `BasePage`** for shared waits/helpers.

---

## BasePage Class

```csharp
// Pages/BasePage.cs
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

---

## Page Class Example

```csharp
// Pages/ContactPage.cs
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
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        wait.Until(d => { var el = d.FindElement(firstNameLoc); return el.Displayed ? el : null; }).SendKeys(firstName);
        wait.Until(d => { var el = d.FindElement(lastNameLoc); return el.Displayed ? el : null; }).SendKeys(lastName);
        wait.Until(d => { var el = d.FindElement(emailLoc); return el.Displayed ? el : null; }).SendKeys(email);

        var subjectElement = wait.Until(d => { var el = d.FindElement(subjectLoc); return el.Displayed ? el : null; });
        var subjectSelect = new SelectElement(subjectElement);
        subjectSelect.SelectByValue("webmaster");

        wait.Until(d => { var el = d.FindElement(messageLoc); return el.Displayed ? el : null; }).SendKeys(message);
    }

    public void SubmitForm()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        wait.Until(d =>
        {
            var el = d.FindElement(btnSubmitLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public string GetAlertText()
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        return wait.Until(d => { var el = d.FindElement(alertLoc); return el.Displayed ? el.Text : null; });
    }
}
```

---

## Test Class (Assertions Here)

```csharp
// Tests/ContactTest.cs
using NUnit.Framework;
using SeleniumCSharpTests.Pages;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class ContactTest : TestBase
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

---

## Optional: SpecFlow (BDD) Layer

Add SpecFlow to write feature files that drive the same page objects.

### Feature file

```gherkin
# Features/ContactForm.feature
Feature: Contact form

  Scenario: Successful contact form submission
    Given I am on the contact page
    When I fill the contact form with valid data
    And I submit the form
    Then I should see "Thanks for your message! We will contact you shortly."
```

### Step definitions

```csharp
// StepDefinitions/ContactSteps.cs
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

## Practice

1. Refactor all Phase 2–3 test classes into `Pages/` classes.
2. Create `BasePage` with shared helpers.
3. Write 3+ page classes (e.g., `LoginPage`, `ContactPage`, `AccountPage`).
4. Write tests in `Tests/` that read like business flows.
5. Optional: add SpecFlow feature files for the core user journey.

---

## Skills to Load

- `selenium-page-object-model` — POM design, BasePage, page classes, SpecFlow BDD.

---

## Milestone

- `Pages/BasePage.cs` with shared wait/click/type/GetText helpers.
- 3+ page classes.
- `Tests/` contains only assertions and flow logic.
- A 3+ page test suite runs with a single `dotnet test` command.
- Optional: SpecFlow feature files driving the same page objects.
