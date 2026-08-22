---
name: selenium-parallel-grid
description: Parallel test execution with JUnit 5 parallel config and Selenium Grid (local Docker setup, RemoteWebDriver, cross-browser testing). Use when scaling test suites, running tests in parallel, or setting up Selenium Grid locally with Java/Maven.
---

# Selenium Parallel Execution & Selenium Grid (Java)

## Purpose

This skill covers Phase 5 (Scaling: Parallel Execution & Selenium Grid) of the Selenium Mastery plan. It teaches how to run JUnit 5 suites in parallel and how to point tests at a local Selenium Grid via Docker for cross-browser execution. Use it when the user wants to speed up their suite, run cross-browser tests, or set up a Selenium Grid with Java.

## When to Apply

- User asks how to run tests in parallel in Java/Maven.
- User asks about JUnit 5 parallel execution, Maven Surefire parallel config.
- User asks how to set up Selenium Grid, Docker containers, or `RemoteWebDriver`.
- User asks about cross-browser testing (Chrome + Firefox) or hub/node architecture.

---

## 1. Parallel Execution with JUnit 5 + Maven Surefire

### JUnit 5 Parallel Config

Add to `pom.xml`:

```xml
<properties>
    <junit.jupiter.execution.parallel.enabled>true</junit.jupiter.execution.parallel.enabled>
    <junit.jupiter.execution.parallel.mode.default>concurrent</junit.jupiter.execution.parallel.mode.default>
    <junit.jupiter.execution.parallel.config.fixed.parallelism>4</junit.jupiter.execution.parallel.config.fixed.parallelism>
</properties>
```

Or use `junit-platform.properties` in `src/test/resources/`:

```
junit.jupiter.execution.parallel.enabled = true
junit.jupiter.execution.parallel.mode.default = concurrent
junit.jupiter.execution.parallel.config.fixed.parallelism = 4
```

- `parallel.enabled = true` turns on parallel execution.
- `mode.default = concurrent` runs all tests in parallel (use `same_thread` for sequential classes).
- `fixed.parallelism = 4` limits to 4 concurrent threads.

### Fixture scope matters

Use `@TestInstance(Lifecycle.PER_METHOD)` (default) so each test gets a fresh driver:

```java
// BaseTest.java
@BeforeEach
public void setUp() {
    driver = new ChromeDriver();
    driver.manage().window().maximize();
}

@AfterEach
public void tearDown() {
    if (driver != null) {
        driver.quit();
    }
}
```

### Measuring speedup

```bash
# Sequential baseline
mvn test

# Parallel
mvn test -Djunit.jupiter.execution.parallel.enabled=true
```

Record execution time before and after to prove the optimization.

---

## 2. Selenium Grid with Docker

### Prerequisites

- Docker Desktop installed (free, one-time download with internet).
- Internet for the initial `docker pull`, then offline-capable.

### Start a standalone Grid (quickest)

```bash
docker run -d -p 4444:4444 --shm-size=2g selenium/standalone-chrome
```

- `-d` runs detached.
- `-p 4444:4444` exposes the Grid console and WebDriver port.
- `--shm-size=2g` prevents Chrome crashes inside Docker.

### Start a full hub + node setup

```bash
# Hub
docker run -d -p 4444:4444 --shm-size=2g selenium/hub:latest

# Chrome node
docker run -d -p 5555:5554 --shm-size=2g \
  -e SE_EVENT_BUS_HOST=host.docker.internal \
  -e SE_EVENT_BUS_PUBLISH_PORT=4442 \
  -e SE_EVENT_BUS_SUBSCRIBE_PORT=4443 \
  selenium/node-chrome:latest

# Firefox node
docker run -d -p 5556:5554 --shm-size=2g \
  -e SE_EVENT_BUS_HOST=host.docker.internal \
  -e SE_EVENT_BUS_PUBLISH_PORT=4442 \
  -e SE_EVENT_BUS_SUBSCRIBE_PORT=4443 \
  selenium/node-firefox:latest
```

