# Phase 6 — Selenium Manager & WireMock

**Time:** 3–4 days

---

## Objectives

- Understand Selenium Manager's driver resolution and offline fallback behavior in C#.
- Install and run WireMock.Net locally to stub backend API responses.
- Build UI tests that depend on mocked APIs instead of live backends.
- Simulate backend failures (500 errors, timeouts) to test app error handling.

---

## Topics

### Selenium Manager

- What it solves: driver/browser version mismatches, no manual ChromeDriver downloads.
- Where it can still fail: offline environments, custom driver paths, non-standard browser installs.
- Override driver path when required using `ChromeDriverService`.

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

var options = new ChromeOptions();
var service = ChromeDriverService.CreateDefaultService("C:\\path\\to\\chromedriver");
var driver = new ChromeDriver(service, options);
```

### WireMock.Net

- A mock HTTP server for stubbing backend responses.
- Install and run locally (CLI tool, Docker, or embedded in tests).
- Create stub mappings for API endpoints your UI depends on.
- Simulate delays, 500 errors, and empty responses.

```json
{
  "Request": {
    "Method": "POST",
    "Url": "/api/login"
  },
  "Response": {
    "StatusCode": 200,
    "Headers": { "Content-Type": "application/json" },
    "BodyAsJson": { "token": "abc123", "user": "admin" }
  }
}
```

---

## Practice

1. Review Selenium Manager behavior under offline conditions.
2. Install WireMock.Net and start it locally:
   ```bash
   # Docker
   docker run -d -p 8080:8080 wiremock/wiremock
   ```
3. Create a WireMock stub mapping for an API endpoint (e.g., `/api/user`).
4. Build a test that hits the WireMock-stubbed endpoint instead of a real API.
5. Build a test that intentionally simulates a slow/failed response to test your app's error handling.

---

## Skills to Load

- `selenium-setup` — Selenium Manager details and offline fallback strategies.

---

## Milestone

A test that proves UI behavior under a *mocked* backend failure (e.g., 500 error, timeout) — a scenario that's hard to test against a real server.
