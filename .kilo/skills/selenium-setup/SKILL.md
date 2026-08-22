---
name: selenium-setup
description: Selenium environment setup, Selenium Manager driver handling, Maven project bootstrap, JUnit 5 screenshot-on-failure hooks, and Java logging. Use when setting up a new Selenium Java project, verifying the installation, or implementing failure evidence capture.
---

# Selenium Setup (Java)

## Purpose

This skill covers Phase 1 (Environment Setup) and Phase 3 (Setup, Debugging, and Evidence Capture) of the Selenium Mastery plan. Use it to bootstrap a Maven-based Java project, verify Selenium Manager auto-resolves drivers, and build a JUnit 5 suite that captures screenshots and logs on failure.

## When to Apply

- User asks to set up Selenium in Java, install Maven dependencies, or verify their environment.
- User needs a `BaseTest` with screenshot-on-failure or logging.
- User asks about Selenium Manager, ChromeDriver, or driver version mismatches in Java.
- User asks about Maven, JUnit 5, or test report artifacts.

---

## 1. Project Bootstrap

### Prerequisites

- Java 21+ (or 11+)
- Maven 3.8+
- IntelliJ IDEA Community Edition (recommended) or VS Code with Java extensions

### Dependencies (`pom.xml`)

```xml
<?xml version="1.0" encoding="UTF-8"?>
<project xmlns="http://maven.apache.org/POM/4.0.0"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 http://maven.apache.org/xsd/maven-4.0.0.xsd">
    <modelVersion>4.0.0</modelVersion>

    <groupId>org.example</groupId>
    <artifactId>selenium-mastery</artifactId>
    <version>1.0-SNAPSHOT</version>

    <properties>
        <maven.compiler.source>21</maven.compiler.source>
        <maven.compiler.target>21</maven.compiler.target>
        <project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
        <selenium.version>4.25.0</selenium.version>
        <junit.version>5.11.1</junit.version>
    </properties>

    <dependencies>
        <dependency>
            <groupId>org.seleniumhq.selenium</groupId>
            <artifactId>selenium-java</artifactId>
            <version>${selenium.version}</version>
        </dependency>
        <dependency>
            <groupId>org.junit.jupiter</groupId>
            <artifactId>junit-jupiter</artifactId>
            <version>${junit.version}</version>
            <scope>test</scope>
        </dependency>
    </dependencies>

    <build>
        <plugins>
            <plugin>
                <groupId>org.apache.maven.plugins</groupId>
                <artifactId>maven-surefire-plugin</artifactId>
                <version>3.2.5</version>
            </plugin>
        </plugins>
    </build>
</project>
```

- `selenium-java` — core WebDriver bindings (4.6+ includes Selenium Manager).
- `junit-jupiter` — JUnit 5 test runner.
- `maven-surefire-plugin` — runs tests and generates reports.

### Recommended folder layout

```
selenium-mastery/
├── src/
│   ├── main/java/               # Page classes (POM)
│   └── test/java/               # JUnit 5 test classes
├── target/                      # Maven build output (generated)
├── test-results/                # screenshots, logs
├── pom.xml
└── .gitignore
```

### Verification script

```java
// src/test/java/VerifySetupTest.java
import org.junit.jupiter.api.Test;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class VerifySetupTest {
    @Test
    void verifyChromeLaunches() {
        WebDriver driver = new ChromeDriver();
        driver.get("https://example.com");
        String title = driver.getTitle();
        assertTrue(title.contains("Example"));
        driver.quit();
    }
}
```

Run with:

```bash
mvn test -Dtest=VerifySetupTest
```

If it runs without manual driver setup, Selenium Manager is working.

---

## 2. Selenium Manager

Selenium Manager ships with Selenium 4.6+. It auto-detects the installed browser version and downloads the matching driver binary.

### What it solves

- No manual `chromedriver` / `geckodriver` downloads.
- No version mismatch between browser and driver.

### Where it can still fail

- **Offline environments** with no cached driver binary.
- **Custom driver paths** required by corporate policies.
- **Browser not installed** or installed in a non-standard location.

### Override driver path (when needed)

```java
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;
import org.openqa.selenium.chrome.ChromeDriverService;

ChromeOptions options = new ChromeOptions();
// Selenium Manager handles this by default; only override if required
ChromeDriverService service = new ChromeDriverService.Builder()
        .usingDriverExecutable(new File("/path/to/chromedriver"))
        .build();
WebDriver driver = new ChromeDriver(service, options);
```

Prefer letting Selenium Manager handle this unless you have a specific reason to override.

---

## 3. Logging

Use Java's built-in `java.util.logging` so failures are traceable without re-running.

