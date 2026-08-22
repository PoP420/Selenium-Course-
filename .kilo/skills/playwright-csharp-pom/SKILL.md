---
name: playwright-csharp-pom
description: Page Object Model (POM) patterns for Playwright in C# — page classes with ILocator fields, async action methods, BasePage with IPage, no assertions inside page objects, and TestBase inheriting from PageTest. Use when building or refactoring a C# Playwright test framework, creating BasePage, or adding page object structure.
---

# Playwright Page Object Model (POM) — C#

## Purpose

This skill teaches how to structure a maintainable Playwright test framework using POM in C#. It covers page classes in `src/main/csharp`, test classes in `src/test/csharp`, a shared `BasePage` with lazy `ILocator` fields, and the transition from Selenium's `By` locators + `WebDriverWait` to Playwright's `ILocator` with built-in auto-waiting. Use it when building or refactoring a C# Playwright suite, designing page classes, or translating Selenium POM code to Playwright.

## When to Apply

- User asks how to structure a C# Playwright test framework or refactor into POM.
- User asks about `BasePage`, `IPage`, or where assertions belong.
- User asks about lazy locators, `ILocator`, or `IPage.GotoAsync`.
- User asks how to avoid duplicated locators or wait logic across tests.
- User asks how to convert Selenium `By` / `WebDriverWait` code to Playwright `ILocator`.

---

## 1. POM Principles (Playwright vs Selenium)

| Concern | Selenium C# | Playwright C# |
|---|---|---|
| Locator type | `By` (resolved immediately) | `ILocator` (lazy — evaluated at action time) |
| Wait mechanism | `WebDriverWait` + lambdas | Built-in auto-waiting on every `Locator` action |
| Element finding | `driver.FindElement(By.Id("..."))` | `Page.Locator("#...")` or `Page.GetByLabel(...)` |
| Assertion | `Assert.That(element.Text, ...)` | `Expect(locator).ToContainTextAsync("...")` |
| Base class | Custom `TestBase` with `IWebDriver` | `PageTest` from `Microsoft.Playwright.NUnit` |

- **One class per page/screen.**
- **Locators as `ILocator` fields** — lazy evaluation means they auto-wait at action time.
- **Actions as async methods** (e.g., `FillFormAsync(...)`, `SubmitFormAsync()`).
- **No assertions inside page objects.** Assertions stay in test classes.
- **Inheritance from `BasePage`** for shared `IPage` and helper methods.

### Why this matters

- `ILocator` fields are **lazy** — they don't query the DOM until you call `ClickAsync`, `FillAsync`, etc. This means they always use the latest DOM state.
- Playwright's auto-waiting eliminates the need for `WebDriverWait` in page objects.
- Business logic reads like English in the test class.
- Page classes become reusable across multiple test suites.

---

## 2. Project Structure

```
PlaywrightCSharpTests/
  PlaywrightCSharpTests.csproj          ← main project (Playwright + page objects)
  PlaywrightCSharpTests.Tests/
    PlaywrightCSharpTests.Tests.csproj  ← test project (NUnit + Playwright.NUnit)
    src/
      test/
        csharp/
          Fixtures/
            TestBase.cs                  ← extends PageTest
          Tests/
            ContactFormTest.cs
            LoginTest.cs
            RegisterTest.cs
            HomePageTest.cs
  src/
    main/
      csharp/
        Pages/
          BasePage.cs                    ← shared IPage + helpers
          ContactPage.cs
          LoginPage.cs
          NavBar.cs
          RegisterPage.cs
          HomePage.cs
        Utils/
          TestLogger.cs
```

---

