# Phase 7 — CI/CD with GitHub Actions

**Time:** 3–4 days  
**Course lesson:** [Run tests in CI with GitHub Actions](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/challenge-run-tests-in-ci-with-github-actions)

---

## Objectives

- Write a GitHub Actions workflow that installs Java, installs deps, and runs Maven tests headless on every push.
- Upload test results (screenshots, Surefire reports) as workflow artifacts.
- Add a passing badge to your repo for portfolio visibility.

---

## Topics

- GitHub Actions free tier for public/private repos.
- Headless Chrome in CI (`--headless=new` Chrome options).
- Maven Surefire report generation.
- Workflow artifacts for `test-results/` and `target/surefire-reports/`.

---

## Practice

1. Create `.github/workflows/tests.yml` in your repo.
2. Configure the workflow to:
   - Trigger on every push.
   - Checkout code.
   - Set up Java.
   - Install dependencies (`mvn install`).
   - Run Maven tests headless.
   - Upload `test-results/` and `target/surefire-reports/` as workflow artifacts.
3. Push a commit and verify the workflow runs successfully.
4. Add a badge to your `README.md`.

```yaml
# .github/workflows/tests.yml
name: Selenium Tests

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Set up Java
        uses: actions/setup-java@v4
        with:
          java-version: '21'
          distribution: 'temurin'

      - name: Install Chrome
        run: ./setup.sh

      - name: Run tests
        run: mvn test

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: test-results
          path: test-results/

      - name: Upload Surefire reports
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: surefire-reports
          path: target/surefire-reports/
```

### Headless Chrome options for CI

```java
// BaseTest.java — headless for CI
ChromeOptions options = new ChromeOptions();
options.addArguments("--headless=new");
options.addArguments("--no-sandbox");
options.addArguments("--disable-dev-shm-usage");
driver = new ChromeDriver(options);
```

### Badge in README

```markdown
![Selenium Tests](https://github.com/<user>/<repo>/actions/workflows/tests.yml/badge.svg)
```

---

## Skills to Load

- `selenium-setup` — JUnit 5 setup, headless configuration, Maven reports.

---

## Milestone

A public GitHub repo where pushing a commit automatically runs your full Selenium suite and uploads screenshots + Surefire reports as artifacts. Link the passing badge in your portfolio.
