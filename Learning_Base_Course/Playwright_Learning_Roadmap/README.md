# Playwright Mastery — Learning Roadmap

## Overview

This roadmap guides you from zero Playwright experience to a **portfolio-ready automation framework** in C#/.NET that you can show to hiring managers, link in freelance QA proposals, and discuss confidently in interviews.

**Prerequisites:** .NET 8+ SDK, Visual Studio 2022 / Rider / VS Code with C# extensions, Git, basic HTML/CSS understanding, familiarity with Selenium C# (see `Csharp_Selenium_Learning_Roadmap`).

**Duration:** 4–5 weeks part-time.

**End goal:** A public GitHub repo with a complete C# Playwright test framework, plus companion master guides covering every major topic in depth.

> **Note:** This roadmap is aligned to C# + Playwright NUnit. It complements the existing Selenium C# roadmap by showing Playwright's modern approach: auto-waiting, built-in tracing, locators, and single-executable browser management — no Selenium Grid, no manual driver downloads.

---

## Phases

| Phase | Topic | Duration | Link |
|---|---|---|---|
| 0 | Environment Setup | Day 0 | [Phase 0](phases/Phase0_Setup.md) |
| 1 | Playwright vs. Selenium vs. Katalon | 2–3 days | [Phase 1](phases/Phase1_Foundations.md) |
| 2 | Playwright Core: Locators, Actions, Auto-Waiting | 4–5 days | [Phase 2](phases/Phase2_Playwright_Core.md) |
| 3 | Debugging & Evidence Capture (Tracing, Video) | 3–4 days | [Phase 3](phases/Phase3_Debugging.md) |
| 4 | Page Object Model (POM) | 5–6 days | [Phase 4](phases/Phase4_POM.md) |
| 5 | Parallel & Cross-Browser | 4–5 days | [Phase 5](phases/Phase5_Parallel.md) |
| 6 | Visual Testing & API Testing | 3–4 days | [Phase 6](phases/Phase6_Visual_API.md) |
| 7 | Advanced CI/CD with GitHub Actions | 3–4 days | [Phase 7](phases/Phase7_CI.md) |
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

## Full Playwright Guides

Deep-dive companion documents for topics that deserve more than a phase doc can cover.

| Guide | Path | What It Covers |
|---|---|---|
| Locators Mastery | [full_playwright_guides/locators_mastery.md](full_playwright_guides/locators_mastery.md) | `page.Locator()`, `GetByRole`, `GetByTestId`, `GetByText`, CSS/XPath selectors, filtering, nth/regex, best practices |
| Waits & Auto-Waiting | [full_playwright_guides/waits_mastery.md](full_playwright_guides/waits_mastery.md) | Auto-waiting mechanism, `Expect` assertions with retries, manual `WaitForAsync`, soft assertions, page load timing |
| Actions Mastery | [full_playwright_guides/actions_mastery.md](full_playwright_guides/actions_mastery.md) | Click, fill, hover, drag-and-drop, keyboard, scroll, file upload/download, multi-touch |
| API Testing | [full_playwright_guides/api_testing.md](full_playwright_guides/api_testing.md) | `APIRequestContext`, GET/POST/PUT/DELETE stubs, authentication, request fixtures, API + UI combo tests |

---

## Skill Reference

| Skill | Location | Use When |
|---|---|---|
| `playwright-csharp-setup` | `.kilo/skills/playwright-csharp-setup/SKILL.md` | .NET project bootstrap, Playwright NuGet, `playwright install`, PlaywrightTest base class, tracing/video config |
| `playwright-csharp-locators-waits` | `.kilo/skills/playwright-csharp-locators-waits/SKILL.md` | Locator strategies, auto-waiting, `Expect` assertions, actions, frames, file upload |
| `playwright-csharp-pom` | `.kilo/skills/playwright-csharp-pom/SKILL.md` | POM design, page models, fixtures, SpecFlow BDD layer |
| `playwright-csharp-parallel` | `.kilo/skills/playwright-csharp-parallel/SKILL.md` | NUnit parallel, Playwright workers, cross-browser, CI pipeline config |
| `kilo-short-reasoning` | `.kilo/skills/kilo-short-reasoning/SKILL.md` | Keeping AI outputs concise and focused during learning |

---

## Course Reference

| Resource | Path |
|---|---|
| Test project (C#, NUnit + Playwright) | `PlaywrightCSharpTests/` (your working project for practice) |
| Test target site | `practicesoftwaretesting.com` (requires internet) |
| Working project | `PlaywrightCSharpTests/` (your own copy for practice) |

> **Note:** The Playwright C# course uses `Microsoft.Playwright.NUnit`. Playwright ships its own browser binaries — no separate driver downloads. Run `playwright install` once to download Chromium, Firefox, and WebKit.
