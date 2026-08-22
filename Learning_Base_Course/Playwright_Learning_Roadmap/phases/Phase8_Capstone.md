# Phase 8 — Capstone Project (Mastery)

**Time:** 1–2 weeks

---

## Objectives

Build one complete Playwright test framework from scratch, applying everything from Phases 0–7. This repo becomes your **portfolio piece** for job applications and freelance QA gigs.

---

## Target Site

Use a realistic, stable, multi-page demo site:

- [practicesoftwaretesting.com](https://practicesoftwaretesting.com) — used in the course.
- Or any other stable e-commerce/demo site.

---

## Requirements

- **POM for every page** — one class per screen, `ILocator` fields, async action methods.
- **Auto-waiting everywhere** — zero `Task.Delay()`.
- **Tracing & screenshot-on-failure** — auto-capture evidence into `test-results/`.
- **Data-driven tests** — CSV or JSON fixtures for test data.
- **Gherkin feature files via SpecFlow** for the core user journey.
- **Parallel execution config** — NUnit parallel enabled.
- **CI pipeline on GitHub Actions** — headless run with artifact upload.
- **`README.md`** — explain architecture decisions like you're handing the repo to a client.

---

## Suggested Structure

```
PlaywrightCSharpTests/
├── src/
│   ├── main/
│   │   ├── csharp/
│   │   │   ├── Pages/
│   │   │   │   ├── BasePage.cs
│   │   │   │   ├── LoginPage.cs
│   │   │   │   ├── ContactPage.cs
│   │   │   │   └── AccountPage.cs
│   │   │   └── Utils/
│   │   │       └── TestLogger.cs
│   └── test/
│       ├── csharp/
│       │   ├── Tests/
│       │   │   ├── TestBase.cs
│       │   │   ├── LoginTest.cs
│       │   │   ├── ContactTest.cs
│       │   │   └── CheckoutTest.cs
│       │   ├── Features/
│       │   │   └── ContactForm.feature
│       │   └── StepDefinitions/
│       │       └── ContactSteps.cs
├── tests/                          ← shared config
│   └── TestData/
│       └── login-data.csv
├── test-results/
├── .github/workflows/tests.yml
├── docs/
│   └── framework-comparison.md
├── PlaywrightCSharpTests.csproj
├── PlaywrightCSharpTests.Tests/PlaywrightCSharpTests.Tests.csproj
├── PlaywrightCSharpTests.slnx
└── README.md
```

---

## Practice Flow

1. **Plan:** Sketch the page flow (login → contact → account).
2. **Scaffold:** Create `BasePage` and one page class per screen.
3. **Implement:** Write POM methods, then write tests asserting outcomes.
4. **Data-drive:** Load test data from CSV fixtures.
5. **BDD layer:** Write 2–3 SpecFlow feature files for the happy path and an edge case.
6. **Evidence:** Ensure traces and screenshots are captured on failure.
7. **Scale:** Enable NUnit parallel config and measure speedup.
8. **CI:** Add GitHub Actions workflow with artifact upload.
9. **Polish:** Write the README as a client handoff document.

---

## Skills to Load

- `playwright-csharp-setup` — fixtures, logging, tracing, screenshots.
- `playwright-csharp-locators-waits` — locators, auto-waiting, actions.
- `playwright-csharp-pom` — POM design, SpecFlow BDD.
- `playwright-csharp-parallel` — parallel execution and cross-browser (optional).

---

## Milestone

This repo becomes your **portfolio piece** for job applications and freelance QA gigs — link it directly in cover letters and your profile.
