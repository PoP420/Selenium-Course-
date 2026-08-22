# Phase 8 — Capstone Project (Mastery)

**Time:** 1–2 weeks

---

## Objectives

Build one complete Selenium test framework from scratch, applying everything from Phases 0–7. This repo becomes your **portfolio piece** for job applications and freelance QA gigs.

---

## Target Site

Use a realistic, stable, multi-page demo site:

- [practicesoftwaretesting.com](https://practicesoftwaretesting.com) — used in the course.
- Or any other stable e-commerce/demo site.

---

## Requirements

- **POM for every page** — one class per screen, `By` locators as attributes, actions as methods.
- **Explicit waits everywhere** — zero `Thread.Sleep()`.
- **Screenshot-on-failure + logging** — auto-capture evidence into `test-results/`.
- **Data-driven tests** — CSV or JSON fixtures for test data via `[TestCaseSource]` or `FromXyz` attributes.
- **Gherkin feature files via SpecFlow** for the core user journey.
- **Parallel execution config** — NUnit parallel enabled.
- **CI pipeline on GitHub Actions** — headless run with artifact upload.
- **`README.md`** — explain architecture decisions like you're handing the repo to a client.

---

## Suggested Structure

```
SeleniumCSharpTests/
├── Pages/
│   ├── BasePage.cs
│   ├── LoginPage.cs
│   ├── ContactPage.cs
│   └── AccountPage.cs
├── Tests/
│   ├── TestBase.cs
│   ├── LoginTest.cs
│   ├── ContactTest.cs
│   └── CheckoutTest.cs
├── Features/
│   └── ContactForm.feature
├── StepDefinitions/
│   └── ContactSteps.cs
├── TestData/
│   └── login-data.csv
├── test-results/
├── TestResults/
├── .github/workflows/tests.yml
├── docs/
│   └── framework-comparison.md
├── SeleniumCSharpTests.csproj
└── README.md
```

---

## Practice Flow

1. **Plan:** Sketch the page flow (login → contact → account).
2. **Scaffold:** Create `BasePage` and one page class per screen.
3. **Implement:** Write POM methods, then write tests asserting outcomes.
4. **Data-drive:** Load test data from CSV fixtures using `[TestCaseSource]`.
5. **BDD layer:** Write 2–3 SpecFlow feature files for the happy path and an edge case.
6. **Evidence:** Ensure screenshots and logs are captured on failure via `TearDown`.
7. **Scale:** Enable NUnit parallel config and measure speedup.
8. **CI:** Add GitHub Actions workflow with artifact upload.
9. **Polish:** Write the README as a client handoff document.

---

## Skills to Load

- `selenium-setup` — fixtures, logging, screenshots.
- `selenium-locators-waits` — locators and waits.
- `selenium-page-object-model` — POM design and SpecFlow BDD.
- `selenium-parallel-grid` — parallel execution and Grid setup (optional).

---

## Milestone

This repo becomes your **portfolio piece** for job applications and freelance QA gigs — link it directly in cover letters and your profile.
