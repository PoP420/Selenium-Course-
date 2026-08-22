# 🚀 Selenium Automation Mastery — Zero to Portfolio-Ready

<p align="center">
  <img src="https://img.shields.io/badge/Java-21-blue?logo=openjdk" alt="Java 21" />
  <img src="https://img.shields.io/badge/C%23-.NET_10-68217A?logo=csharp" alt="C# .NET 10" />
  <img src="https://img.shields.io/badge/Selenium-4.47-green?logo=selenium" alt="Selenium 4" />
  <img src="https://img.shields.io/badge/NUnit-4.3-green?logo=nunit" alt="NUnit 4" />
  <img src="https://img.shields.io/badge/JUnit-5-red?logo=junit5" alt="JUnit 5" />
  <img src="https://img.shields.io/badge/Maven-3.9-C71A36?logo=apachemaven" alt="Maven" />
  <img src="https://img.shields.io/badge/Docker-Grid-2496ED?logo=docker" alt="Docker" />
  <img src="https://img.shields.io/badge/GitHub_Actions-CI-2088FF?logo=githubactions" alt="CI" />
</p>

<p align="center">
  <b>A structured 8-phase course repository</b> that takes you from zero Selenium experience to a complete, interview-ready automation framework — in <b>Java + C#</b>, with production-grade POM, parallel execution, Docker Grid, WireMock, and GitHub Actions CI.
</p>

---

## 👋 Why This Repo Exists

Most Selenium courses stop at "hello world." This repo goes further.

Built around the LinkedIn Learning course *"Learning Selenium: Structure, Scale, Run, and Optimize Automated Tests"* by Qambar Raza, it transforms a passive video course into an **active, portfolio-building curriculum**. Every phase produces a linkable milestone. The capstone is a complete framework you can put in job applications and freelance proposals.

It runs **locally on Windows 11** — no paid Codespaces, no cloud IDE required. Two parallel tracks (Java and C#) let you master Selenium across the two most in-demand enterprise languages.

---

## 🗺️ What's Inside

| Track | What You Build | Target |
|-------|---------------|--------|
| **Java (Reference + Roadmap)** | Maven project mirroring the LinkedIn course; 8-phase roadmap with deep-dive guides | `practicesoftwaretesting.com` |
| **C# (Working Project)** | Live POM framework with 7+ page objects, multi-page flows, sorting, search, BDD layer | `practicesoftwaretesting.com` |
| **AI Skills** | 8 `.kilo/skills` that turn your IDE into a pair-programming tutor | Kilo Code (VS Code) |

---

## 🛠️ Tech Stack

### Java Track
| Tool | Version |
|------|---------|
| Java | 21 |
| Maven | 3.9.9 |
| Selenium | 4.25.0 |
| JUnit | 5.11.1 |
| Cucumber (BDD) | 7.15.0 |
| CI | GitHub Actions (ubuntu-latest, headless Chrome) |

### C# Track
| Tool | Version |
|------|---------|
| C# / .NET | 10.0 |
| Selenium | 4.47.0 |
| ChromeDriver | 151.0.7922.13800 (pinned) |
| NUnit | 4.3.2 |
| coverlet | 6.0.4 |
| SpecFlow (BDD) | Latest |
| CI | GitHub Actions (ubuntu-latest, headless Chrome) |

---

## 📚 8-Phase Learning Roadmap

| Phase | Topic | Duration |
|-------|-------|----------|
| 0 | Environment Setup | Day 0 |
| 1 | Selenium vs. Playwright / Katalon | 2–3 days |
| 2 | WebDriver Core: Locators, Waits, Actions API | 4–5 days |
| 3 | Debugging & Evidence Capture | 3–4 days |
| 4 | Page Object Model (POM) + BDD | 5–6 days |
| 5 | Parallel Execution & Selenium Grid | 4–5 days |
| 6 | Selenium Manager & WireMock | 3–4 days |
| 7 | CI/CD with GitHub Actions | 3–4 days |
| 8 | Capstone Project | 1–2 weeks |

**Total:** ~5–6 weeks part-time to a portfolio-ready framework.

---

## 🧠 Deep-Dive Companion Guides

| Guide | What It Covers |
|-------|----------------|
| Locators Mastery | ID, CSS, XPath axes/functions, shadow DOM, dynamic elements, priority order |
| Waits & Synchronization | Implicit vs. explicit vs. fluent, stale elements, Angular/SPA timing |
| Actions API | Drag-and-drop, hover, key combos, scroll, pointer events |
| Alerts, Popups & Windows | JS alerts, confirms, iframes, shadow DOM, file uploads |

---

## ✨ Standout Features

- **Dual-language mastery** — Parallel, production-quality implementations in Java (Maven/JUnit 5) and C# (.NET/NUnit 4)
- **AI-guided learning** — 8 custom Kilo Code skills provide context-aware, language-specific guidance as you code
- **Portfolio-first design** — Every phase ends with a milestone you can showcase; capstone README doubles as a client handoff document
- **Real-world target** — Tests run against `practicesoftwaretesting.com`, a live Angular e-commerce demo
- **Multi-page flows** — Login → browse → product detail → add-to-cart, plus sorting, search, and registration
- **Advanced infra** — Docker Selenium Grid, WireMock API mocking, screenshot-on-failure hooks, data-driven CSV fixtures
- **BDD layers** — Cucumber (Java) and SpecFlow (C#) on top of the same POM

---

## 📂 Project Structure

```
SeleniumCodebaseCourse/
├── Learning_Base_Course/            ← 8-phase Java + C# roadmaps & guides
├── .kilo/skills/                    ← 8 AI skills for guided learning
├── learning-selenium-*/            ← Java reference course (LinkedIn Learning)
└── SeleniumCSharpTests/             ← Live C# POM framework (src/main + src/test)
    ├── src/main/csharp/Pages/       ← 7+ page objects
    └── src/test/csharp/             ← NUnit tests + TestBase
```

---

## 🏁 Getting Started

### Java
```bash
cd learning-selenium-structure-scale-run-and-optimize-automated-tests-5989088
mvn test
```

### C#
```bash
cd SeleniumCSharpTests
dotnet test
```

> **Prerequisites:** Java 21+, Maven, Git, IntelliJ IDEA (Java) / Visual Studio 2022 or Rider (C#)

---

## 🎯 Who This Is For

- **QA Engineers** transitioning to automation or expanding to Selenium
- **Developers** who want production-grade test automation patterns
- **Freelancers** building portfolio projects for Fiverr / Upwork QA gigs
- **Job seekers** preparing for automation interview discussions
- **Students** of the LinkedIn Learning course who want a local, offline practice environment

---

<p align="center">
  <b>Built with discipline. Tested in the real world. Ready for your portfolio.</b>
</p>

<p align="center">
  <a href="Learning_Base_Course/Learning_Roadmap/README.md">Java Roadmap</a> ·
  <a href="Learning_Base_Course/Csharp_Selenium_Learning_Roadmap/README.md">C# Roadmap</a> ·
  <a href="Learning_Base_Course/selenium-mastery-plan.md">Master Plan</a>
</p>
