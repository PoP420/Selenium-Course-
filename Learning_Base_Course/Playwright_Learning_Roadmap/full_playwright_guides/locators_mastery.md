# Locators Mastery

The single most important skill in any UI automation framework. Bad locators = flaky tests = bad reputation in code review. Master this and you'll write tests that survive UI changes.

---

## The Locator System

Playwright doesn't use `By.*` finders like Selenium. Instead, it uses **Locator** objects — lazy, auto-retried, and composable.

```csharp
ILocator locator = page.Locator("css=button.submit");
await locator.ClickAsync();  // DOM query happens here, not at creation
```

Locators are:
- **Lazy** — the DOM is not queried until you perform an action or assertion.
- **Auto-retried** — if the element isn't ready, Playwright re-queries automatically.
- **Stable** — use the recommended strategies below for resilient locators.

---

## The 7 Locator Strategies

### 1. `GetByRole` — **Best choice (accessible)**

```csharp
page.GetByRole("button", new() { Name = "Submit" })
page.GetByRole("link", new() { NameString = "Forgot password?" })
page.GetByRole("heading", new() { Name = "Welcome Back" })
page.GetByRole("checkbox", new() { NameString = "Remember me" })
```

- Based on [ARIA roles](https://www.w3.org/TR/wai-aria-1.2/#roles).
- Most resilient to DOM changes — survives restructuring.
- **Priority: 1 (use first)**

### 2. `GetByTestId` — **Best for test attributes**

```csharp
page.GetByTestId("submit-btn")
page.GetByTestId("login-form")
```

- Looks for `data-testid`, `data-test`, `data-qa` attributes (configurable).
- Stable when added by developers.
- **Priority: 2**

> **Config:** Customize the test ID attribute in your config:
> ```csharp
> new PlaywrightTestOptions { TestIdAttribute = "data-cy" }
> ```

### 3. `GetByLabel` — **Best for form fields**

```csharp
page.GetByLabel("Email address")
page.GetByLabel("Password")
```

- Matches `<label>` elements whose text or `for` attribute points to the input.
- Most resilient for forms — label text rarely changes.
- **Priority: 3**

### 4. `GetByText` — **Best for text content**

```csharp
page.GetByText("Thanks for your message!")
page.GetByText("Forgot", new() { Exact = false })  // partial match
```

- Matches visible text on the page.
- **Priority: 4**

### 5. `GetByPlaceholder` — **For placeholder attributes**

```csharp
page.GetByPlaceholder("Enter your email")
```

- Only works on elements with a `placeholder` attribute.
- **Priority: 5**

### 6. `GetByAltText` / `GetByTitle` — **For media and tooltips**

```csharp
page.GetByAltText("Company logo")
page.GetByTitle("Click to expand")
```

- **Priority: 6**

### 7. `page.Locator()` — **CSS/XPath fallback**

```csharp
page.Locator("css=.btn-submit")
page.Locator("xpath=//button[contains(text(), 'Submit')]")
```

- Use CSS by default; XPath only when CSS can't express what you need.
- **Priority: 7 (fallback)**

---

## Locator Priority Cheat Sheet

| Priority | Strategy | When to Use |
|---|---|---|
| 1 | `GetByRole` | Element has a semantic role (button, link, heading, checkbox) |
| 2 | `GetByTestId` | Element has `data-testid` / `data-test` attribute |
| 3 | `GetByLabel` | Form input with an associated `<label>` |
| 4 | `GetByText` | Text content on the page |
| 5 | `GetByPlaceholder` | Input with a placeholder attribute |
| 6 | `GetByAltText` / `GetByTitle` | Image alt text or title attribute |
| 7 | `page.Locator("css=...")` | Complex selectors, nth-child, attribute combos |

---

## Writing Good Locators

### CSS selectors (via `page.Locator`)

```csharp
page.Locator("css=form > button[type='submit']")
page.Locator("css=input[type='email'][placeholder*='email']")
page.Locator("css=[data-testid='submit-btn']")
```

### XPath (via `page.Locator`)

```csharp
page.Locator("xpath=//button[text()='Submit']")
page.Locator("xpath=//div[contains(@class, 'error')]")
page.Locator("xpath=//label[text()='Email']/following-sibling::input")
```

### Filtering locators

```csharp
// nth element
page.Locator("li.item").Nth(2)

// First/last
page.Locator("li.item").First
page.Locator("li.item").Last

// Filter by text
page.GetByRole("listitem").And(page.GetByText("Active"))

// Filter by descendant
page.GetByRole("listitem").Locator("text=Buy Now")
```

---

## Anti-Patterns to Avoid

### Absolute CSS / XPath — **NEVER**

```csharp
// BAD: breaks on any DOM change
page.Locator("html > body > div:nth-child(2) > div > form > input:nth-child(3)")

// GOOD: relative, attribute-based
page.Locator("css=input[type='email']")
```

### Index-based locators — **avoid when possible**

```csharp
// BAD: fragile
page.Locator("form > input:nth-child(3)")

// GOOD: use stable attributes
page.Locator("css=input[name='email']")
```

### Text matching without care

```csharp
// Playwright text matching is exact by default
page.GetByText("Submit")         // exact match
page.GetByText("Submit", new() { Exact = false }) // contains "Submit"
```

### Locator chaining (find inside find)

```csharp
// BAD: unnecessary chaining
await page.Locator("form").Locator("input#email").FillAsync("test@test.com");

// GOOD: single locator
await page.Locator("css=form input#email").FillAsync("test@test.com");
```

---

## Dynamic Elements

### Elements with dynamic IDs

```csharp
// BAD: ID changes every load
page.Locator("id=input-1234-random")

// GOOD: use stable attribute
page.Locator("css=input[data-test='email-input']")
page.Locator("xpath=//input[starts-with(@id, 'input-')]")
```

### Elements appearing after JS execution

```csharp
// No wait needed — auto-waiting handles it
await page.Locator(".dynamic-content").ClickAsync();

// If you need to wait for visibility explicitly:
await page.Locator(".dynamic-content").WaitForAsync(
    new() { State = WaitForSelectorState.Visible });
```

---

## Shadow DOM

```csharp
// Playwright handles shadow DOM automatically in CSS selectors
await page.Locator("css=my-component >> .inner-button").ClickAsync();

// Or use the :has() / pierce-like syntax (auto-handled by Playwright)
await page.Locator("css=my-custom-element::part(button)").ClickAsync();
```

---

## Best Practices Summary

1. **Role-based locators first** — `GetByRole` survives DOM restructuring.
2. **Test IDs second** — `data-testid` is the most stable when added by devs.
3. **Label/text third** — `GetByLabel` and `GetByText` are resilient for forms and UI text.
4. **CSS over XPath** — use XPath only when you need DOM traversal (parent lookup, siblings).
5. **No absolute selectors** — always relative and attribute-based.
6. **Avoid indexes** — use stable attributes instead of `nth-child`.
7. **Lazy locators** — define `ILocator` fields at class level, they don't resolve until used.
8. **Descriptive names** — `SubmitButton`, not `btn` or `loc1`.

---

## Interview Questions

1. "What's your locator priority in Playwright and why?"
2. "How does Playwright's Locator system differ from Selenium's `By.*`?"
3. "How do you handle dynamic element IDs?"
4. "What's the difference between CSS and XPath in Playwright locators? When would you pick one over the other?"
5. "How do you locate elements inside a shadow DOM?"
6. "What makes a locator flaky, and how do you fix it?"
7. "How does auto-retries make locators more resilient?"
