# Phase 0 — Environment Setup

**Time:** Day 0 (1 session)

---

## Objectives

- Install Java 21+, Maven, IntelliJ IDEA Community Edition, and Git.
- Verify Selenium Manager auto-resolves browser drivers.
- Create a working Maven project with Selenium + JUnit 5.
- Run your first passing test with zero manual driver configuration.

---

## Verified Setup (Windows 11)

### Java

Check Java is installed:
```powershell
java --version
javac --version
```
If not, install Eclipse Temurin JDK 21 from [adoptium.net](https://adoptium.net).

### Maven

Chocolatey may fail due to permissions. Download Maven directly:
1. Download `apache-maven-3.9.9-bin.zip` from [archive.apache.org](https://archive.apache.org/dist/maven/maven-3/3.9.9/binaries/apache-maven-3.9.9-bin.zip)
2. Extract to `C:\Users\ajdpe\apache-maven-3.9.9\`
3. Add to System PATH: `C:\Users\ajdpe\apache-maven-3.9.9\apache-maven-3.9.9\bin`
4. Verify:
   ```powershell
   mvn --version
   ```

### Browsers

Install Google Chrome from [google.com/chrome](https://www.google.com/chrome/). Selenium Manager will auto-resolve the matching ChromeDriver.

---

## Project Setup

### Option A: Use the course repo

```powershell
git clone https://github.com/LinkedInLearning/learning-selenium-structure-scale-run-and-optimize-automated-tests-5989088.git
cd learning-selenium-structure-scale-run-and-optimize-automated-tests-5989088
mvn test
```

### Option B: Create your own project

```powershell
mkdir selenium-mastery
cd selenium-mastery
```

Create `pom.xml`:

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
    </properties>
    <dependencies>
        <dependency>
            <groupId>org.seleniumhq.selenium</groupId>
            <artifactId>selenium-java</artifactId>
            <version>4.25.0</version>
        </dependency>
        <dependency>
            <groupId>org.junit.jupiter</groupId>
            <artifactId>junit-jupiter</artifactId>
            <version>5.11.1</version>
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

Create directory structure:
```powershell
mkdir src\main\java\org\example
mkdir src\test\java\org\example
mkdir src\test\resources
```

Create `src/test/java/org/example/VerifySetupTest.java`:

```java
package org.example;

import org.junit.jupiter.api.Test;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import static org.junit.jupiter.api.Assertions.assertTrue;

public class VerifySetupTest {
    @Test
    void verifyChromeLaunches() {
        WebDriver driver = new ChromeDriver();
        driver.get("https://example.com");
        assertTrue(driver.getTitle().contains("Example"));
        driver.quit();
    }
}
```

Run:
```powershell
mvn test -Dtest=VerifySetupTest
```

---

## Troubleshooting

| Issue | Fix |
|---|---|
| `mvn` not recognized | Add Maven `bin` folder to System PATH, restart PowerShell |
| Chrome blocked by Cloudflare | Run in headed mode (remove `--headless=new`), use a fresh temp profile |
| Element not found on `practicesoftwaretesting.com` | Site is an Angular SPA — use explicit waits, not headless with mismatched user-agent |
| CDP warning for Chrome 151 | Informational only. Ignore unless you use DevTools features. Add `selenium-devtools` dependency if needed. |

---

## Skills to Load

- `selenium-setup` — Maven bootstrap, Selenium Manager, verification scripts.

---

## Milestone

`mvn test -Dtest=VerifySetupTest` runs successfully with no manual driver configuration. Selenium Manager resolved the driver automatically.
