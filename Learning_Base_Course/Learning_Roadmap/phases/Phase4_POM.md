# Phase 4 — Page Object Model (POM)

**Time:** 5–6 days  
**Course lesson:** [Implement a POM in a test](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/implement-page-object-model-in-test)

---

## Objectives

- Understand POM principles and why they matter for maintainability in Java.
- Build a `BasePage` class with shared wait/click/type helpers.
- Refactor all Phase 2–3 test classes into POM classes.
- Optional: layer Cucumber (Gherkin) on top to write feature files driving the same page objects.

---

## POM Principles

- **One class per page/screen.**
- **Locators as `By` class attributes.**
- **Actions as methods** (e.g., `loginAs(String username, String password)`).
- **No assertions inside page objects.** Assertions stay in test classes.
- **Inheritance from `BasePage`** for shared waits/helpers.

---

## BasePage Class

```java
// src/main/java/pages/BasePage.java
package pages;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
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
}
```

---

## Page Class Example

```java
// src/main/java/pages/ContactPage.java
package pages;

import org.openqa.selenium.By;
import org.openqa.selenium.support.ui.Select;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.openqa.selenium.support.ui.ExpectedConditions;
import java.time.Duration;

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
        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
        wait.until(ExpectedConditions.visibilityOfElementLocated(firstNameLoc)).sendKeys(firstName);
        wait.until(ExpectedConditions.visibilityOfElementLocated(lastNameLoc)).sendKeys(lastName);
        wait.until(ExpectedConditions.visibilityOfElementLocated(emailLoc)).sendKeys(email);

        WebElement subjectElement = wait.until(ExpectedConditions.visibilityOfElementLocated(subjectLoc));
        Select subjectSelect = new Select(subjectElement);
        subjectSelect.selectByValue("webmaster");

        wait.until(ExpectedConditions.visibilityOfElementLocated(messageLoc)).sendKeys(message);
    }

    public void submitForm() {
        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
        wait.until(ExpectedConditions.elementToBeClickable(btnSubmitLoc)).click();
    }

    public String getAlertText() {
        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(15));
        return wait.until(ExpectedConditions.visibilityOfElementLocated(alertLoc)).getText();
    }
}
```

---

## Test Class (Assertions Here)

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

        assertEquals("Thanks for your message! We will contact you shortly.", contactPage.getAlertText().trim());
    }
}
```

---

## Optional: Cucumber (BDD) Layer

Given your Cucumber/Gherkin background, add Cucumber Java to write feature files that drive the same page objects.

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

## Practice

1. Refactor all Phase 2–3 test classes into `src/main/java/pages/` classes.
2. Create `BasePage` with shared helpers.
3. Write 3+ page classes (e.g., `LoginPage`, `ContactPage`, `AccountPage`).
4. Write tests in `src/test/java/` that read like business flows.
5. Optional: add Cucumber feature files for the core user journey.

---

## Skills to Load

- `selenium-page-object-model` — POM design, BasePage, page classes, Cucumber BDD.

---

## Milestone

- `pages/BasePage.java` with shared wait/click/type/getText helpers.
- 3+ page classes.
- `src/test/java/` contains only assertions and flow logic.
- A 3+ page test suite runs with a single `mvn test` command.
- Optional: Cucumber feature files driving the same page objects.
