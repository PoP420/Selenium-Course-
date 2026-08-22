# Waits & Synchronization Mastery

The #1 cause of flaky Selenium tests is bad timing. Master waits and your tests will be reliable enough to run in CI without random failures.

---

## The Three Wait Types

### 1. Implicit Wait — **Avoid**

```java
driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(10));
```

- Tells WebDriver to poll the DOM for up to N seconds before throwing `NoSuchElementException`.
- **Global** — applies to every `findElement` call.
- **Dangerous** when mixed with explicit waits (they multiply!).
- **Legacy pattern.** Use explicit waits instead.

### 2. Explicit Wait — **Use this**

```java
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
WebElement element = wait.until(
    ExpectedConditions.visibilityOfElementLocated(By.id("email"))
);
```

- Waits for a specific condition on a specific element.
- **Local scope** — only affects the element you're waiting for.
- Preferred for all dynamic content.

### 3. Fluent Wait — **For advanced cases**

```java
Wait<WebDriver> wait = new FluentWait<>(driver)
    .withTimeout(Duration.ofSeconds(30))
    .pollingEvery(Duration.ofMillis(500))
    .ignoring(NoSuchElementException.class);

WebElement element = wait.until(driver -> {
    return driver.findElement(By.id("email"));
});
```

- Custom polling interval.
- Can ignore specific exceptions.
- Use when elements appear in batches or with unusual timing.

---

## ExpectedConditions Deep Dive

### Element State Conditions

| Condition | Use When |
|---|---|
| `presenceOfElementLocated` | Element exists in DOM (may be hidden) |
| `visibilityOfElementLocated` | Element is in DOM AND visible (has size, not `display:none`) |
| `invisibilityOfElementLocated` | Element is gone or hidden (loading spinners) |
| `elementToBeClickable` | Visible AND enabled (safe to click) |
| `stalenessOf(element)` | Element is detached from DOM (after page load/refresh) |

### Text Conditions

| Condition | Use When |
|---|---|
| `textToBePresentInElementLocated` | Text appears inside an element |
| `textToBePresentInElementValue` | Text appears in input value |
| `elementTextContains` | Element text contains substring |

### Alert Conditions

| Condition | Use When |
|---|---|
| `alertIsPresent` | JS alert/confirm/prompt is open |

### Frame Conditions

| Condition | Use When |
|---|---|
| `frameToBeAvailableAndSwitchToIt` | Iframe is loaded, auto-switches to it |

### Complete List

```java
ExpectedConditions.and(
    visibilityOfElementLocated(By.id("email")),
    elementToBeClickable(By.cssSelector(".submit"))
)

ExpectedConditions.or(
    visibilityOfElementLocated(By.id("success")),
    visibilityOfElementLocated(By.id("error"))
)
```

---

## Common Wait Patterns

### Pattern 1: Wait for element, then interact

```java
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
WebElement email = wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("email")));
email.sendKeys("test@example.com");
```

### Pattern 2: Wait for clickable, then click

```java
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
WebElement submit = wait.until(ExpectedConditions.elementToBeClickable(By.cssSelector(".btn-submit")));
submit.click();
```

### Pattern 3: Wait for text to appear

```java
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
wait.until(ExpectedConditions.textToBePresentInElementLocated(By.id("status"), "Success"));
```

### Pattern 4: Wait for element to disappear (spinner)

```java
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
wait.until(ExpectedConditions.invisibilityOfElementLocated(By.cssSelector(".spinner")));
```

### Pattern 5: Wait for page load (URL change)

```java
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
wait.until(ExpectedConditions.urlToBe("https://example.com/dashboard"));
```

### Pattern 6: Wait for new window/tab

```java
String originalWindow = driver.getWindowHandle();
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
wait.until(driver -> driver.getWindowHandles().size() > 1);
for (String handle : driver.getWindowHandles()) {
    if (!handle.equals(originalWindow)) {
        driver.switchTo().window(handle);
        break;
    }
}
```

---

