# Phase 2 — WebDriver Core: Locators, Actions, Waits

**Time:** 4–5 days

---

## Objectives

- Master Selenium locator strategies in Java.
- Use explicit waits instead of `Thread.sleep()`.
- Perform common user interactions (clicks, form fills, dropdowns, hover, drag, alerts, tabs, iframes).
- Build a `src/test/java/` folder with standalone test classes for each interaction type.

---

## Topics

### Locators

- `By.id`, `By.name`, `By.cssSelector`, `By.xpath`, `By.linkText`, `By.className`.
- Priority order and when to use each.
- Writing both CSS and XPath for the same element.
- Locator hygiene: avoid absolute XPath, prefer relative selectors.

### Waits

- `driver.manage().timeouts().implicitlyWait()` (avoid mixing with explicit waits).
- `WebDriverWait` + `ExpectedConditions` (explicit waits).
- Common conditions: `presenceOfElementLocated`, `visibilityOfElementLocated`, `elementToBeClickable`, `textToBePresentInElementLocated`, `alertIsPresent`.
- **Never use `Thread.sleep()`** — build this discipline now.

### Actions API

- Clicks and form fills (`clear()`, `sendKeys()`).
- Dropdowns (`Select` class).
- `Actions` class for hover, drag-and-drop, key combos.
- Alert handling (`accept()`, `dismiss()`, `getText()`).
- Window/tab switching (`getWindowHandles()`).
- Iframe switching (`switchTo().frame()`, `switchTo().defaultContent()`).

---

## Practice

Build 5–8 small test classes in `src/test/java/`. Each should extend `BaseTest` and use JUnit 5 assertions.

**Important:** The course target site `practicesoftwaretesting.com` is a modern Angular SPA. Tests must:
- Run in **headed mode** (no `--headless=new`).
- Use **explicit waits** for every element interaction.
- Use a **temp user-data-dir** to avoid profile lock issues.

```java
// src/test/java/ContactFormTest.java
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.openqa.selenium.support.ui.Select;
import java.time.Duration;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class ContactFormTest extends BaseTest {
    @Test
    void testContactFormSubmit() {
        driver.get("https://practicesoftwaretesting.com/contact");

        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));

        WebElement firstName = wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("first_name")));
        WebElement lastName = wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("last_name")));
        WebElement email = wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("email")));
        WebElement subject = wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("subject")));
        WebElement message = wait.until(ExpectedConditions.visibilityOfElementLocated(By.id("message")));
        WebElement btnSubmit = wait.until(ExpectedConditions.elementToBeClickable(By.className("btnSubmit")));

        firstName.sendKeys("John");
        lastName.sendKeys("Smith");
        email.sendKeys("john.smith@example.com");

        Select subjectSelect = new Select(subject);
        subjectSelect.selectByValue("webmaster");

        message.sendKeys("Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
        btnSubmit.click();

        WebElement alert = wait.until(
            ExpectedConditions.visibilityOfElementLocated(By.className("alert"))
        );
        assertEquals("Thanks for your message! We will contact you shortly.", alert.getText().trim());
    }
}
```

### Test classes to build

1. **Contact form submit** — fill + submit + assert success message
2. **Login form** — enter credentials, submit, assert redirect or error
3. **Dropdown selection** — select by value, index, visible text
4. **Hover menu** — hover over nav item, click submenu
5. **Drag and drop** — drag element to drop zone
6. **JavaScript alert** — trigger alert, accept/dismiss, assert text
7. **New tab** — open link in new tab, switch, assert content
8. **Iframe interaction** — switch to iframe, interact, switch back

---

## Skills to Load

- `selenium-locators-waits` — locators, waits, Actions API, alerts, iframes, window switching.

---

## Milestone

A `src/test/java/` folder with 5–8 test classes, each covering one interaction type. Every test uses JUnit 5 assertions for pass/fail and explicit waits instead of `Thread.sleep()`. All tests pass with `mvn test`.
