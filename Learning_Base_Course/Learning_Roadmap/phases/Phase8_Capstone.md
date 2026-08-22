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
- **Explicit waits everywhere** — zero `Thread.sleep()`.
- **Screenshot-on-failure + logging** — auto-capture evidence into `test-results/`.
- **Data-driven tests** — CSV or JSON fixtures for test data via `@CsvFileSource` or `@JsonFileSource`.
- **Gherkin feature files via Cucumber** for the core user journey.
- **Parallel execution config** — JUnit 5 parallel enabled.
- **CI pipeline on GitHub Actions** — headless run with artifact upload.
- **`README.md`** — explain architecture decisions like you're handing the repo to a client.

---

## Suggested Structure

```
selenium-mastery/
├── src/
│   ├── main/java/pages/
│   │   ├── BasePage.java
│   │   ├── LoginPage.java
│   │   ├── ContactPage.java
│   │   └── AccountPage.java
│   └── test/java/
│       ├── BaseTest.java
│       ├── tests/
│       │   ├── LoginTest.java
│       │   ├── ContactTest.java
│       │   └── CheckoutTest.java
│       ├── features/
│       │   └── contact.feature
│       └── steps/
│           └── ContactSteps.java
├── src/test/resources/
│   └── login-data.csv
├── test-results/
├── target/
├── .github/workflows/tests.yml
├── docs/
│   └── framework-comparison.md
├── pom.xml
└── README.md
```

---

## Practice Flow

1. **Plan:** Sketch the page flow (login → contact → account).
2. **Scaffold:** Create `BasePage` and one page class per screen.
3. **Implement:** Write POM methods, then write tests asserting outcomes.
4. **Data-drive:** Load test data from CSV fixtures using `@CsvFileSource`.
5. **BDD layer:** Write 2–3 Cucumber feature files for the happy path and an edge case.
6. **Evidence:** Ensure screenshots and logs are captured on failure via `TestWatcher`.
7. **Scale:** Enable JUnit 5 parallel config and measure speedup.
8. **CI:** Add GitHub Actions workflow with artifact upload.
9. **Polish:** Write the README as a client handoff document.

---

## Skills to Load

- `selenium-setup` — fixtures, logging, screenshots.
- `selenium-locators-waits` — locators and waits.
- `selenium-page-object-model` — POM design and Cucumber BDD.
- `selenium-parallel-grid` — parallel execution and Grid setup (optional).

---

## Milestone

This repo becomes your **portfolio piece** for job applications and freelance QA gigs — link it directly in cover letters and your profile.