## StaleElementReferenceException

### What causes it

The element you found was valid, but the DOM updated (page reload, AJAX, React/Vue re-render) and the reference is now stale.

### Fix

```java
// BAD: element goes stale after page update
WebElement button = driver.findElement(By.id("submit"));
button.click(); // may throw StaleElementReferenceException

// GOOD: re-find the element after each DOM change
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
WebElement button = wait.until(ExpectedConditions.elementToBeClickable(By.id("submit")));
button.click();

// Or use staleness check
wait.until(ExpectedConditions.stalenessOf(oldElement));
WebElement newButton = wait.until(ExpectedConditions.elementToBeClickable(By.id("submit")));
newButton.click();
```

---

## SPA / Angular / React Timing

### The problem

Single-page apps load content dynamically. `driver.get()` returns before the app finishes rendering.

### Solutions

1. **Wait for a specific element** — most reliable
   ```java
   wait.until(ExpectedConditions.visibilityOfElementLocated(By.cssSelector(".app-root")));
   ```

2. **Wait for URL to stabilize** — useful for route changes
   ```java
   wait.until(ExpectedConditions.urlContains("/dashboard"));
   ```

3. **Wait for JS ready state** — last resort
   ```java
   new WebDriverWait(driver, Duration.ofSeconds(30)).until(
       webDriver -> ((JavascriptExecutor) webDriver)
           .executeScript("return document.readyState").equals("complete")
   );
   ```

4. **Wait for Angular** — if the site uses Angular
   ```java
   wait.until(webDriver -> {
       JavascriptExecutor js = (JavascriptExecutor) webDriver;
       return Boolean.parseBoolean(js.executeScript(
           "return (window.angular !== undefined) && (angular.element(document.body).injector() !== undefined) && (angular.element(document.body).injector().get('$http').pendingRequests.length === 0)"
       ).toString());
   });
   ```

---

## Page Load Strategy

```java
ChromeOptions options = new ChromeOptions();
options.setPageLoadStrategy(PageLoadStrategy.EAGER); // DOMContentLoaded
// options.setPageLoadStrategy(PageLoadStrategy.NORMAL); // full load (default)
// options.setPageLoadStrategy(PageLoadStrategy.NONE); // return immediately
```

| Strategy | When to Use |
|---|---|
| `NORMAL` | Default. Wait for full page load. |
| `EAGER` | SPAs where DOMContentLoaded is enough. Faster. |
| `NONE` | You'll handle all waits manually. Risky. |

---

## Anti-Patterns

### Thread.sleep() — **NEVER**

```java
// BAD: waits full 5 seconds even if element is ready
Thread.sleep(5000);

// GOOD: waits only as long as needed
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("email")));
```

### Fixed timeouts everywhere

```java
// BAD: hardcoded 10 seconds everywhere
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));

// GOOD: centralize timeout in BasePage
public class BasePage {
    private static final int DEFAULT_TIMEOUT = 15;
    private WebDriverWait wait;

    public BasePage(WebDriver driver) {
        this.wait = new WebDriverWait(driver, Duration.ofSeconds(DEFAULT_TIMEOUT));
    }
}
```

---

## Best Practices Summary

1. **Explicit waits only** — never mix with implicit waits.
2. **Wait for the right condition** — visibility for interaction, presence for DOM check, invisibility for spinners.
3. **Wait before interaction** — never `findElement` then `sendKeys` without a wait.
4. **Centralize timeouts** — one constant in `BasePage`.
5. **Use Page Object methods** — hide waits inside POM actions.
6. **Handle stale elements** — re-find after DOM changes.

---

## Interview Questions

1. "What's the difference between implicit and explicit waits? Why shouldn't you use both?"
2. "How do you handle a loading spinner that blocks interaction?"
3. "What causes StaleElementReferenceException and how do you fix it?"
4. "How do you wait for an element that's rendered by JavaScript after page load?"
5. "What's the page load strategy, and when would you use EAGER vs NORMAL?"
