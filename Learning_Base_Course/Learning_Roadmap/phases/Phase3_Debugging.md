# Phase 3 — Debugging & Evidence Capture

**Time:** 3–4 days  
**Course lessons:** setup / Selenium Manager, debugging, screenshots on failure

---

## Objectives

- Catch and log Selenium exceptions meaningfully in Java.
- Automatically capture screenshots on test failure via JUnit 5 `TestWatcher`.
- Add structured logging so failures are traceable without re-running.
- Generate Maven Surefire test reports.

---

## Topics

### Exception Handling

- `NoSuchElementException` — element not found in DOM.
- `TimeoutException` — explicit wait timed out.
- `ElementClickInterceptedException` — overlay or modal blocking click.
- `StaleElementReferenceException` — element detached from DOM after page update.

### Screenshot on Failure

Implement via a JUnit 5 `TestWatcher` extension in `BaseTest`:

```java
// BaseTest.java
import org.junit.jupiter.api.extension.ExtensionContext;
import org.junit.jupiter.api.extension.TestWatcher;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Optional;

public class BaseTest {
    protected WebDriver driver;

    @BeforeEach
    public void setUp() throws IOException {
        ChromeOptions chromeOptions = new ChromeOptions();
        Path tempProfile = Files.createTempDirectory("chrome-profile");
        chromeOptions.addArguments("--disable-blink-features=AutomationControlled");
        chromeOptions.addArguments("--user-data-dir=" + tempProfile.toAbsolutePath());
        driver = new ChromeDriver(chromeOptions);
        driver.manage().window().maximize();
    }

    @AfterEach
    public void tearDown() {
        if (driver != null) {
            driver.quit();
        }
    }

    public static class ScreenshotOnFailure implements TestWatcher {
        @Override
        public void testFailed(ExtensionContext context, Throwable cause) {
            Optional<Object> testInstance = context.getTestInstance();
            if (testInstance.isPresent() && ((BaseTest) testInstance.get()).driver != null) {
                WebDriver driver = ((BaseTest) testInstance.get()).driver;
                try {
                    Files.createDirectories(Paths.get("test-results"));
                    String timestamp = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMdd_HHmmss"));
                    String testName = context.getDisplayName().replaceAll("[^a-zA-Z0-9_-]", "_");
                    String screenshotPath = "test-results/" + testName + "_" + timestamp + ".png";
                    driver.getScreenshotAs(OutputType.FILE).renameTo(new File(screenshotPath));
                    logger.severe("Test failed: " + context.getDisplayName() + ". Screenshot: " + screenshotPath);
                } catch (Exception e) {
                    logger.severe("Failed to capture screenshot: " + e.getMessage());
                }
            }
        }
    }
}
```

Register the extension in tests:

```java
@ExtendWith(BaseTest.ScreenshotOnFailure.class)
public class ContactTest extends BaseTest {
    // tests here
}
```

### Maven Surefire Reports

```powershell
mvn test
```

Reports are in `target/surefire-reports/`:
- `TEST-*.xml` — JUnit XML per test class
- `surefire-reports.html` — HTML summary

---

## Practice

1. Add the `ScreenshotOnFailure` watcher above to your `BaseTest`.
2. Run a passing test — confirm no screenshot is created.
3. Run a failing test (e.g., wrong locator) — confirm a timestamped PNG drops into `test-results/`.
4. Run `mvn test` and verify the Surefire report includes the failure.
5. Add a Java logger (`java.util.logging.Logger`) to `BaseTest` and log driver start/stop events.

---

## Skills to Load

- `selenium-setup` — JUnit 5 extensions, screenshot-on-failure hooks, Java logging.

---

## Milestone

Any test in your suite that fails automatically drops a timestamped screenshot + log entry into `test-results/`. `mvn test` produces Surefire reports in `target/surefire-reports/`.
