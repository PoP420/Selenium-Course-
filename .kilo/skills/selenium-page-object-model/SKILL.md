---
name: selenium-page-object-model
description: Page Object Model (POM) patterns for Selenium in Java — BasePage class, page classes with locators and actions, no assertions inside page objects, and optional Cucumber BDD layer. Use when refactoring scripts into POM, designing a test framework, or adding Gherkin feature files with Java step definitions.
---

# Selenium Page Object Model (POM) — Java

## Purpose

This skill covers Phase 4 (Page Object Model) of the Selenium Mastery plan. It teaches how to structure a maintainable Selenium framework using POM in Java, refactor procedural scripts into reusable page classes, and optionally layer Cucumber (Java's Gherkin BDD) on top. Use it when building or refactoring a Selenium test suite, designing a `BasePage`, or integrating BDD.

## When to Apply

- User asks how to structure a Selenium test framework or refactor into POM in Java.
- User asks about `BasePage`, page classes, or where assertions belong.
- User asks about Cucumber, `.feature` files, or Java step definitions.
- User asks how to avoid duplicated locators or wait logic across tests.

---

## 1. POM Principles

- **One class per page/screen.**
- **Locators as `By` class attributes.**
- **Actions as methods** (e.g., `login(String username, String password)`).
- **No assertions inside page objects.** Assertions stay in test classes.
- **Inheritance from `BasePage`** for shared waits/helpers.

### Why this matters

- Locators change in one place, not scattered across tests.
- Business logic (e.g., "checkout flow") reads like English in the test class.
- Page classes become reusable across multiple test suites.

---

## 2. BasePage Class

```java
// src/main/java/pages/BasePage.java
package pages;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.By;
import java.time.Duration;

public class BasePage {
    protected WebDriver driver;
    private WebDriverWait wait;
    private static final int DEFAULT_TIMEOUT = 10;

    public BasePage(WebDriver driver) {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, Duration.ofSeconds(DEFAULT_TIMEOUT));
    }

    public BasePage(WebDriver driver, int timeoutSeconds) {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, Duration.ofSeconds(timeoutSeconds));
    }

    protected WebElement find(By locator) {
        return wait.until(ExpectedConditions.presenceOfElementLocated(locator));
    }

    protected void click(By locator) {
        wait.until(ExpectedConditions.elementToBeClickable(locator)).click();
    }

    protected void typeText(By locator, String text) {
        WebElement element = wait.until(ExpectedConditions.visibilityOfElementLocated(locator));
        element.clear();
        element.sendKeys(text);
    }

    protected String getText(By locator) {
        return wait.until(ExpectedConditions.visibilityOfElementLocated(locator)).getText();
    }

    protected boolean isDisplayed(By locator) {
        try {
            return wait.until(ExpectedConditions.visibilityOfElementLocated(locator)).isDisplayed();
        } catch (Exception e) {
            return false;
        }
    }
}
```

---

## 3. Page Class Example

```java
// src/main/java/pages/LoginPage.java
package pages;

import org.openqa.selenium.By;

public class LoginPage extends BasePage {
    // Locators
    private final By emailLoc = By.id("email");
    private final By passwordLoc = By.id("password");
    private final By btnSubmitLoc = By.className("btnSubmit");

    public LoginPage(org.openqa.selenium.WebDriver driver) {
        super(driver);
    }

    // Actions
    public void loginAs(String username, String password) {
        typeText(emailLoc, username);
        typeText(passwordLoc, password);
        click(btnSubmitLoc);
    }
}
```

```java
// src/main/java/pages/ContactPage.java
package pages;

import org.openqa.selenium.By;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.Select;

public class ContactPage extends BasePage {
    private final By firstNameLoc = By.id("first_name");
    private final By lastNameLoc = By.id("last_name");
    private final By emailLoc = By.id("email");
    private final By subjectLoc = By.id("subject");
    private final By messageLoc = By.id("message");
    private final By btnSubmitLoc = By.className("btnSubmit");
    private final By alertLoc = By.className("alert");

    public ContactPage(org.openqa.selenium.WebDriver driver) {
        super(driver);
    }

    public void fillForm(String firstName, String lastName, String email, String message) {
        typeText(firstNameLoc, firstName);
        typeText(lastNameLoc, lastName);
        typeText(emailLoc, email);

        WebElement subjectDropdown = driver.findElement(subjectLoc);
        Select select = new Select(subjectDropdown);
        select.selectByValue("webmaster");

        typeText(messageLoc, message);
    }

    public void submitForm() {
        click(btnSubmitLoc);
    }

    public String getAlertText() {
        return getText(alertLoc);
    }
}
```

---

## 4. Test Class (Assertions Here)

```java
// src/test/java/ContactTest.java
import org.junit.jupiter.api.Test;
import pages.ContactPage;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class ContactTest extends BaseTest {
    @Test
    void testContactFormSubmit() {
        driver.get("https://practicesoftwaretesting.com/contact");

        ContactPage contactPage = new ContactPage(driver);
        contactPage.fillForm("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
        contactPage.submitForm();

        String alertText = contactPage.getAlertText();
        assertEquals("Thanks for your message! We will contact you shortly.", alertText.trim());
    }
}
```

---

## 5. Refactoring from Phase 2

Take every test from Phase 2 and convert it:

1. Create a `pages/` package under `src/main/java/`.
2. Create a `BasePage` with shared wait/click/type/getText helpers.
3. Create one page class per page/screen.
4. Move locators and actions into the page class.
5. Keep only assertions and test flow in `src/test/java/`.

This refactor is the actual learning — don't skip it.

---

## 6. Optional: Cucumber (BDD) Layer

Given your Cucumber/Gherkin background, add Cucumber Java to write feature files that drive the same page objects.

### Dependencies (`pom.xml`)

```xml
<dependency>
    <groupId>io.cucumber</groupId>
    <artifactId>cucumber-java</artifactId>
    <version>7.15.0</version>
    <scope>test</scope>
</dependency>
<dependency>
    <groupId>io.cucumber</groupId>
    <artifactId>cucumber-junit</artifactId>
    <version>7.15.0</version>
    <scope>test</scope>
</dependency>
```

### Directory structure

```
src/test/java/
├── features/
│   └── contact.feature
└── steps/
    └── ContactSteps.java
```

### Feature file

```gherkin
# src/test/java/features/contact.feature
Feature: Contact form

  Scenario: Successful contact form submission
    Given I am on the contact page
    When I fill the contact form with valid data
    And I submit the form
    Then I should see "Thanks for your message! We will contact you shortly."
```

### Step definitions

```java
// src/test/java/steps/ContactSteps.java
package steps;

import io.cucumber.java.en.Given;
import io.cucumber.java.en.When;
import io.cucumber.java.en.Then;
import pages.ContactPage;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class ContactSteps {
    private ContactPage contactPage;

    @Given("I am on the contact page")
    public void iAmOnTheContactPage() {
        driver.get("https://practicesoftwaretesting.com/contact");
        contactPage = new ContactPage(driver);
    }

    @When("I fill the contact form with valid data")
    public void iFillTheContactFormWithValidData() {
        contactPage.fillForm("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
    }

    @When("I submit the form")
    public void iSubmitTheForm() {
        contactPage.submitForm();
    }

    @Then("I should see {string}")
    public void iShouldSee(String message) {
        assertEquals(message, contactPage.getAlertText().trim());
    }
}
```

---

## 7. Data-Driven Tests with JUnit 5

Use `@ParameterizedTest` with `@CsvFileSource` to load test data from CSV files.

```java
// src/test/java/LoginTest.java
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvFileSource;
import org.junit.jupiter.api.Tag;
import pages.LoginPage;
import pages.AccountPage;
import static org.junit.jupiter.api.Assertions.assertEquals;

public class LoginTest extends BaseTest {
    @ParameterizedTest
    @Tag("smoke")
    @CsvFileSource(resources = "/login-data.csv", numLinesToSkip = 0)
    void testLoginWithMultipleUsers(String username, String password) {
        driver.get("https://practicesoftwaretesting.com/auth/login");

        LoginPage loginPage = new LoginPage(driver);
        loginPage.loginAs(username, password);

        AccountPage accountPage = new AccountPage(driver);
        assertEquals("My account", accountPage.getPageTitleText());
    }
}
```

Place `login-data.csv` in `src/test/resources/`.

---

## 8. Multi-Page Flow Example

```java
// src/test/java/CheckoutTest.java
public class CheckoutTest extends BaseTest {
    @Test
    void testLoginSearchAddToCartCheckout() {
        driver.get("https://practicesoftwaretesting.com/auth/login");
        LoginPage loginPage = new LoginPage(driver);
        loginPage.loginAs("customer", "password");

        // Continue through InventoryPage, CartPage, CheckoutPage...
    }
}
```

Each page transition initializes the next page object, keeping the test readable.

---

## 9. Common Pitfalls

| Pitfall | Fix |
|---|---|
| Assertions inside page objects | Move assertions to test classes. Page objects return data; tests assert. |
| Locators duplicated | Define locators as `By` class attributes on the page object only. |
| Driver not passed to page objects | Pass `driver` in `super(driver)` via `BasePage`. |
| Page objects know about other page objects | They can, but keep coupling loose; prefer returning the next page object from an action method. |
| Hardcoded waits inside page objects | Use `BasePage`'s `WebDriverWait` for all waits. |

---

## 10. Milestone

- `pages/BasePage.java` with shared wait/click/type/getText helpers.
- 3+ page classes (e.g., `LoginPage`, `ContactPage`, `AccountPage`).
- `src/test/java/` contains only assertions and flow logic.
- A 3+ page test suite runs with a single `mvn test` command.
- Optional: Cucumber feature files driving the same page objects.
