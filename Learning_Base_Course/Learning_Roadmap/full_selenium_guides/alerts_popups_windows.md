# Alerts, Popups, Windows & Frames

Interacting with browser-level UI elements — alerts, popups, tabs, iframes — is a common interview topic and a real pain point in automation. This guide covers every pattern.

---

## JavaScript Alerts

### Accept / Dismiss / Get Text

```java
// Wait for alert
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
Alert alert = wait.until(ExpectedConditions.alertIsPresent());

// Get text
String alertText = alert.getText();

// Accept (OK button)
alert.accept();

// Dismiss (Cancel button)
alert.dismiss();

// Send text (for prompt dialogs)
alert.sendKeys("some text");
alert.accept();
```

### Types of JS alerts

| Alert Type | Buttons | Method |
|---|---|---|
| `alert()` | OK | `accept()` |
| `confirm()` | OK + Cancel | `accept()` or `dismiss()` |
| `prompt()` | OK + Cancel + Text input | `sendKeys()` then `accept()` or `dismiss()` |

---

## Authentication Popups (Basic Auth)

### URL-based (preferred)

```java
// Embed credentials in URL
driver.get("https://username:password@example.com");
```

### AutoIT (Windows fallback)

```java
// When URL-based auth doesn't work
Runtime.getRuntime().exec("path/to/auth-script.exe");
```

---

## Window / Tab Switching

### Get all windows

```java
String originalWindow = driver.getWindowHandle();
Set<String> allWindows = driver.getWindowHandles();
```

### Switch to new window

```java
String originalWindow = driver.getWindowHandle();

// Click that opens a new tab/window
driver.findElement(By.id("open-new-tab")).click();

WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
wait.until(driver -> driver.getWindowHandles().size() > 1);

for (String windowHandle : driver.getWindowHandles()) {
    if (!windowHandle.equals(originalWindow)) {
        driver.switchTo().window(windowHandle);
        break;
    }
}

// Now interact with the new window
System.out.println(driver.getTitle());

// Close and switch back
driver.close();
driver.switchTo().window(originalWindow);
```

### Window operations

```java
// Resize
driver.manage().window().setSize(new Dimension(1920, 1080));

// Maximize
driver.manage().window().maximize();

// Minimize
driver.manage().window().minimize();

// Fullscreen
driver.manage().window().fullscreen();
```

---

## Iframe Switching

### Switch to iframe by locator

```java
// Switch to iframe
WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(10));
wait.until(ExpectedConditions.frameToBeAvailableAndSwitchToIt(By.id("payment-frame")));

// Interact with elements inside iframe
driver.findElement(By.id("card-number")).sendKeys("4111111111111111");

// Switch back to main document
driver.switchTo().defaultContent();
```

### Switch by index or name

```java
driver.switchTo().frame(0);              // first iframe
driver.switchTo().frame("payment-frame"); // by name/id
```

### Nested iframes

```java
driver.switchTo().frame("outer-frame");
driver.switchTo().frame("inner-frame");
// interact with inner frame element
driver.switchTo().parentContent(); // back to outer frame
driver.switchTo().defaultContent(); // back to main document
```

---

## Shadow DOM (Selenium 4+)

### Access shadow root

```java
// Find the shadow host element
WebElement shadowHost = driver.findElement(By.cssSelector("my-custom-element"));

// Get the shadow root
SearchContext shadowRoot = shadowHost.getShadowRoot();

// Find elements inside shadow DOM
WebElement innerButton = shadowRoot.findElement(By.cssSelector(".inner-button"));
innerButton.click();
```

### Nested shadow DOM

```java
WebElement outerHost = driver.findElement(By.cssSelector("outer-element"));
SearchContext outerRoot = outerHost.getShadowRoot();
WebElement innerHost = outerRoot.findElement(By.cssSelector("inner-element"));
SearchContext innerRoot = innerHost.getShadowRoot();
WebElement deepElement = innerRoot.findElement(By.cssSelector(".deep-element"));
```

---

## File Upload

### Standard input file upload

```java
// The input[type="file"] element
WebElement fileInput = driver.findElement(By.id("file-upload"));
fileInput.sendKeys("C:\\path\\to\\file.pdf");
driver.findElement(By.id("upload-btn")).click();
```

### Non-input file upload (drag-and-drop simulation)

```java
// For dropzones that don't use <input type="file">
WebElement dropzone = driver.findElement(By.id("dropzone"));
String filePath = "C:\\path\\to\\file.pdf";

JavascriptExecutor js = (JavascriptExecutor) driver;
js.executeScript(
    "arguments[0].scrollIntoView(true);", dropzone
);
// Note: true drag-and-drop via JS is complex; prefer standard input uploads
```

---

## File Download

### Configure Chrome to auto-download

```java
ChromeOptions options = new ChromeOptions();
String downloadPath = "C:\\Users\\ajdpe\\Downloads";

HashMap<String, Object> chromePrefs = new HashMap<>();
chromePrefs.put("download.default_directory", downloadPath);
chromePrefs.put("download.prompt_for_download", false);
chromePrefs.put("profile.default_content_settings.popups", 0);

options.setExperimentalOption("prefs", chromePrefs);
driver = new ChromeDriver(options);
```

### Verify download

```java
File downloadDir = new File("C:\\Users\\ajdpe\\Downloads");
File[] files = downloadDir.listFiles((dir, name) -> name.endsWith(".pdf"));
assert files != null && files.length > 0;
```

---

## Cookie Management

### Get / Set / Delete cookies

```java
// Add cookie
Cookie cookie = new Cookie("sessionId", "abc123", ".example.com", "/", new Date(System.currentTimeMillis() + 3600000));
driver.manage().addCookie(cookie);

// Get cookie
Cookie session = driver.manage().getCookieNamed("sessionId");

// Delete cookie
driver.manage().deleteCookieNamed("sessionId");

// Delete all cookies
driver.manage().deleteAllCookies();
```

---

## Best Practices Summary

1. **Always wait for alerts** — use `ExpectedConditions.alertIsPresent()`.
2. **Always switch back** — after iframe work, call `defaultContent()`.
3. **Track original window** — store `getWindowHandle()` before opening new tabs.
4. **Close windows you open** — don't leave orphaned tabs.
5. **Shadow DOM needs Selenium 4+** — upgrade if you're on Selenium 3.
6. **File inputs accept absolute paths** — no need for AutoIT if there's a hidden input.

---

## Interview Questions

1. "How do you handle a JavaScript confirm dialog?"
2. "How do you switch between multiple tabs?"
3. "How do you interact with elements inside an iframe?"
4. "How do you handle a shadow DOM element?"
5. "How do you upload a file when the input is hidden?"
