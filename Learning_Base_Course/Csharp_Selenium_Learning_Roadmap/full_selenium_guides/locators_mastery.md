# Locators Mastery

The single most important skill in Selenium. Bad locators = flaky tests = bad reputation in code review. Master this and you'll write tests that survive UI changes.

---

## The 8 Locator Strategies

### 1. `By.Id` — **Best choice when available**

```csharp
By.Id("email")
```

- IDs should be unique in the DOM.
- Fastest lookup (browser-level optimization).
- Most resilient to DOM changes.
- **Priority: 1 (use first)**

### 2. `By.Name` — **Second best for forms**

```csharp
By.Name("username")
```

- Unique within a form in legacy HTML.
- Good for input fields without IDs.
- **Priority: 2**

### 3. `By.CssSelector` — **Most versatile**

```csharp
By.CssSelector(".btn-submit")
By.CssSelector("#user-profile > div.card > h2")
By.CssSelector("input[type='email'][placeholder='Enter email']")
By.CssSelector("div[class*='user-card']")
```

- Faster than XPath in most browsers.
- Supports class, ID, attribute, pseudo-class selectors.
- Can combine multiple attributes.
- **Priority: 3**

### 4. `By.XPath` — **Most powerful, most dangerous**

```csharp
By.XPath("//button[text()='Submit']")
By.XPath("//input[@type='email']")
By.XPath("//div[contains(@class, 'user-card')]")
By.XPath("//label[text()='Email']/following-sibling::input")
```

- Can traverse up and down the DOM.
- Supports text matching, axes (`following-sibling`, `parent`, `ancestor`).
- Slower than CSS on some browsers.
- **Never use absolute XPath** (`/html/body/div[2]/div/div[1]`).
- **Priority: 4 (last resort for complex traversals)**

### 5. `By.ClassName` — **Limited, avoid for compound classes**

```csharp
By.ClassName("btn-submit")           // OK: single class
```

- Only accepts a single class name.
- Fails on compound classes (`"btn btn-primary"` → use CSS instead).
- **Priority: 5**

### 6. `By.LinkText` / `By.PartialLinkText` — **For anchors only**

```csharp
By.LinkText("Forgot password?")
By.PartialLinkText("Forgot")
```

- Only works on `<a>` elements.
- Good for navigation links.
- **Priority: 6**

### 7. `By.TagName` — **Rarely useful alone**

```csharp
By.TagName("input")
```

- Returns all elements of a tag.
- Usually needs indexing or filtering.
- **Priority: 7**

### 8. Custom locators — **For advanced cases**

```csharp
// Shadow DOM (Selenium 4+)
IWebElement shadowHost = driver.FindElement(By.CssSelector("my-component"));
ISearchContext shadowRoot = shadowHost.GetShadowRoot();
IWebElement innerButton = shadowRoot.FindElement(By.CssSelector(".inner-button"));
```

---

## Locator Priority Cheat Sheet

| Priority | Locator | When to Use |
|---|---|---|
| 1 | `By.Id` | Element has a unique, stable ID |
| 2 | `By.Name` | Form input with a name attribute |
| 3 | `By.CssSelector` | Most other cases — class, attribute, combined |
| 4 | `By.XPath` | Complex DOM traversal (parent lookup, text matching) |
| 5 | `By.ClassName` | Single class, no ambiguity |
| 6 | `By.LinkText` | Anchor text navigation |
| 7 | `By.TagName` | Rare, usually with filtering |

---

## Writing Good CSS Selectors

### Attribute selectors

```csharp
By.CssSelector("input[type='email']")
By.CssSelector("input[placeholder*='email']")       // contains
By.CssSelector("input[placeholder^='Enter']")       // starts with
By.CssSelector("input[placeholder$='here']")        // ends with
By.CssSelector("[data-testid='submit-btn']")        // data attribute (preferred)
```

### Class selectors

