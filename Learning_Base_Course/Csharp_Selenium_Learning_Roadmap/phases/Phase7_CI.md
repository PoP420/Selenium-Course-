# Phase 7 — CI/CD with GitHub Actions

**Time:** 3–4 days

---

## Objectives

- Write a GitHub Actions workflow that installs .NET, installs deps, and runs NUnit tests headless on every push.
- Upload test results (screenshots, NUnit reports) as workflow artifacts.
- Add a passing badge to your repo for portfolio visibility.

---

## Topics

- GitHub Actions free tier for public/private repos.
- Headless Chrome in CI (`--headless=new` Chrome options).
- NUnit test report generation.
- Workflow artifacts for `test-results/` and `TestResults/`.

---

## Practice

1. Create `.github/workflows/tests.yml` in your repo.
2. Configure the workflow to:
   - Trigger on every push.
   - Checkout code.
   - Set up .NET.
   - Restore and build.
   - Run NUnit tests headless.
   - Upload `test-results/` and `TestResults/` as workflow artifacts.
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

      - name: Set up .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Run tests
        run: dotnet test --no-build --logger "trx;LogFileName=test_results.trx"

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: test-results
          path: test-results/

      - name: Upload NUnit TRX reports
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: nunit-reports
          path: TestResults/
```

### Headless Chrome options for CI

```csharp
// TestBase.cs — headless for CI
var options = new ChromeOptions();
if (Environment.GetEnvironmentVariable("CI") != null)
{
    options.AddArgument("--headless=new");
    options.AddArgument("--no-sandbox");
    options.AddArgument("--disable-dev-shm-usage");
}
var driver = new ChromeDriver(options);
```

### Badge in README

```markdown
![Selenium Tests](https://github.com/<user>/<repo>/actions/workflows/tests.yml/badge.svg)
```

---

## Skills to Load

- `selenium-setup` — NUnit setup, headless configuration, test reports.

---

## Milestone

A public GitHub repo where pushing a commit automatically runs your full Selenium suite and uploads screenshots + test reports as artifacts. Link the passing badge in your portfolio.