## 3. BasePage Class

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

    protected ILocator Find(string selector) => Page.Locator(selector);
    protected ILocator FindByTestId(string testId) => Page.GetByTestId(testId);
}
```

**Key notes:**
- Uses `IPage` (Playwright's page abstraction) instead of `IWebDriver`.
- `Find` returns a **lazy** `ILocator` — no DOM query until an action is called.
- `FindByTestId` maps to `data-testid` attributes (configurable via `PlaywrightTestOptions`).

---

## 4. Page Class Example

### ContactPage

```csharp
// src/main/csharp/Pages/ContactPage.cs
using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class ContactPage : BasePage
{
    private ILocator FirstNameInput => Page.Locator("#first_name");
    private ILocator LastNameInput => Page.Locator("#last_name");
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator SubjectSelect => Page.Locator("#subject");
    private ILocator MessageInput => Page.Locator("#message");
    private ILocator BtnSubmit => Page.Locator("[data-test='contact-submit']");
    private ILocator Alert => Page.Locator(".alert");

    public ContactPage(IPage page) : base(page) { }

    public async Task NavigateToContactAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/contact");
        await FirstNameInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task FillFormAsync(string firstName, string lastName, string email, string message)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await EmailInput.FillAsync(email);
        await SubjectSelect.SelectOptionAsync("webmaster");
        await MessageInput.FillAsync(message);
    }

    public async Task SubmitFormAsync()
    {
        await BtnSubmit.ClickAsync();
    }

    public async Task<string> GetAlertTextAsync()
    {
        return (await Alert.InnerTextAsync()).Trim();
    }
}
```

### LoginPage

```csharp
// src/main/csharp/Pages/LoginPage.cs
using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class LoginPage : BasePage
{
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator PasswordInput => Page.Locator("#password");
    private ILocator LoginButton => Page.Locator("[data-test='login-submit']");
    private ILocator AlertMessage => Page.Locator(".alert");
    private ILocator NavHomeLink => Page.Locator("a[data-test='nav-home']");

    public LoginPage(IPage page) : base(page) { }

    public async Task NavigateToLoginAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/login");
        await EmailInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string> GetErrorMessageAsync()
    {
        return (await AlertMessage.InnerTextAsync()).Trim();
    }
}
```

### HomePage

```csharp
// src/main/csharp/Pages/HomePage.cs
using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class HomePage : BasePage
{
    private ILocator ProductCard => Page.Locator("a.card[data-test^='product-']");

    public HomePage(IPage page) : base(page) { }

    public async Task NavigateAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/");
        await ProductCard.First.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task<bool> IsProductContainerDisplayedAsync()
    {
        return await ProductCard.First.IsVisibleAsync();
    }

    public async Task<int> GetProductCardCountAsync()
    {
        return await ProductCard.CountAsync();
    }

    public async Task ClickProductCardAsync(int index = 0)
    {
        await ProductCard.Nth(index).ClickAsync();
    }
}
```

### RegisterPage

```csharp
// src/main/csharp/Pages/RegisterPage.cs
using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class RegisterPage : BasePage
{
    private ILocator FirstNameInput => Page.Locator("#first_name");
    private ILocator LastNameInput => Page.Locator("#last_name");
    private ILocator DobInput => Page.Locator("#dob");
    private ILocator CountrySelect => Page.Locator("#country");
    private ILocator PostalCodeInput => Page.Locator("#postal_code");
    private ILocator HouseNumberInput => Page.Locator("#house_number");
    private ILocator StreetInput => Page.Locator("#street");
    private ILocator CityInput => Page.Locator("#city");
    private ILocator StateInput => Page.Locator("#state");
    private ILocator PhoneInput => Page.Locator("#phone");
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator PasswordInput => Page.Locator("#password");
    private ILocator BtnSubmit => Page.Locator(".btnSubmit");
    private ILocator Alert => Page.Locator(".alert");

    public RegisterPage(IPage page) : base(page) { }

    public async Task NavigateToRegisterAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/register");
        await FirstNameInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task RegisterAsync(
        string firstName, string lastName, string dob, string postalCode,
        string houseNumber, string street, string city, string state,
        string phone, string email, string password)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await DobInput.FillAsync(dob);
        await CountrySelect.SelectOptionAsync("PH");
        await PostalCodeInput.FillAsync(postalCode);
        await HouseNumberInput.FillAsync(houseNumber);
        await StreetInput.FillAsync(street);
        await CityInput.FillAsync(city);
        await StateInput.FillAsync(state);
        await PhoneInput.FillAsync(phone);
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
    }

    public async Task SubmitFormAsync() => await BtnSubmit.ClickAsync();
    public async Task<string> GetAlertTextAsync() => (await Alert.InnerTextAsync()).Trim();
}
```

### NavBar

```csharp
// src/main/csharp/Pages/NavBar.cs
using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class NavBar : BasePage
{
    private ILocator NavHome => Page.Locator("a[data-test='nav-home']");
    private ILocator NavLogin => Page.Locator("a[data-test='nav-sign-in']");
    private ILocator NavContact => Page.Locator("a[data-test='nav-contact']");

    public NavBar(IPage page) : base(page) { }

    public async Task GoToHomeAsync() => await NavHome.ClickAsync();
    public async Task GoToLoginAsync() => await NavLogin.ClickAsync();
    public async Task GoToContactAsync() => await NavContact.ClickAsync();
}
```

---

## 5. Test Class (Assertions Here)

```csharp
// src/test/csharp/Tests/ContactFormTest.cs
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class ContactFormTest : TestBase
{
    [Test]
    public async Task TestContactFormSubmit()
    {
        var contactPage = new ContactPage(Page);
        await contactPage.NavigateToContactAsync();

        await contactPage.FillFormAsync("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters.");
        await contactPage.SubmitFormAsync();

        var alertText = await contactPage.GetAlertTextAsync();
        Assert.That(alertText,
            Is.EqualTo("Thanks for your message! We will contact you shortly."));
    }
}
```

```csharp
// src/test/csharp/Tests/LoginTest.cs
using System.Text.RegularExpressions;
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class LoginTest : TestBase
{
    [Test]
    public async Task TestLoginPageLoads()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateToLoginAsync();

        await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));
        await Expect(Page).ToHaveURLAsync("https://practicesoftwaretesting.com/auth/login");
    }

    [Test]
    public async Task TestLoginWithInvalidCredentials()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateToLoginAsync();

        await loginPage.LoginAsync("invalid@example.com", "wrongpassword");

        var errorMessage = await loginPage.GetErrorMessageAsync();
        Assert.That(errorMessage, Does.Contain("Invalid email or password"));
    }
}
```

```csharp
// src/test/csharp/Tests/NavBarTest.cs
using System.Text.RegularExpressions;
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class NavBarTest : TestBase
{
    [Test]
    public async Task TestNavigationBarLinks()
    {
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        var navBar = new NavBar(Page);
        await navBar.GoToContactAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(".*contact.*"));

        await navBar.GoToLoginAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*auth/login.*"));

        await navBar.GoToHomeAsync();
        await Expect(Page).ToHaveURLAsync("https://practicesoftwaretesting.com/");
    }
}
```

**Key notes:**
- Tests extend `TestBase` (which extends `PageTest`) for the `Page` property.
- Tests instantiate page objects, call action methods, and assert results.
- No locators or wait logic in tests.
- Playwright `Expect` assertions auto-wait — they retry until the condition is met or timeout.

---

## 6. Refactoring from Selenium POM

Taking a Selenium page object and converting it to Playwright:

| Selenium C# | Playwright C# |
|---|---|
| `private readonly By emailLoc = By.Id("email");` | `private ILocator EmailInput => Page.Locator("#email");` |
| `wait.Until(d => d.FindElement(By.Id("email")))` | `await EmailInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });` |
| `element.SendKeys("text")` | `await EmailInput.FillAsync("text");` |
| `element.Click()` | `await BtnSubmit.ClickAsync();` |
| `new SelectElement(element).SelectByValue("webmaster")` | `await SubjectSelect.SelectOptionAsync("webmaster");` |
| `element.Text` | `await element.InnerTextAsync()` |

### Example conversion

```csharp
// Selenium — explicit wait before every interaction
var el = wait.Until(d =>
{
    var element = d.FindElement(By.Id("email"));
    return element.Displayed ? element : null;
});
el.Clear();
el.SendKeys("test@example.com");