```csharp
By.CssSelector(".btn-primary")                       // single class
By.CssSelector("button.btn-primary")                 // tag + class (more specific)
By.CssSelector("form .btn-primary")                  // descendant
By.CssSelector("form > .btn-primary")                // direct child
```

### Pseudo-classes

```csharp
By.CssSelector("input:checked")                      // checked checkbox/radio
By.CssSelector("option:checked")                     // selected option
By.CssSelector("tr:nth-child(2)")                    // second row
```

---

## Writing Good XPath

### Relative XPath (always use this)

```csharp
By.XPath("//button[text()='Submit']")                // exact text match
By.XPath("//button[contains(text(), 'Submit')]")     // partial text match
By.XPath("//input[@type='email']")                   // attribute match
By.XPath("//div[contains(@class, 'error')]")         // partial class match
```

### XPath Axes (for complex DOM)

```csharp
// Parent lookup
By.XPath("//input[@id='email']/parent::div")

// Following sibling
By.XPath("//label[text()='Email']/following-sibling::input")

// Ancestor
By.XPath("//span[text()='Error']/ancestor::form")

// Nth element
By.XPath("(//div[@class='card'])[2]")
```

---

## Anti-Patterns to Avoid

### Absolute XPath — **NEVER**

```csharp
// BAD: breaks on any DOM change
By.XPath("/html/body/div[2]/div/div[1]/form/input[3]")

// GOOD: relative, attribute-based
By.XPath("//input[@type='email']")
```

### Index-based locators — **avoid when possible**

```csharp
// BAD: fragile
By.CssSelector("div:nth-child(3) > input")

// GOOD: use stable attributes
By.CssSelector("input[name='email']")
```

### Text matching without normalization

```csharp
// BAD: fails on extra whitespace
By.XPath("//button[text()='Submit']")

// GOOD: normalize whitespace
By.XPath("//button[normalize-space(text())='Submit']")
```

### Locator chaining (find inside find)

```csharp
// BAD: slows down, harder to debug
driver.FindElement(By.CssSelector("form")).FindElement(By.Id("email"))

// GOOD: single locator
driver.FindElement(By.CssSelector("form #email"))
```

---

## Dynamic Elements

### Elements with dynamic IDs

```csharp
// BAD: ID changes every load
By.Id("input-1234-random")

// GOOD: use stable attribute or partial match
By.CssSelector("input[id^='input-']")               // starts with
By.CssSelector("input[id*='email']")                // contains
```

### Elements appearing after JS execution

```csharp
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
IWebElement element = wait.Until(d =>
{
    var el = d.FindElement(By.CssSelector(".dynamic-content"));
    return el.Displayed ? el : null;
});
```

---

## Shadow DOM (Selenium 4+)

```csharp
// Find the shadow host
IWebElement shadowHost = driver.FindElement(By.CssSelector("my-component"));

// Access the shadow root
ISearchContext shadowRoot = shadowHost.GetShadowRoot();

// Find elements inside shadow DOM
IWebElement innerButton = shadowRoot.FindElement(By.CssSelector(".inner-button"));
innerButton.Click();
```

---

## Best Practices Summary

1. **IDs first** — always check if an element has a stable ID.
2. **Data attributes** — `data-testid`, `data-cy` are the most stable. Ask devs to add them.
3. **CSS over XPath** — use XPath only when you need DOM traversal.
4. **No absolute XPath** — always relative and attribute-based.
5. **Avoid indexes** — use stable attributes instead of `nth-child`.
6. **Consistent style** — pick CSS or XPath per project, don't mix unnecessarily.
7. **Page objects** — keep all locators in page classes, never in test classes.
8. **Descriptive names** — `emailLoc`, not `loc1` or `l`.

---

## Interview Questions

1. "What's your locator priority and why?"
2. "How do you handle dynamic element IDs?"
3. "What's the difference between CSS and XPath? When would you pick one over the other?"
4. "How do you locate elements inside a shadow DOM?"
5. "What makes a locator flaky, and how do you fix it?"
