---
name: selenium-locators-waits
description: Selenium WebDriver locators (id, name, CSS, XPath, link text, class name), waits (implicit, explicit with WebDriverWait + ExpectedConditions), Actions API (clicks, form fills, Actions class, alerts, windows, iframes). Use when writing interaction scripts, debugging flaky locators, or building the Phase 2 basics/ folder in Java.
---

# Selenium Locators, Actions, and Waits (Java)

## Purpose

This skill covers Phase 2 (WebDriver Core) of the Selenium Mastery plan. It teaches how to reliably find elements, interact with them, and handle timing issues without `Thread.sleep()`. Use it when writing low-level Selenium scripts in Java, debugging flaky tests, or translating Playwright/Katalon interaction patterns into Selenium.

## When to Apply

- User asks how to find an element, write a locator, or choose between CSS and XPath.
- User asks about waits, `WebDriverWait`, `ExpectedConditions`, or `implicitlyWait`.
- User asks about clicks, form fills, dropdowns, `Actions` class, alerts, tabs, iframes.
- User asks why a test is flaky or timing-dependent.

---

## 1. Locator Strategies

### Priority order

1. `id` — fastest, most stable.
2. `name` — stable if names are unique and semantic.
3. `css selector` — fast, readable, supports class/attribute/structural patterns.
4. `xpath` — most powerful, supports text/structure, but slower and more brittle.
5. `link text` / `partial link text` — only for `<a>` tags.
6. `class name` — useful when class is unique; avoid if class is shared by many elements.

### Writing locators

```java
import org.openqa.selenium.By;

driver.findElement(By.id("username"));
driver.findElement(By.name("email"));
driver.findElement(By.cssSelector("button[type='submit']"));
driver.findElement(By.xpath("//button[contains(text(), 'Submit')]"));
driver.findElement(By.linkText("Forgot password?"));
driver.findElement(By.className("btn-primary"));
```

### CSS vs XPath for the same element

```java
// Example: primary submit button
WebElement cssButton = driver.findElement(By.cssSelector("form > button.btn-primary"));
WebElement xpathButton = driver.findElement(By.xpath("//form/button[contains(@class, 'btn-primary')]"));
```

Practice writing both for every element in your Phase 2 scripts. You will need both in real codebases.

### Locator hygiene

- Avoid absolute XPath (`/html/body/div[2]/div[1]`) — it breaks on any DOM change.
- Prefer relative XPath (`//button[contains(text(), 'Submit')]`) or CSS selectors.
- Group locators at the top of the page class as `By` constants.

---

## 2. Waits

### Never use `Thread.sleep()`

`Thread.sleep()` pauses for a fixed duration regardless of whether the element is ready. It makes tests slow and flaky.

### Implicit wait (global, avoid mixing with explicit)

```java
driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(10));
```

- Tells WebDriver to poll the DOM for up to 10 seconds when finding elements.
- **Do not mix** implicit and explicit waits — it causes unpredictable timeout math.

### Explicit wait (recommended)

```java
import org.openqa.selenium.support.ui.WebDriverWait;
import org.openqa.selenium.support.ui.ExpectedConditions;
import java.time.Duration;

WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));

WebElement element = wait.until(
    ExpectedConditions.presenceOfElementLocated(By.id("username"))
);
```

### Common ExpectedConditions

| Condition | Use case |
|---|---|
| `presenceOfElementLocated` | Element exists in DOM (not necessarily visible). |
| `visibilityOfElementLocated` | Element exists and has non-zero size (visible). |
| `elementToBeClickable` | Visible and enabled (ready for click). |
| `textToBePresentInElement` | Wait for dynamic text. |
| `alertIsPresent` | Wait for a JavaScript alert. |

```java
// Wait for clickable
wait.until(ExpectedConditions.elementToBeClickable(By.cssSelector("button[type='submit']"))).click();

// Wait for text
wait.until(ExpectedConditions.textToBePresentInElementLocated(By.id("status"), "Success"));
```

### Flakiness guardrails

- Always wait for the **condition you actually need** (clickable, visible), not just presence.
- If an element is still not found, check for iframes, shadow DOM, or timing issues before increasing the timeout.

