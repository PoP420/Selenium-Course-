# API Testing Mastery

Playwright's `APIRequestContext` lets you make HTTP requests directly from within tests — ideal for setup, teardown, and assertions on API responses. No need for `RestAssured` or `HttpClient` wrappers.

---

## The APIRequestContext

Available in tests that extend `PlaywrightTest` as the `Request` property. It shares storage state (cookies, auth) with the browser context when configured.

```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
```

---

## Basic API Requests

### GET

```csharp
[Test]
public async Task TestGetProducts()
{
    var response = await Request.GetAsync(new()
    {
        Url = "https://api.practicesoftwaretesting.com/api/products",
    });

    Assert.That(response.Status, Is.EqualTo(200));
    var json = await response.JsonAsync();
    var data = json.Value.GetProperty("data").GetArrayLength();
    Assert.That(data, Is.GreaterThan(0));
}
```

### POST

```csharp
[Test]
public async Task TestCreateUser()
{
    var response = await Request.PostAsync(new()
    {
        Url = "https://api.practicesoftwaretesting.com/api/users",
        DataObject = new
        {
            name = "John Doe",
            email = "john@example.com",
            password = "Password123"
        }
    });

    Assert.That(response.Status, Is.EqualTo(201));
    var json = await response.JsonAsync();
    Assert.That(json.Value.GetProperty("id").GetInt32(), Is.GreaterThan(0));
}
```

### PUT / PATCH / DELETE

```csharp
var response = await Request.PutAsync(new()
{
    Url = "https://api.example.com/api/users/123",
    DataObject = new { name = "Jane Doe" }
});

var deleteResponse = await Request.DeleteAsync("https://api.example.com/api/users/123");
Assert.That(deleteResponse.Status, Is.EqualTo(204));
```

---

## Request Options

### Query parameters

```csharp
var response = await Request.GetAsync(new()
{
    Url = "https://api.example.com/api/products",
    Query = new Dictionary<string, string>
    {
        ["page"] = "1",
        ["limit"] = "20"
    }
});
```

### Custom headers

```csharp
var response = await Request.GetAsync(new()
{
    Url = "https://api.example.com/api/data",
    Headers = new Dictionary<string, string>
    {
        ["Authorization"] = "Bearer abc123",
        ["X-Request-ID"] = Guid.NewGuid().ToString()
    }
});
```

### JSON body

```csharp
var response = await Request.PostAsync(new()
{
    Url = "https://api.example.com/api/login",
    DataObject = new
    {
        email = "user@test.com",
        password = "pass123"
    },
    Headers = new Dictionary<string, string>
    {
        ["Content-Type"] = "application/json"
    }
});
```

### Raw body

```csharp
var response = await Request.PostAsync(new()
{
    Url = "https://api.example.com/api/upload",
    DataString = "raw payload data",
    Headers = new Dictionary<string, string>
    {
        ["Content-Type"] = "text/plain"
    }
});
```

---

## Response Handling

### Status codes and headers

```csharp
var response = await Request.GetAsync("https://api.example.com/api/users/1");
Assert.That(response.Status, Is.EqualTo(200));

var contentType = response.Headers.TryGetValue("content-type", out var ct) ? ct : null;
Assert.That(contentType, Does.Contain("application/json"));
```

### Deserializing JSON

```csharp
var response = await Request.GetAsync("https://api.example.com/api/users/1");
var json = await response.JsonAsync();

// Access as JsonElement
var userId = json.Value.GetProperty("id").GetInt32();
var email = json.Value.GetProperty("email").GetString();
var isActive = json.Value.GetProperty("active").GetBoolean();

// Or serialize to a POCO
var user = JsonSerializer.Deserialize<UserDto>(await response.BodyAsync());
```

### Saving response body

```csharp
var response = await Request.GetAsync("https://api.example.com/api/export");
var body = await response.BodyAsync();
await File.WriteAllBytesAsync("export.csv", body);
```

---

## API + UI Combo Tests

The power of Playwright is combining API and UI in the same test. Use the API to set up state, then verify the UI reflects it.

### Login via API, test UI

