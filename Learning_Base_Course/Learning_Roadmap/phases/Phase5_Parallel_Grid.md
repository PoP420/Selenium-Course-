# Phase 5 — Parallel Execution & Selenium Grid

**Time:** 4–5 days  
**Course lessons:** [Run tests in parallel](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/run-tests-in-parallel), [Test across browsers locally](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/test-across-browsers-locally)

---

## Objectives

- Speed up the test suite with JUnit 5 parallel execution.
- Set up a local Selenium Grid with Docker.
- Run the same suite cross-browser (Chrome + Firefox) via Grid.
- Measure and document speedup.

---

## Topics

### Parallel Execution with JUnit 5 + Maven Surefire

- Add parallel config to `junit-platform.properties`.
- Run with `mvn test`.
- Fixture scope matters: keep driver setup in `@BeforeEach` for parallel safety.
- Measure sequential vs. parallel execution time.

### Selenium Grid with Docker

- Start a standalone Grid: `docker run -d -p 4444:4444 --shm-size=2g selenium/standalone-chrome`.
- Start a full hub + node setup with Chrome and Firefox nodes.
- Access the Grid console at `http://localhost:4444/ui`.

### Pointing Tests at the Grid

- Replace `new ChromeDriver()` with `new RemoteWebDriver()`.
- Configure `ChromeOptions` or `FirefoxOptions` directly.
- Parametrize tests for cross-browser execution (`chrome`, `firefox`).

---

## Practice

1. Run your Phase 4 suite sequentially and record the time.
2. Enable JUnit 5 parallel config and record the speedup.
3. Start the Selenium Grid in Docker.
4. Update your `BaseTest` to switch between local and Grid based on a flag or system property.
5. Run the same suite against the Grid with cross-browser parametrization.

```java
// BaseTest.java — Grid-aware setup
import org.openqa.selenium.remote.RemoteWebDriver;
import org.openqa.selenium.remote.DesiredCapabilities;
import java.net.URL;

public class BaseTest {
    protected WebDriver driver;
    private static final String GRID_URL = "http://localhost:4444/wd/hub";
    private static final boolean USE_GRID = true;

    @BeforeEach
    public void setUp() throws Exception {
        if (USE_GRID) {
            String browser = System.getProperty("browser", "chrome");
            if ("firefox".equalsIgnoreCase(browser)) {
                FirefoxOptions options = new FirefoxOptions();
                driver = new RemoteWebDriver(new URL(GRID_URL), options);
            } else {
                ChromeOptions options = new ChromeOptions();
                driver = new RemoteWebDriver(new URL(GRID_URL), options);
            }
        } else {
            ChromeOptions options = new ChromeOptions();
            driver = new ChromeDriver(options);
        }
        driver.manage().window().maximize();
    }

    @AfterEach
    public void tearDown() {
        if (driver != null) {
            driver.quit();
        }
    }
}
```

```java
// CrossBrowserTest.java
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

public class CrossBrowserTest extends BaseTest {
    @ParameterizedTest
    @ValueSource(strings = {"chrome", "firefox"})
    void testLoginCrossBrowser(String browser) {
        System.setProperty("browser", browser);
        // Same test logic runs on both browsers via Grid
    }
}
```

---

## Skills to Load

- `selenium-parallel-grid` — JUnit 5 parallel config, Selenium Grid with Docker, cross-browser testing.

---

## Milestone

| Configuration | Time |
|---|---|
| Sequential, local | ___s |
| Parallel (JUnit 5), local | ___s |
| Parallel, Grid (Chrome + Firefox) | ___s |

Document the before/after execution time in a table. Show the speedup percentage.
