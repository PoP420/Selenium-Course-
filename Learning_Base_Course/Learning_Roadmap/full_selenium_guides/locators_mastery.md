# Locators Mastery

The single most important skill in Selenium. Bad locators = flaky tests = bad reputation in code review. Master this and you'll write tests that survive UI changes.

---

## The 8 Locator Strategies

### 1. `By.id` — **Best choice when available**

```java
By.id("email")
```

- IDs should be unique in the DOM.
- Fastest lookup (browser-level optimization).
- Most resilient to DOM changes.
- **Priority: 1 (use first)**

### 2. `By.name` — **Second best for forms**

```java
By.name("username")
```

- Unique within a form in legacy HTML.
- Good for input fields without IDs.
- **Priority: 2**

### 3. `By.cssSelector` — **Most versatile**

```java
By.cssSelector(".btn-submit")
By.cssSelector("#user-profile > div.card > h2")
By.cssSelector("input[type='email'][placeholder='Enter email']")
By.cssSelector("div[class*='user-card']")
```

- Faster than XPath in most browsers.
- Supports class, ID, attribute, pseudo-class selectors.
- Can combine multiple attributes.
- **Priority: 3**

### 4. `By.xpath` — **Most powerful, most dangerous**

```java
By.xpath("//button[text()='Submit']")
By.xpath("//input[@type='email']")
By.xpath("//div[contains(@class, 'user-card')]")
By.xpath("//label[text()='Email']/following-sibling::input")
```

- Can traverse up and down the DOM.
- Supports text matching, axes (`following-sibling`, `parent`, `ancestor`).
- Slower than CSS on some browsers.
- **Never use absolute XPath** (`/html/body/div[2]/div/div[1]`).
- **Priority: 4 (last resort for complex traversals)**

### 5. `By.className` — **Limited, avoid for compound classes**

```java
By.className("btn-submit")           // OK: single class
```

- Only accepts a single class name.
- Fails on compound classes (`"btn btn-primary"` → use CSS instead).
- **Priority: 5**

### 6. `By.linkText` / `By.partialLinkText` — **For anchors only**

```java
By.linkText("Forgot password?")
By.partialLinkText("Forgot")
```

- Only works on `<a>` elements.
- Good for navigation links.
- **Priority: 6**

### 7. `By.tagName` — **Rarely useful alone**

```java
By.tagName("input")
```

- Returns all elements of a tag.
- Usually needs indexing or filtering.
- **Priority: 7**

### 8. Custom locators — **For advanced cases**

```java
// Shadow DOM (Selenium 4+)
SearchContext shadow = driver.findElement(By.cssSelector("my-component")).getShadowRoot();
shadow.findElement(By.cssSelector(".inner-button"));
```

---

## Locator Priority Cheat Sheet

| Priority | Locator | When to Use |
|---|---|---|
| 1 | `By.id` | Element has a unique, stable ID |
| 2 | `By.name` | Form input with a name attribute |
| 3 | `By.cssSelector` | Most other cases — class, attribute, combined |
| 4 | `By.xpath` | Complex DOM traversal (parent lookup, text matching) |
| 5 | `By.className` | Single class, no ambiguity |
| 6 | `By.linkText` | Anchor text navigation |
| 7 | `By.tagName` | Rare, usually with filtering |

---

## Writing Good CSS Selectors

### Attribute selectors

```java
By.cssSelector("input[type='email']")
By.cssSelector("input[placeholder*='email']")       // contains
By.cssSelector("input[placeholder^='Enter']")       // starts with
By.cssSelector("input[placeholder$='here']")        // ends with
By.cssSelector("[data-testid='submit-btn']")        // data attribute (preferred)
```

### Class selectors

```java
By.cssSelector(".btn-primary")                       // single class
By.cssSelector("button.btn-primary")                 // tag + class (more specific)
By.cssSelector("form .btn-primary")                  // descendant
By.cssSelector("form > .btn-primary")                // direct child
```

### Pseudo-classes

```java
By.cssSelector("input:checked")                      // checked checkbox/radio
By.cssSelector("option:checked")                     // selected option
By.cssSelector("tr:nth-child(2)")                    // second row
```

---

## Writing Good XPath

### Relative XPath (always use this)

```java
By.xpath("//button[text()='Submit']")                // exact text match
By.xpath("//button[contains(text(), 'Submit')]")     // partial text match
By.xpath("//input[@type='email']")                   // attribute match
By.xpath("//div[contains(@class, 'error')]")         // partial class match
```

### XPath Axes (for complex DOM)

```java
// Parent lookup
By.xpath("//input[@id='email']/parent::div")

// Following sibling
By.xpath("//label[text()='Email']/following-sibling::input")

// Ancestor
By.xpath("//span[text()='Error']/ancestor::form")

// Nth element
By.xpath("(//div[@class='card'])[2]")
```

---

## Anti-Patterns to Avoid

### Absolute XPath — **NEVER**

```java
// BAD: breaks on any DOM change
By.xpath("/html/body/div[2]/div/div[1]/form/input[3]")

// GOOD: relative, attribute-based
By.xpath("//input[@type='email']")
```

### Index-based locators — **avoid when possible**

```java
// BAD: fragile
By.cssSelector("div:nth-child(3) > input")

// GOOD: use stable attributes
By.cssSelector("input[name='email']")
```

### Text matching without normalization

```java
// BAD: fails on extra whitespace
By.xpath("//button[text()='Submit']")

// GOOD: normalize whitespace
By.xpath("//button[normalize-space(text())='Submit']")
```

### Locator chaining (find inside find)

```java
// BAD: slows down, harder to debug
driver.findElement(By.cssSelector("form")).findElement(By.id("email"))

// GOOD: single locator
driver.findElement(By.cssSelector("form #email"))
```

---

## Dynamic Elements

### Elements with dynamic IDs

```java
// BAD: ID changes every load
By.id("input-1234-random")

// GOOD: use stable attribute or partial match
By.cssSelector("input[id^='input-']")               // starts with
By.cssSelector("input[id*='email']")                // contains
```

### Elements appearing after JS execution

```java
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
WebElement element = wait.until(
    ExpectedConditions.visibilityOfElementLocated(By.cssSelector(".dynamic-content"))
);
```

### Replacing text in locators

```java
String username = "john_doe";
By.cssSelector(String.format(".user[data-username='%s']", username))
```

---

## Shadow DOM (Selenium 4+)

```java
// Find the shadow host
WebElement shadowHost = driver.findElement(By.cssSelector("my-component"));

// Access the shadow root
SearchContext shadowRoot = shadowHost.getShadowRoot();

// Find elements inside shadow DOM
WebElement innerButton = shadowRoot.findElement(By.cssSelector(".inner-button"));
innerButton.click();
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
