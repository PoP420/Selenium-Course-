# Phase 1 — Selenium vs. Modern Frameworks

**Time:** 2–3 days  
**Course lesson:** [Selenium vs. modern frameworks](https://www.linkedin.com/learning/learning-selenium-structure-scale-run-and-optimize-automated-tests/selenium-vs-modern-frameworks)

---

## Objectives

- Understand Selenium's architecture compared to Playwright and Katalon.
- Identify when Selenium is the right tool vs. modern alternatives.
- Write a comparison document you can reference in interviews.
- Know which questions to ask a client before recommending a framework.

---

## Topics

### Selenium Architecture

- **WebDriver protocol (W3C)** — language-agnostic, browser-vendor implementations.
- **Selenium Manager** — auto-resolves drivers (Selenium 4.6+).
- **Language bindings** — Java, Python, C#, JavaScript, Ruby.
- **Grid** — distributed execution across browsers/machines.

### Playwright

- **CDP-based** — built on Chrome DevTools Protocol, faster element interaction.
- **Auto-wait** — waits for elements to be actionable by default.
- **Modern only** — Chromium, Firefox, WebKit. No legacy IE/Safari support.
- **Single executable** — no separate driver downloads.

### Katalon

- **Low-code IDE** — record and playback, built on Selenium/Appium.
- **Enterprise focus** — reporting, test management, CI plugins.
- **Less flexible** — harder to customize at the code level.

### Decision Matrix

| Factor | Selenium | Playwright | Katalon |
|---|---|---|---|
| Legacy browser support | Yes | No | Partial |
| Language flexibility | High | Moderate | Low |
| Auto-wait | No (manual) | Yes | Partial |
| CI/CD integration | Excellent | Excellent | Good |
| Enterprise reporting | DIY | DIY | Built-in |
| Learning curve | Steep | Moderate | Low |
| Community / jobs | Very high | Growing | Moderate |

---

## Practice

1. Watch the course lesson and take notes on the matrix above.
2. Write a **1-page comparison doc**: Selenium vs Playwright — architecture, waits, locators, when a client would ask for one over the other. Save it as `docs/framework-comparison.md` in your repo.
3. Practice the basics:

```java
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;

WebDriver driver = new ChromeDriver();
driver.get("https://example.com");
System.out.println(driver.getTitle());
driver.quit();
```

4. Interview prep: practice explaining in 30 seconds why you chose Selenium for a project.

---

## Skills to Load

- `selenium-setup` — for environment verification and Selenium Manager behavior.

---

## Milestone

Can explain, unprompted, why a team would pick Selenium over Playwright (legacy codebase, wider language support, existing infra) and vice versa. Have a written comparison doc saved.
