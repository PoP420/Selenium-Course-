# Phase 7 — Advanced CI/CD with GitHub Actions

**Time:** 3–4 days

---

## Objectives

- Write a GitHub Actions workflow that installs .NET, installs Playwright browsers, and runs tests headless on every push.
- Upload test results (traces, screenshots) as workflow artifacts.
- Add a passing badge to your repo for portfolio visibility.
- Configure parallel test execution in CI.

---

## Topics

- GitHub Actions free tier for public/private repos.
- `playwright install --with-deps` to install browsers + system dependencies on Linux.
- Headless mode (Playwright is headless by default in CI).
- NUnit test output and trace artifact upload.
- Parallel execution in CI via NUnit's built-in parallelization.

---

## Practice

1. Create `.github/workflows/tests.yml` in your repo.
2. Configure the workflow to:
   - Trigger on every push.
   - Checkout code.
   - Set up .NET.
   - Install dependencies (`dotnet restore`).
   - Install Playwright browsers (`playwright install --with-deps`).
   - Run tests headless (`dotnet test`).
   - Upload `test-results/` as workflow artifacts.
3. Push a commit and verify the workflow runs successfully.
4. Add a badge to your `README.md`.

```yaml
# .github/workflows/tests.yml
name: Playwright Tests

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

      - name: Install Playwright browsers
        run: |
          dotnet test --no-build --no-restore
          playwright install-deps --accept-hard-links
          playwright install --with-deps

      - name: Run tests
        run: dotnet test --no-build --verbosity normal --logger "trx;LogFileName=test_results.trx"

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

### Headless mode

Playwright runs headless by default in CI. You can explicitly set it in your fixture:

```csharp
// Fixtures/CustomFixture.cs
public class CustomFixture : PlaywrightTest
{
    public CustomFixture()
    {
        Headless = true;  // default in CI; set to false for local debugging
    }
}
```

Or via environment variable:

```yaml
- name: Run tests
  run: dotnet test
  env:
    PLAYWRIGHT_BROWSERS_PATH: "0"  # use system-installed browsers
```

### Badge in README

```markdown
![Playwright Tests](https://github.com/<user>/<repo>/actions/workflows/tests.yml/badge.svg)
```

### Parallel execution in CI

NUnit handles parallelization automatically. You can control the level via `dotnet test`:

```powershell
dotnet test -p:Parallelize=true -p:ParallelizeCount=4
```

Or via `NUnit.config`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<NUnit>
  <defaults>
    <testcase />
  </defaults>
  <settings>
    <setting name="NumberOfTestWorkers" value="4" />
  </settings>
</NUnit>
```

---

## Skills to Load

- `playwright-csharp-setup` — PlaywrightTest base class, headless configuration.
- `playwright-csharp-parallel` — parallel and cross-browser config.

---

## Milestone

A public GitHub repo where pushing a commit automatically runs your full Playwright suite headless, uploads traces, screenshots, and TRX reports as artifacts, and displays a passing badge in the README.
