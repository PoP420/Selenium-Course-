# Mastering Selenium — Full Study Plan
### With Kilo Code (VS Code AI assistant) · Local setup (no GitHub Codespaces) · LinkedIn Learning course as backbone

**Reference course:** [Learning Selenium: Structure, Scale, Run, and Optimize Automated Tests](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests) — Qambar Raza
**Reference repo (read-only, don't need Codespaces):** [LinkedInLearning/learning-selenium-structure-scale-run-and-optimize-automated-tests-5989088](https://github.com/LinkedInLearning/learning-selenium-structure-scale-run-and-optimize-automated-tests-5989088)

Course topics covered, in order: Selenium vs. modern frameworks → setup & Selenium Manager → debugging & screenshots on failure → Page Object Model → parallel execution & Selenium Grid → WireMock integration → CI with GitHub Actions.

---

## 0. Why this plan is structured this way

- You already know **Python, Cucumber/Gherkin BDD, TDD, Postman, SQL, Git** and have hands-on scripting in **Katalon** and **Playwright**. That means you skip "what is a locator / what is a test" — you go straight to Selenium-specific mechanics and how it differs from Playwright/Katalon under the hood.
- You **can't afford GitHub Codespaces** → everything below runs on your own laptop instead. Selenium 4.6+ ships **Selenium Manager**, which auto-downloads the right browser driver, so you don't need to hand-manage ChromeDriver versions. Setup cost is basically zero.
- You have **intermittent internet** → each phase lists what to download once, upfront, so you can practice offline.
- You're using **Kilo Code** (AI coding agent in VS Code) as a pair-programmer, not a crutch — see §6 for exactly how to use it so you actually retain the skill instead of just shipping code you can't explain in an interview.
- End goal ties into your existing plan: this becomes portfolio proof for your **QA/automation freelance gigs and job applications**, and pairs naturally with the **ISTQB Foundation** track you're already pursuing.

---

## 1. Environment Setup (Day 0 — do this once, needs internet)

1. Install **Python 3.11+**, **VS Code**, **Kilo Code extension**, and **Git**.
2. Install Google Chrome and Firefox (Selenium Manager will grab matching drivers automatically — no manual ChromeDriver downloads).
3. Create a project folder and virtual environment:
   ```bash
   mkdir selenium-mastery && cd selenium-mastery
   python -m venv venv
   venv\Scripts\activate        # Windows
   source venv/bin/activate     # macOS/Linux
   pip install selenium pytest pytest-html pytest-xdist behave requests
   ```
4. Clone the LinkedIn repo locally instead of opening it in Codespaces — you only need the files, not the cloud VM:
   ```bash
   git clone https://github.com/LinkedInLearning/learning-selenium-structure-scale-run-and-optimize-automated-tests-5989088.git
   ```
   Browse the branches locally (`git branch -a`, `git checkout <branch>`) to see each lesson's before/after state — this replaces the Codespaces experience at zero cost.
5. Pick a stable practice target site that doesn't need internet once cached, e.g. the-internet.herokuapp.com, or better: keep a **local static HTML test page** (I can generate one for you) so you can practice locators/waits fully offline.
6. Verify:
   ```python
   from selenium import webdriver
   driver = webdriver.Chrome()
   driver.get("https://example.com")
   print(driver.title)
   driver.quit()
   ```

> **Note on language:** the LinkedIn course likely demonstrates in JS/TS. Since your stack and freelance QA target (Katalon/Playwright/pytest experience) lean Python, this plan has you **watch the concept in the course, then re-implement it in Python** in your own repo. That's a stronger learning loop than copy-pasting anyway — you're translating, not transcribing.

---

## 2. Phase-by-Phase Curriculum

### Phase 1 — Selenium vs. Modern Frameworks (Foundations)
**Course lesson:** [Selenium vs. modern frameworks](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/selenium-vs-modern-frameworks)
**Time:** 2–3 days

- Watch the lesson; take notes specifically on: WebDriver protocol (W3C) vs. Playwright's CDP-based approach, why Selenium has broader browser/language support, where Selenium is slower (no built-in auto-wait) vs. Playwright.
- Since you already know Playwright, write yourself a **1-page comparison doc**: Selenium vs Playwright — waits, locators, architecture, when a client would ask for one over the other. This becomes interview material.
- Practice: open a browser, navigate, read title/URL, close it. Confirm Selenium Manager auto-resolved the driver with no manual setup.

**Milestone:** Can explain, unprompted, why a team would pick Selenium over Playwright (legacy codebase, wider language support, existing infra) and vice versa.

---

### Phase 2 — WebDriver Core: Locators, Actions, Waits
**Time:** 4–5 days (not a dedicated course chapter, but required before POM)

- Locator strategies: `id`, `name`, `css selector`, `xpath`, `link text`, `class name`. Practice writing both CSS and XPath for the same element — you'll need both in real codebases.
- Waits: `implicitly_wait`, `WebDriverWait` + `expected_conditions` (explicit waits). **Never use `time.sleep()`** — build this discipline now.
- Actions API: clicks, form fills, dropdowns, `ActionChains` for hover/drag, alert handling, window/tab switching, iframes.
- Build 5–8 small scripts against your offline test page covering each of the above.

**Milestone:** A `basics/` folder with one script per interaction type, each with a clear pass/fail assertion (use `assert`, not print statements).

---

### Phase 3 — Setup, Debugging, and Evidence Capture
**Course lessons:** setup / Selenium Manager, debugging, screenshots on failure
**Time:** 3–4 days

- Reproduce the course's debugging workflow in Python: catch exceptions (`NoSuchElementException`, `TimeoutException`), log meaningful errors, and **capture a screenshot automatically on test failure**.
- Build a `pytest` fixture/hook (`pytest_runtest_makereport` or a `conftest.py` teardown) that screenshots on failure — this is the Python equivalent of what the course does with its runner.
- Add basic logging (Python's `logging` module) so failures are traceable without re-running.

**Milestone:** Any test in your suite that fails automatically drops a timestamped screenshot + log entry into a `test-results/` folder.

---

### Phase 4 — Page Object Model (POM)
**Course lesson:** [Implement a POM in a test](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/implement-page-object-model-in-test)
**Time:** 5–6 days

- Study the course's POM structure, then rebuild it in Python: one class per page, locators as class attributes, actions as methods, no assertions inside page objects (assertions stay in test files).
- Refactor **all** your Phase 2–3 scripts into POM classes. This refactor is the actual learning — don't skip it.
- Add a `BasePage` class with shared waits/helpers that every page object inherits from.
- Optional but valuable given your Cucumber/Gherkin background: layer **Behave** (Python's Cucumber) on top of your POM so you can write feature files in Gherkin driving the same page objects — this directly reuses a skill you already have.

**Milestone:** A 3+ page test suite (e.g., login → search → checkout-style flow) fully in POM, runnable with a single `pytest` command.

---

### Phase 5 — Scaling: Parallel Execution & Selenium Grid
**Course lessons:** [Run tests in parallel](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/run-tests-in-parallel), [Test across browsers locally](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/test-across-browsers-locally)
**Time:** 4–5 days

- Parallelize your pytest suite with `pytest-xdist` (`pytest -n auto`) — Python's equivalent of whatever parallel runner the course uses.
- Set up **Selenium Grid locally with Docker** (hub + Chrome/Firefox nodes) — this is free and runs entirely on your machine, no cloud cost:
  ```bash
  docker run -d -p 4444:4444 --shm-size=2g selenium/standalone-chrome
  ```
  If Docker isn't installed yet, install Docker Desktop once (needs internet), then everything after is offline-capable.
- Point your `webdriver.Remote()` calls at the Grid instead of local `webdriver.Chrome()`, and run the same suite cross-browser (Chrome + Firefox) to see the abstraction pay off.

**Milestone:** Same test suite runs (a) in parallel locally, and (b) against a local Selenium Grid across 2 browsers, with a documented before/after execution time.

---

### Phase 6 — Selenium Manager & WireMock
**Course lessons:** Selenium Manager deep dive, WireMock integration
**Time:** 3–4 days

- Understand what Selenium Manager solves (driver/browser version mismatches) and where it can still fail (offline environments, custom driver paths) — relevant given your intermittent connectivity.
- Install and run **WireMock** locally (a mock HTTP server) to stub backend API responses so your UI tests aren't dependent on a live backend — valuable for testing flaky or rate-limited third-party integrations.
- Build one test that hits a WireMock-stubbed endpoint instead of a real API, and one that intentionally simulates a slow/failed response to test your app's error handling.

**Milestone:** A test that proves UI behavior under a *mocked* backend failure (e.g., 500 error, timeout) — a scenario that's hard to test against a real server.

---

### Phase 7 — CI/CD with GitHub Actions
**Course lesson:** [Run tests in CI with GitHub Actions](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/challenge-run-tests-in-ci-with-github-actions)
**Time:** 3–4 days

- GitHub Actions is **free for public repos** and has a generous free tier for private ones — this is the one piece of "cloud" you actually want, since it doesn't cost you anything and is a resume-visible artifact.
- Write a `.github/workflows/tests.yml` that installs Python, installs deps, and runs your pytest suite headless (`--headless` Chrome options) on every push.
- Upload the `test-results/` folder (screenshots, HTML report via `pytest-html`) as a workflow artifact.

**Milestone:** A public GitHub repo where pushing a commit automatically runs your full Selenium suite and you can link the passing badge/run in your portfolio or job applications.

---

### Phase 8 — Capstone Project (Mastery)
**Time:** 1–2 weeks

Build one complete framework from scratch, applying everything above, against a realistic multi-page site (an e-commerce demo site like saucedemo.com works well and is stable):

- POM for every page
- Explicit waits everywhere, zero `time.sleep()`
- Screenshot-on-failure + logging
- Data-driven tests (CSV or JSON fixtures, reusing your Postman/API-testing instincts to also validate any exposed API alongside the UI)
- Gherkin feature files via Behave for at least the core user journey
- Parallel execution config
- CI pipeline on GitHub Actions with HTML report artifact
- A `README.md` explaining architecture decisions — write this like you're handing the repo to a client, since that's effectively what you're doing

**Milestone:** This repo becomes your **portfolio piece** for Fiverr QA gigs and job applications — link it directly in cover letters and your profile.

---

## 3. Suggested Timeline

| Week | Focus |
|---|---|
| 1 | Setup + Phase 1 + Phase 2 |
| 2 | Phase 3 + Phase 4 |
| 3 | Phase 5 + Phase 6 |
| 4 | Phase 7 |
| 5–6 | Phase 8 capstone + polish + write-up |

Adjust freely around your job applications and interview schedule — the phases are sequential but not date-locked.

---

## 4. Using Kilo Code Without Skipping the Learning

Kilo Code is an AI agent inside VS Code — treat it like a senior pair-programmer, not an answer key:

- **Ask it to explain, not just generate.** Before accepting a POM class it writes, ask "why did you structure the locators this way instead of X" — make it teach you the reasoning.
- **Write the first draft yourself, then have it review.** For each new concept (waits, POM, Grid config), attempt it solo first, hit a wall, *then* bring in Kilo Code to unblock you. This mirrors real work far better than generating everything upfront.
- **Use it for the tedious parts you already understand**, like scaffolding boilerplate `conftest.py` fixtures or CI YAML, so you spend your limited time on the concepts, not syntax.
- **Have it generate deliberately broken code for debugging practice** — ask it to introduce a flaky wait or wrong locator, then debug it yourself using Phase 3 skills.
- At the end of each phase, **explain out loud (or in writing) what you built and why**, without looking at the code — if you can't, that phase isn't done yet regardless of what's in the repo.

---

## 5. After This Plan

- Cross-reference with your existing **ISTQB Foundation** prep — Selenium mastery plus ISTQB theory covers both the practical and certification sides of QA applications.
- Fold this into your **Fiverr QA testing gig** listing as a specific, demonstrable skill with a linked GitHub repo and CI badge.
- Consider a short add-on module comparing your new Selenium skills against your existing Katalon/Playwright experience — being able to speak to trade-offs across all three is a strong differentiator for freelance QA work.