```java
// src/test/java/util/TestLogger.java
package util;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.logging.*;

public class TestLogger {
    private static final String LOG_DIR = "test-results";
    private static final String LOG_FILE = LOG_DIR + "/test_run.log";

    static {
        try {
            Files.createDirectories(Paths.get(LOG_DIR));
            Logger logger = Logger.getLogger("selenium");
            logger.setLevel(Level.INFO);
            FileHandler fileHandler = new FileHandler(LOG_FILE, true);
            fileHandler.setFormatter(new SimpleFormatter());
            logger.addHandler(fileHandler);
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static Logger getLogger() {
        return Logger.getLogger("selenium");
    }
}
```

In tests:

```java
import util.TestLogger;
import java.util.logging.Logger;

public class LoginTest extends BaseTest {
    private static final Logger logger = TestLogger.getLogger();

    @Test
    void testLogin() {
        logger.info("Navigating to login page");
        driver.get("https://practicesoftwaretesting.com/auth/login");
        // ...
    }
}
```

---

## 4. Screenshot on Failure

Implement via JUnit 5 `TestWatcher` extension in `BaseTest`.

```java
// src/test/java/BaseTest.java
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.extension.ExtensionContext;
import org.junit.jupiter.api.extension.TestWatcher;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;
import util.TestLogger;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.Optional;
import java.util.logging.Logger;

public class BaseTest {
    protected WebDriver driver;
    private static final Logger logger = TestLogger.getLogger();
    private static final String TEST_RESULTS_DIR = "test-results";

    @BeforeEach
    public void setUp() {
        ChromeOptions options = new ChromeOptions();
        options.addArguments("--disable-blink-features=AutomationControlled");
        driver = new ChromeDriver(options);
        driver.manage().window().maximize();
        logger.info("Browser launched");
    }

    @AfterEach
    public void tearDown() {
        if (driver != null) {
            driver.quit();
            logger.info("Browser closed");
        }
    }

    public static class ScreenshotOnFailure implements TestWatcher {
        @Override
        public void testFailed(ExtensionContext context, Throwable cause) {
            Optional<Object> testInstance = context.getTestInstance();
            if (testInstance.isPresent() && ((BaseTest) testInstance.get()).driver != null) {
                WebDriver driver = ((BaseTest) testInstance.get()).driver;
                try {
                    Files.createDirectories(Paths.get(TEST_RESULTS_DIR));
                    String timestamp = LocalDateTime.now().format(DateTimeFormatter.ofPattern("yyyyMMdd_HHmmss"));
                    String testName = context.getDisplayName().replaceAll("[^a-zA-Z0-9_-]", "_");
                    String screenshotPath = TEST_RESULTS_DIR + "/" + testName + "_" + timestamp + ".png";
                    driver.getScreenshotAs(org.openqa.selenium.OutputType.FILE)
                          .renameTo(new java.io.File(screenshotPath));
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
import org.junit.jupiter.api.extension.ExtendWith;

@ExtendWith(BaseTest.ScreenshotOnFailure.class)
public class ContactTest extends BaseTest {
    // tests here
}
```

### Result

Any failing test drops a timestamped PNG into `test-results/` and logs the error. This mirrors what the LinkedIn course demonstrates, but in pure Java/JUnit 5.

---

## 5. Maven Surefire Reports

Maven Surefire generates test reports automatically:

```bash
mvn test
```

Reports are in `target/surefire-reports/`:
- `TEST-*.xml` — JUnit XML per test class
- `surefire-reports.html` — HTML summary (if plugin configured)

For richer HTML reports with embedded screenshots, consider **ExtentReports** or **Allure** in later phases.

---

## 6. Common Pitfalls

| Pitfall | Fix |
|---|---|
| Driver version mismatch | Let Selenium Manager handle it; don't manually download drivers unless required. |
| Browser window too small for responsive elements | Call `driver.manage().window().maximize()` in `@BeforeEach`. |
| Tests leave browser processes running | Ensure `driver.quit()` is in `@AfterEach`. |
| Screenshots blank because page not loaded | Use explicit waits (see `selenium-locators-waits` skill) before screenshot. |
| Logs not appearing | Verify the `test-results/` directory exists and is writable. |
| Maven can't find tests | Ensure test class names end with `Test` or configure Surefire includes. |

---

## 7. Milestone

- `mvn test -Dtest=VerifySetupTest` runs successfully with no manual driver configuration.
- Every failing test in `src/test/java/` auto-drops a timestamped screenshot into `test-results/`.
- `test-results/test_run.log` captures test execution events.
- `mvn test` produces Surefire reports in `target/surefire-reports/`.
