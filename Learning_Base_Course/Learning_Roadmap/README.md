# Selenium Mastery — Learning Roadmap

## Overview

This roadmap guides you from zero Selenium experience to a **portfolio-ready automation framework** that you can show to hiring managers, link in freelance QA proposals, and discuss confidently in interviews.

**Prerequisites:** Java 21+, Maven, Git, IntelliJ IDEA Community Edition (or VS Code + Java extensions), basic HTML/CSS understanding.  
**Duration:** 5–6 weeks part-time.  
**End goal:** A public GitHub repo with a complete Java Selenium test framework, plus companion master guides covering every major topic in depth.

---

## Phases

| Phase | Topic | Duration | Link |
|---|---|---|---|
| 0 | Environment Setup | Day 0 | [Phase 0](phases/Phase0_Setup.md) |
| 1 | Selenium vs. Modern Frameworks | 2–3 days | [Phase 1](phases/Phase1_Foundations.md) |
| 2 | WebDriver Core: Locators, Actions, Waits | 4–5 days | [Phase 2](phases/Phase2_WebDriver_Core.md) |
| 3 | Debugging & Evidence Capture | 3–4 days | [Phase 3](phases/Phase3_Debugging.md) |
| 4 | Page Object Model (POM) | 5–6 days | [Phase 4](phases/Phase4_POM.md) |
| 5 | Parallel Execution & Selenium Grid | 4–5 days | [Phase 5](phases/Phase5_Parallel_Grid.md) |
| 6 | Selenium Manager & WireMock | 3–4 days | [Phase 6](phases/Phase6_WireMock.md) |
| 7 | CI/CD with GitHub Actions | 3–4 days | [Phase 7](phases/Phase7_CI.md) |
| 8 | Capstone Project (Mastery) | 1–2 weeks | [Phase 8](phases/Phase8_Capstone.md) |

---

## Suggested Timeline

| Week | Focus |
|---|---|
| 1 | Setup + Phase 1 + Phase 2 |
| 2 | Phase 3 + Phase 4 |
| 3 | Phase 5 + Phase 6 |
| 4 | Phase 7 |
| 5–6 | Phase 8 capstone + polish + portfolio packaging |

Adjust freely around interviews and applications. The phases are sequential but not date-locked.

---

## Full Selenium Guides

Deep-dive companion documents for topics that deserve more than a phase doc can cover.

| Guide | Path | What It Covers |
|---|---|---|
| Locators Mastery | [full_selenium_guides/locators_mastery.md](full_selenium_guides/locators_mastery.md) | ID, name, CSS, XPath (axes, functions), link text, class name, shadow DOM, dynamic elements, priority order, anti-patterns |
| Waits & Synchronization | [full_selenium_guides/waits_mastery.md](full_selenium_guides/waits_mastery.md) | Implicit vs explicit vs fluent waits, ExpectedConditions deep dive, stale elements, page load strategy, Angular/SPA timing |
| Actions API Deep Dive | [full_selenium_guides/actions_api.md](full_selenium_guides/actions_api.md) | Click, double-click, context click, drag-and-drop, hover, key combos, scroll, pointer events, touch actions |
| Alerts, Popups & Windows | [full_selenium_guides/alerts_popups_windows.md](full_selenium_guides/alerts_popups_windows.md) | JS alerts, confirms, prompts, authentication dialogs, window/tab switching, iframes, shadow DOM, file uploads, downloads |

---

## Skill Reference

| Skill | Location | Use When |
|---|---|---|
| `selenium-setup` | `.kilo/skills/selenium-setup/SKILL.md` | Maven bootstrap, Selenium Manager, JUnit 5 setup, screenshot-on-failure, Java logging |
| `selenium-locators-waits` | `.kilo/skills/selenium-locators-waits/SKILL.md` | Locators, explicit waits, Actions API, alerts, iframes, window switching in Java |
| `selenium-page-object-model` | `.kilo/skills/selenium-page-object-model/SKILL.md` | POM design, BasePage, page classes, Cucumber BDD layer |
| `selenium-parallel-grid` | `.kilo/skills/selenium-parallel-grid/SKILL.md` | JUnit 5 parallel config, Selenium Grid with Docker, cross-browser testing |
| `kilo-short-reasoning` | `.kilo/skills/kilo-short-reasoning/SKILL.md` | Keeping AI outputs concise and focused during learning |

---

## Course Reference

| Resource | Path |
|---|---|
| Course repo (Java, Maven) | `learning-selenium-structure-scale-run-and-optimize-automated-tests-5989088/` |
| Reference branches | `02_02_e` through `04_03` (Phases 0–4); Phases 5–7 not in repo |
| Test target site | `practicesoftwaretesting.com` (requires internet) |
| Working project | `selenium-mastery/` (your own copy for practice) |

> **Note:** The course reference code is Java + JUnit 5 + Maven. This roadmap and all skills are aligned to Java. For offline practice, use a local static HTML test page.
