# PlaywrightCSharpTests

Portfolio-ready Playwright test automation framework in C#/.NET using NUnit + `Microsoft.Playwright.NUnit`.

## Stack

| Layer | Technology |
|---|---|
| Language | C# |
| Framework | Microsoft.Playwright.NUnit |
| Test Runner | NUnit 4 |
| Build | .NET 8+ / .NET 10 |
| Browsers | Playwright (Chromium, Firefox, WebKit) |
| CI | GitHub Actions |
| Parallelism | NUnit `[Parallelizable]` + Playwright browser contexts |

## Project Structure

```
PlaywrightCSharpTests/
├── PlaywrightCSharpTests.csproj          # main project (page objects, utils)
├── PlaywrightCSharpTests.Tests/
│   ├── PlaywrightCSharpTests.Tests.csproj
│   └── src/test/csharp/
│       ├── Fixtures/
│       │   └── TestBase.cs              # PlaywrightTest base, screenshot-on-failure
│       ├── Tests/
│       │   ├── VerifySetupTest.cs
│       │   ├── LoginTest.cs
│       │   ├── ContactFormTest.cs
│       │   ├── HomePageTest.cs
│       │   ├── RegisterTest.cs
│       │   ├── NavBarTest.cs
│       │   └── CrossBrowserTest.cs
│       └── Features/
│           └── ContactForm.feature
├── src/main/csharp/
│   ├── Pages/
│   │   ├── BasePage.cs
│   │   ├── LoginPage.cs
│   │   │   ├── ContactPage.cs
│   │   │   ├── HomePage.cs
│   │   │   ├── NavBar.cs
│   │   │   ├── ProductDetailPage.cs
│   │   │   └── RegisterPage.cs
│   └── Utils/
│       └── TestLogger.cs
├── .github/workflows/tests.yml
└── PlaywrightCSharpTests.slnx
```

## Prerequisites

- .NET 8+ SDK
- Chrome browser (optional — Playwright downloads its own browsers)

## Setup

```powershell
# Restore + build + install Playwright browsers
dotnet build
playwright install
```

## Run Tests

```powershell
# All tests (headed, for debugging)
dotnet test

# Headless (CI mode)
dotnet test --filter "FullyQualifiedName~VerifySetupTest"

# Cross-browser (specify in test or via config)
dotnet test --filter "FullyQualifiedName~CrossBrowserTest"
```

## CI Status

![Playwright Tests](https://github.com/<user>/<repo>/actions/workflows/tests.yml/badge.svg)

## Key Differences from Selenium C#

| Feature | Selenium C# | Playwright C# |
|---|---|---|
| Driver | Selenium Manager / ChromeDriver | Built-in browser binaries |
| Locators | `By.Id("...")` (eager) | `page.Locator(...)` (lazy, auto-retried) |
| Waits | `WebDriverWait` + `ExpectedConditions` | Auto-waiting on every action; `Expect` for assertions |
| Debugging | Manual screenshots + logs | Built-in Tracing, Video, Console capture |
| Page model | Constructor takes `IWebDriver` | Constructor takes `IPage` |
| Parallelism | Selenium Grid / NUnit parallel | Browser contexts (native isolation) |
| API testing | RestAssured / HttpClient | Built-in `APIRequestContext` |
| Visual testing | Sikuli / Applitools | Built-in `Expect(locator).ToHaveScreenshotAsync()` |