---

## 3. Actions API

### Clicks and form fills

```java
import org.openqa.selenium.By;

WebElement username = driver.findElement(By.id("username"));
username.clear();
username.sendKeys("admin");

driver.findElement(By.cssSelector("button[type='submit']")).click();
```

- `clear()` before `sendKeys()` to ensure no residual text.
- Use `click()` only after `elementToBeClickable`.

### Dropdowns

```java
import org.openqa.selenium.support.ui.Select;

WebElement dropdown = driver.findElement(By.id("country"));
Select select = new Select(dropdown);
select.selectByVisibleText("United States");
// or
select.selectByValue("US");
```

### Actions class (hover, drag, key combos)

```java
import org.openqa.selenium.interactions.Actions;

Actions actions = new Actions(driver);

// Hover
actions.moveToElement(driver.findElement(By.id("menu"))).perform();

// Drag and drop
WebElement source = driver.findElement(By.id("draggable"));
WebElement target = driver.findElement(By.id("droppable"));
actions.dragAndDrop(source, target).perform();
```

### Alerts

```java
Alert alert = driver.switchTo().alert();
alert.accept();      // OK
// or
alert.dismiss();     // Cancel
// or
String alertText = alert.getText();
```

### Window / tab switching

```java
String mainWindow = driver.getWindowHandle();
driver.findElement(By.id("open-new-tab")).click();

for (String handle : driver.getWindowHandles()) {
    if (!handle.equals(mainWindow)) {
        driver.switchTo().window(handle);
        break;
    }
}

// Switch back
driver.switchTo().window(mainWindow);
```

### Iframes

```java
// Switch to iframe by locator
driver.switchTo().frame(driver.findElement(By.tagName("iframe")));

// Interact with elements inside iframe
driver.findElement(By.id("iframe-input")).sendKeys("hello");

// Switch back to default content
driver.switchTo().defaultContent();
```

---

## 4. Phase 2 Script Template

Each test class should follow this shape:

```java
// src/test/java/ContactFormTest.java
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import java.time.Duration;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class ContactFormTest extends BaseTest {
    @Test
    void testContactFormSubmit() {
        driver.get("https://practicesoftwaretesting.com/contact");

        WebElement firstName = driver.findElement(By.id("first_name"));
        WebElement lastName = driver.findElement(By.id("last_name"));
        WebElement email = driver.findElement(By.id("email"));
        WebElement subject = driver.findElement(By.id("subject"));
        WebElement message = driver.findElement(By.id("message"));
        WebElement btnSubmit = driver.findElement(By.className("btnSubmit"));

        firstName.sendKeys("John");
        lastName.sendKeys("Smith");
        email.sendKeys("john.smith@example.com");

        Select subjectSelect = new Select(subject);
        subjectSelect.selectByValue("webmaster");

        message.sendKeys("Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
        btnSubmit.click();

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
        WebElement alert = wait.until(
            ExpectedConditions.visibilityOfElementLocated(By.className("alert"))
        );
        assertEquals("Thanks for your message! We will contact you shortly.", alert.getText().trim());
    }
}
```

---

## 5. Common Pitfalls

| Pitfall | Fix |
|---|---|
| `Thread.sleep()` used instead of waits | Replace with `WebDriverWait` + `ExpectedConditions`. |
| `NoSuchElementException` after page loads | Wait for `presenceOfElementLocated` or `visibilityOfElementLocated`. |
| Click intercepted / not clickable | Wait for `elementToBeClickable`; ensure no overlay covers the element. |
| StaleElementReferenceException | Re-locate the element after a page update; don't cache elements across waits. |
| Switching to wrong iframe | Verify you're in the correct iframe before interacting; switch back with `defaultContent()`. |

---

## 6. Milestone

A `src/test/java/` folder with 5–8 test classes, each covering one interaction type (login, form fill, dropdown, hover, drag, alert, window switch, iframe). Every test uses JUnit 5 `assertEquals`/`assertTrue` for pass/fail and explicit waits instead of `Thread.sleep()`.
