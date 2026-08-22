# Phase 4 — Page Object Model (POM)

**Time:** 5–6 days

---

## Objectives

- Understand POM principles and why they matter for maintainability in C# with Playwright.
- Build a `BasePage` class with shared helper methods.
- Refactor all Phase 2–3 test classes into Playwright page models.
- Optional: layer SpecFlow (Gherkin) on top to write feature files driving the same page objects.

---

## POM Principles

- **One class per page/screen.**
- **Locators as `Locator` or `ILocator` fields.** (Playwright locators are lazy and auto-retried — you can define them at class level without worrying about timing.)
- **Action methods** (e.g., `LoginAsync(string email, string password)`).
- **No assertions inside page objects.** Assertions stay in test classes via `Expect()`.
- **Inheritance from `BasePage`** for shared helpers (optional — Playwright page objects can own their own page).

---

## Page Model with Playwright

Playwright page objects differ from Selenium's because locators are lazy. You define locators as fields at the class level, and they don't resolve until an action or assertion runs.

```csharp
// src/main/csharp/Pages/BasePage.cs
using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected ILocator Find(string selector)
    {
        return Page.Locator(selector);
    }

    protected ILocator FindByTestId(string testId)
    {
        return Page.GetByTestId(testId);
    }
}
```

### Page class example

```csharp
// src/main/csharp/Pages/ContactPage.cs
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightCSharpTests.Pages;

public class ContactPage : BasePage
{
    private readonly ILocator FirstNameInput => Page.Locator("#first_name");
    private readonly ILocator LastNameInput => Page.Locator("#last_name");
    private readonly ILocator EmailInput => Page.Locator("#email");
    private readonly ILocator SubjectSelect => Page.Locator("#subject");
    private readonly ILocator MessageInput => Page.Locator("#message");
    private readonly ILocator BtnSubmit => Page.Locator(".btnSubmit");
    private readonly ILocator Alert => Page.Locator(".alert");

    public ContactPage(IPage page) : base(page) { }

    public async Task FillFormAsync(string firstName, string lastName, string email, string message)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await EmailInput.FillAsync(email);

        await SubjectSelect.SelectOptionAsync(new() { Value = "webmaster" });

        await MessageInput.FillAsync(message);
    }

    public async Task SubmitFormAsync()
    {
        await BtnSubmit.ClickAsync();
    }

    public async Task<string> GetAlertTextAsync()
    {
        return await Alert.InnerTextAsync();
    }
}
```

### Test class (assertions here)

```csharp
// src/test/csharp/Tests/ContactTest.cs
using PlaywrightCSharpTests.Pages;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class ContactTest : PlaywrightTest
{
    [Test]
    public async Task TestContactFormSubmit()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/contact");

        var contactPage = new ContactPage(Page);
        await contactPage.FillFormAsync("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters.");
        await contactPage.SubmitFormAsync();

        var alertText = await contactPage.GetAlertTextAsync();
        Assert.That(alertText.Trim(),
            Is.EqualTo("Thanks for your message! We will contact you shortly."));
    }
}
```

### Multi-page flow example

```csharp
// src/test/csharp/Tests/LoginTest.cs
[Test]
public async Task TestLoginFlow()
{
    await Page.GotoAsync("https://practicesoftwaretesting.com/auth/login");

    var loginPage = new LoginPage(Page);
    await loginPage.LoginAsync("customer", "password123");

    var homePage = new HomePage(Page);
    await Expect(homePage.ProductCard).ToBeVisibleAsync();
}
```

Each page transition initializes the next page object, keeping the test readable.

---

## Optional: SpecFlow (BDD) Layer

Add SpecFlow to write Gherkin feature files that drive the same page objects.

### NuGet packages

```
dotnet add package SpecFlow.NUnit
dotnet add package SpecFlow.Tools.MsBuild.Generation
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
using TechTalk.SpecFlow;
using PlaywrightCSharpTests.Pages;
using Microsoft.Playwright.NUnit;

namespace PlaywrightCSharpTests.StepDefinitions;

[Binding]
public class ContactSteps : PlaywrightTest
{
    private ContactPage contactPage;

    [Given("I am on the contact page")]
    public async Task GivenIAmOnTheContactPage()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/contact");
        contactPage = new ContactPage(Page);
    }

    [When("I fill the contact form with valid data")]
    public async Task WhenIFillTheContactFormWithValidData()
    {
        await contactPage.FillFormAsync("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters.");
    }

    [When("I submit the form")]
    public async Task WhenISubmitTheForm()
    {
        await contactPage.SubmitFormAsync();
    }

    [Then("I should see {string}")]
    public async Task ThenIShouldSee(string message)
    {
        var alertText = await contactPage.GetAlertTextAsync();
        Assert.That(alertText.Trim(), Is.EqualTo(message));
    }
}
```

---

## Practice

1. Refactor all Phase 2–3 test classes into `src/main/csharp/Pages/` classes.
2. Create `BasePage` with shared helpers.
3. Write 3+ page classes (e.g., `LoginPage`, `ContactPage`, `AccountPage`).
4. Write tests in `src/test/csharp/` that read like business flows.
5. Optional: add SpecFlow feature files for the core user journey.

---

## Skills to Load

- `playwright-csharp-pom` — POM design, BasePage, page classes, SpecFlow BDD.
- `playwright-csharp-locators-waits` — for locator and assertion patterns used in page objects.

---

## Milestone

- `Pages/BasePage.cs` with shared locator helpers.
- 3+ page classes in `src/main/csharp/`.
- `src/test/csharp/` contains only assertions and flow logic.
- A 3+ page test suite runs with a single `dotnet test` command.
- Optional: SpecFlow feature files driving the same page objects.