```csharp
[Test]
public async Task TestLoggedInUiState()
{
    // 1. Login via API to get auth token
    var loginResponse = await Request.PostAsync(new()
    {
        Url = "https://api.practicesoftwaretesting.com/api/login",
        DataObject = new { email = "customer", password = "password123" }
    });

    var loginJson = await loginResponse.JsonAsync();
    var token = loginJson.Value.GetProperty("access_token").GetString();

    // 2. Set token as cookie for the browser
    await Page.Context.AddCookiesAsync(new[]
    {
        new Cookie("Authorization", $"Bearer {token}", "practicesoftwaretesting.com")
        {
            Path = "/",
            HttpOnly = true
        }
    });

    // 3. Navigate and verify UI shows logged-in state
    await Page.GotoAsync("https://practicesoftwaretesting.com/authorize");
    await Expect(Page.Locator("text=Account")).ToBeVisibleAsync();
}
```

### Seed test data via API, then verify in UI

```csharp
[Test]
public async Task TestProductAppearsInUi()
{
    // 1. Create a product via API
    await Request.PostAsync(new()
    {
        Url = "https://api.example.com/api/products",
        DataObject = new { name = "Widget 999", price = 99.99 }
    });

    // 2. Verify it appears in the UI
    await Page.GotoAsync("https://example.com/products");
    await Expect(Page.GetByText("Widget 999")).ToBeVisibleAsync();
}
```

---

## Mocking API Responses via Route Interception

You can mock API responses from within browser tests using `Page.RouteAsync`. This is the C# equivalent of WireMock/MockServer.

### Mock a 500 error

```csharp
[Test]
public async Task TestApiErrorHandling()
{
    await Page.RouteAsync("**/api/**", async route =>
    {
        await route.FulfillAsync(new()
        {
            Status = 500,
            ContentType = "application/json",
            Body = "{\"error\": \"Internal server error\"}"
        });
    });

    await Page.GotoAsync("https://example.com/dashboard");
    await Expect(Page.Locator(".error-banner")).ToBeVisibleAsync();
    await Expect(Page.Locator(".error-banner"))
        .ToContainTextAsync("Something went wrong");
}
```

### Mock a slow response

```csharp
[Test]
public async Task TestSlowApiHandling()
{
    await Page.RouteAsync("**/api/data", async route =>
    {
        await Task.Delay(5000);  // 5s delay
        await route.FulfillAsync(new()
        {
            Status = 200,
            Body = "{\"data\": \"delayed\"}"
        });
    });

    await Page.GotoAsync("https://example.com");
    // Verify loading spinner shows, then data appears after 5s
    await Expect(Page.Locator(".loading")).ToBeVisibleAsync();
    await Expect(Page.Locator(".data")).ToContainTextAsync("delayed");
}
```

### Mock partial data

```csharp
[Test]
public async Task TestEmptyState()
{
    await Page.RouteAsync("**/api/products", route =>
    {
        return route.FulfillAsync(new()
        {
            Status = 200,
            ContentType = "application/json",
            Body = "{\"data\": []}"
        });
    });

    await Page.GotoAsync("https://example.com/products");
    await Expect(Page.Locator(".empty-state")).ToBeVisibleAsync();
}
```

---

## Best Practices Summary

1. **Use API for setup** — create users, products, or other state via API before UI tests. Faster and more reliable than UI actions.
2. **Share storage** — configure `APIRequestContext` to share auth state with your browser context.
3. **Mock in browser, not API** — use `Page.RouteAsync` to mock responses within UI tests; use `Request` for real API calls.
4. **Assert on API responses directly** — don't navigate to UI just to check an API response; check it at the source.
5. **Use `JsonObject` / `JsonElement`** — Playwright's `JsonAsync()` returns a `JsonElement` you can query without extra packages.
6. **Save response bodies when debugging** — `await response.BodyAsync()` is useful for inspecting malformed responses.

---

## Interview Questions

1. "How does Playwright's API testing compare to RestAssured/Postman/Newman?"
2. "How do you share authentication state between API and UI tests in Playwright?"
3. "How do you mock a backend failure to test frontend error handling?"
4. "When would you use `APIRequestContext` vs `Page.RouteAsync`?"
5. "How do you handle rate limiting or authentication tokens in Playwright API tests?"
6. "Can you chain API response data into a UI test?"
