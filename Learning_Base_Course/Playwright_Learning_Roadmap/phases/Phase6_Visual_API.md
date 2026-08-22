# Phase 6 — Visual Testing & API Testing

**Time:** 3–4 days

---

## Objectives

- Use Playwright's built-in visual comparison to catch UI regressions.
- Use Playwright's built-in API testing to validate backend endpoints.
- Combine API setup with UI tests for fast, deterministic E2E tests.
- Simulate backend failures to test app error handling.

---

## Topics

### Visual Testing

Playwright's `Expect(locator).ToHaveScreenshotAsync()` automatically:

1. Creates a baseline screenshot on first run.
2. Compares the current screenshot against the baseline.
3. Fails the test if there's a pixel-level difference (above a threshold).

```csharp
// First run: creates test-results/playwright-test-snapshots/login-page-expected.png
// Subsequent runs: compares against baseline
await Expect(Page.Locator("form.login-form")).ToHaveScreenshotAsync(new()
{
    Name = "login-page-expected.png",
    Threshold = 0.2,  // tolerate small rendering differences
});
```

#### Full-page screenshots

```csharp
await Expect(Page).ToHaveScreenshotAsync(new()
{
    Name = "full-login-page.png",
    FullPage = true,
});
```

#### Updating baselines

```powershell
# Regenerate all baseline snapshots
dotnet test -- -UpdateSnapshots
```

### API Testing

Playwright's `APIRequestContext` lets you make HTTP requests directly from within tests — ideal for setup, teardown, and assertions on API responses.

```csharp
// Tests/ApiTest.cs
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

[TestFixture]
public class ApiTest : PlaywrightTest
{
    [Test]
    public async Task TestGetProducts()
    {
        var response = await Request.GetAsync(new()
        {
            Url = "https://api.practicesoftwaretesting.com/api/products",
        });

        Assert.That(response.Status, Is.EqualTo(200));
        var json = await response.JsonAsync();
        // Assert on json structure
    }
}
```

#### API + UI combo test

```csharp
[Test]
public async Task TestLoginViaApiThenVerifyUi()
{
    // 1. Login via API to get auth token
    var loginResponse = await Request.PostAsync(new()
    {
        Url = "https://api.practicesoftwaretesting.com/api/login",
        DataObject = new { email = "customer", password = "password123" },
    });
    var loginJson = await loginResponse.JsonAsync();
    var token = loginJson["access_token"].ToString();

    // 2. Set token as cookie for UI
    await Page.Context.AddCookiesAsync(new[]
    {
        new Cookie("token", token, "practicesoftwaretesting.com") { Path = "/" }
    });

    // 3. Navigate and verify the UI shows the logged-in state
    await Page.GotoAsync("https://practicesoftwaretesting.com/authorize");
    await Expect(Page.Locator("text=Account")).ToBeVisibleAsync();
}
```

#### Mocking API failures

```csharp
[Test]
public async Task TestHandles500Error()
{
    // Route API calls to simulate a failure
    await Page.RouteAsync("**/api/products/**", async route =>
    {
        await route.FulfillAsync(new()
        {
            Status = 500,
            Body = "{\"error\": \"Internal server error\"}",
        });
    });

    await Page.GotoAsync("https://practicesoftwaretesting.com");
    await Expect(Page.Locator(".error-message")).ToBeVisibleAsync();
    await Expect(Page.Locator(".error-message"))
        .ToContainTextAsync("Something went wrong");
}
```

---

## Practice

1. Capture a baseline screenshot for a page element and verify it on a second run.
2. Write an API test that validates a backend endpoint response.
3. Write a combo test that logs in via API, sets the cookie, and verifies the UI.
4. Write a test that mocks a 500 API response and verifies the UI shows an error message.

---

## Skills to Load

- `playwright-csharp-locators-waits` — for `Expect` assertions and route interception.
- `playwright-csharp-parallel` — for running tests across browsers.

---

## Milestone

- A test with `Expect(locator).ToHaveScreenshotAsync()` that catches visual regressions.
- A test that hits an API endpoint directly and asserts the response.
- A combo test that uses API for setup and UI for verification.
- A test that mocks a backend failure and verifies UI error handling.