### Grid console

Open `http://localhost:4444/ui` to see registered nodes and active sessions.

---

## 3. Pointing Tests at the Grid

Replace `new ChromeDriver()` with `new RemoteWebDriver()`.

```java
// src/test/java/BaseTest.java
import org.openqa.selenium.remote.RemoteWebDriver;
import org.openqa.selenium.remote.DesiredCapabilities;
import org.openqa.selenium.chrome.ChromeOptions;
import org.openqa.selenium.firefox.FirefoxOptions;
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

### Cross-browser parametrization

```java
// src/test/java/CrossBrowserTest.java
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.ValueSource;

public class CrossBrowserTest extends BaseTest {
    @ParameterizedTest
    @ValueSource(strings = {"chrome", "firefox"})
    void testLoginCrossBrowser(String browser) {
        System.setProperty("browser", browser);
        // Re-initialize driver with browser param (simplified)
        // Same test runs on both browsers via Grid
    }
}
```

> **Note:** `DesiredCapabilities` is deprecated in newer Selenium versions in favor of passing `Options` objects directly to `RemoteWebDriver()`. Use `ChromeOptions` or `FirefoxOptions` as shown above.

---

## 4. Docker Compose (Optional)

Save as `docker-compose.yml` for easier management:

```yaml
version: "3"
services:
  selenium-hub:
    image: selenium/hub:latest
    container_name: selenium-hub
    ports:
      - "4444:4444"
    environment:
      - SE_EVENT_BUS_HOST=selenium-hub

  chrome:
    image: selenium/node-chrome:latest
    shm_size: 2g
    depends_on:
      - selenium-hub
    environment:
      - SE_EVENT_BUS_HOST=selenium-hub
      - SE_EVENT_BUS_PUBLISH_PORT=4442
      - SE_EVENT_BUS_SUBSCRIBE_PORT=4443
      - SE_NODE_MAX_SESSIONS=4
      - SE_NODE_OVERRIDE_MAX_SESSIONS=true

  firefox:
    image: selenium/node-firefox:latest
    shm_size: 2g
    depends_on:
      - selenium-hub
    environment:
      - SE_EVENT_BUS_HOST=selenium-hub
      - SE_EVENT_BUS_PUBLISH_PORT=4442
      - SE_EVENT_BUS_SUBSCRIBE_PORT=4443
      - SE_NODE_MAX_SESSIONS=4
      - SE_NODE_OVERRIDE_MAX_SESSIONS=true
```

```bash
docker-compose up -d
docker-compose down
```

---

## 5. Local Parallel + Grid Together

You can run JUnit 5 parallel execution against the Grid for maximum throughput:

```bash
mvn test -Dbrowser=chrome
```

This parallelizes tests **and** distributes them across Grid nodes.

### Document before/after

Record execution time for each configuration:

| Configuration | Time |
|---|---|
| Sequential, local | Xs |
| Parallel (JUnit 5), local | Ys |
| Parallel, Grid (Chrome + Firefox) | Zs |

This is the Phase 5 milestone.

---

## 6. Common Pitfalls

| Pitfall | Fix |
|---|---|
| Chrome crashes inside Docker | Add `--shm-size=2g` to `docker run`. |
| Port already in use | Stop existing containers with `docker ps` + `docker stop`. |
| `SessionNotCreatedException` on Grid | Verify node browser version matches your test expectations. |
| Parallel tests overwrite screenshots | Include test name + timestamp in screenshot filenames. |
| Fixture scope too wide | Keep driver setup in `@BeforeEach` for parallel safety. |

---

## 7. Milestone

- JUnit 5 parallel config runs the suite with documented speedup.
- Selenium Grid runs locally in Docker with Chrome and Firefox nodes.
- Same suite runs against the Grid with cross-browser parametrization.
- A table comparing sequential, parallel-local, and parallel-Grid execution times.