// Playwright — lazy locator + auto-waiting
await Page.Locator("#email").FillAsync("test@example.com");
```

Playwright's `ILocator` is lazy (the DOM is queried at action time, not at field access time) and auto-waits. This eliminates the need for most explicit waits.

---

## 7. Page Transitions

Page objects can return the next page object, keeping the flow fluent:

```csharp
// LoginPage.cs
public async Task<HomePage> LoginAndNavigateHomeAsync(string email, string password)
{
    await EmailInput.FillAsync(email);
    await PasswordInput.FillAsync(password);
    await LoginButton.ClickAsync();
    await Page.Locator("a[data-test='nav-home']").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    return new HomePage(Page);
}
```

```csharp
// In test:
var loginPage = new LoginPage(Page);
await loginPage.NavigateToLoginAsync();
var homePage = await loginPage.LoginAndNavigateHomeAsync("user@example.com", "pass123");
await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));
```

---

## 8. Common Pitfalls

| Pitfall | Fix |
|---|---|
| Assertions inside page objects | Move assertions to test classes. Page objects return data via `Task<string>`, `Task<int>`, etc. |
| Using `ILocator` fields eagerly (querying DOM at field access) | `ILocator` is **lazy** — don't call `.CountAsync()` or `.InnerTextAsync()` before the page is ready. Use `.WaitForAsync()` first. |
| `driver`/`IWebDriver` references in Playwright code | Use `IPage` instead. `Page` comes from `PageTest` or is passed via `BasePage` constructor. |
| Selenium `By.Id("email")` vs Playwright `#email` | CSS selectors are simpler in Playwright: `#email`, `.btnSubmit`, `[data-test='login-submit']`. |
| `WebDriverWait` in page objects | Remove all `WebDriverWait` — Playwright auto-waits on every `Locator` action. |
| Page object constructor takes `IWebDriver` | Change to `IPage`: `public LoginPage(IPage page) : base(page) { }`. |
| `GetByRole(AriaRole.Button, ...)` not finding `input[type=submit]` | It does work! `input[type=submit]` maps to ARIA role `button`. `NameString` matches `aria-label` or `value` attributes. |
| Selector `nav-login` doesn't exist | The site uses `nav-sign-in`. Always verify selectors against the live site. |

---

## 9. Milestone

- `Pages/BasePage.cs` with `IPage` and `Find`/`FindByTestId` helpers.
- 5+ page classes (`LoginPage`, `ContactPage`, `RegisterPage`, `HomePage`, `NavBar`) in `src/main/csharp/`.
- `src/test/csharp/` contains only assertions and flow logic.
- A 5+ page test suite runs with a single `dotnet test` command.
- Page objects use `ILocator` fields + async methods; no `WebDriverWait` or `IWebDriver`.
