# Alerts, Popups, Windows & Frames

Interacting with browser-level UI elements — alerts, popups, tabs, iframes — is a common interview topic and a real pain point in automation. This guide covers every pattern.

---

## JavaScript Alerts

### Accept / Dismiss / Get Text

```csharp
// Wait for alert
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
IAlert alert = wait.Until(d => _ = d.SwitchTo().Alert());

// Get text
string alertText = alert.Text;

// Accept (OK button)
alert.Accept();

// Dismiss (Cancel button)
alert.Dismiss();

// Send text (for prompt dialogs)
alert.SendKeys("some text");
alert.Accept();
```

### Types of JS alerts

| Alert Type | Buttons | Method |
|---|---|---|
| `alert()` | OK | `Accept()` |
| `confirm()` | OK + Cancel | `Accept()` or `Dismiss()` |
| `prompt()` | OK + Cancel + Text input | `SendKeys()` then `Accept()` or `Dismiss()` |

---

## Authentication Popups (Basic Auth)

### URL-based (preferred)

```csharp
// Embed credentials in URL
driver.Navigate().GoToUrl("https://username:password@example.com");
```

---

## Window / Tab Switching

### Get all windows

```csharp
string originalWindow = driver.CurrentWindowHandle;
ReadOnlyCollection<string> allWindows = driver.WindowHandles;
```

### Switch to new window

```csharp
string originalWindow = driver.CurrentWindowHandle;

// Click that opens a new tab/window
driver.FindElement(By.Id("open-new-tab")).Click();

var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
wait.Until(d => d.WindowHandles.Count > 1);

foreach (string windowHandle in driver.WindowHandles)
{
    if (!windowHandle.Equals(originalWindow))
    {
        driver.SwitchTo().Window(windowHandle);
        break;
    }
}

// Now interact with the new window
Console.WriteLine(driver.Title);

// Close and switch back
driver.Close();
driver.SwitchTo().Window(originalWindow);
```

### Window operations

```csharp
// Resize
driver.Manage().Window.Size = new System.Drawing.Size(1920, 1080);

// Maximize
driver.Manage().Window.Maximize();

// Minimize
driver.Manage().Window.Minimize();

// Fullscreen
driver.Manage().Window.FullScreen();
```

---

## Iframe Switching

### Switch to iframe by locator

```csharp
// Switch to iframe
var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
wait.Until(d =>
{
    try
    {
        d.SwitchTo().Frame(By.Id("payment-frame"));
        return true;
    }
    catch (NoSuchFrameException)
    {
        return false;
    }
});

// Interact with elements inside iframe
driver.FindElement(By.Id("card-number")).SendKeys("4111111111111111");

// Switch back to main document
driver.SwitchTo().DefaultContent();
```

### Switch by index or name

```csharp
driver.SwitchTo().Frame(0);              // first iframe
driver.SwitchTo().Frame("payment-frame"); // by name/id
```

### Nested iframes

```csharp
driver.SwitchTo().Frame("outer-frame");
driver.SwitchTo().Frame("inner-frame");
// interact with inner frame element
driver.SwitchTo().ParentContent(); // back to outer frame
driver.SwitchTo().DefaultContent(); // back to main document
```

---

## Shadow DOM (Selenium 4+)

### Access shadow root

```csharp
// Find the shadow host element
IWebElement shadowHost = driver.FindElement(By.CssSelector("my-custom-element"));

// Get the shadow root
ISearchContext shadowRoot = shadowHost.GetShadowRoot();

// Find elements inside shadow DOM
IWebElement innerButton = shadowRoot.FindElement(By.CssSelector(".inner-button"));
innerButton.Click();
```

### Nested shadow DOM

```csharp
IWebElement outerHost = driver.FindElement(By.CssSelector("outer-element"));
ISearchContext outerRoot = outerHost.GetShadowRoot();
IWebElement innerHost = outerRoot.FindElement(By.CssSelector("inner-element"));
ISearchContext innerRoot = innerHost.GetShadowRoot();
IWebElement deepElement = innerRoot.FindElement(By.CssSelector(".deep-element"));
```

---

## File Upload

### Standard input file upload

```csharp
// The input[type="file"] element
IWebElement fileInput = driver.FindElement(By.Id("file-upload"));
fileInput.SendKeys("C:\\path\\to\\file.pdf");
driver.FindElement(By.Id("upload-btn")).Click();
```

### Non-input file upload

```csharp
// For dropzones that don't use <input type="file">
IWebElement dropzone = driver.FindElement(By.Id("dropzone"));
string filePath = "C:\\path\\to\\file.pdf";

IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
js.ExecuteScript("arguments[0].scrollIntoView(true);", dropzone);
// Note: true drag-and-drop via JS is complex; prefer standard input uploads
```

---

## File Download

### Configure Chrome to auto-download

```csharp
var options = new ChromeOptions();
string downloadPath = "C:\\Users\\ajdpe\\Downloads";

var chromePrefs = new Dictionary<string, object>
{
    { "download.default_directory", downloadPath },
    { "download.prompt_for_download", false },
    { "profile.default_content_settings.popups", 0 }
};

options.AddUserProfilePreference("download", chromePrefs);
var driver = new ChromeDriver(options);
```

### Verify download

```csharp
var downloadDir = new DirectoryInfo("C:\\Users\\ajdpe\\Downloads");
FileInfo[] files = downloadDir.GetFiles("*.pdf");
Assert.That(files.Length, Is.GreaterThan(0));
```

---

## Cookie Management

### Get / Set / Delete cookies

```csharp
// Add cookie
var cookie = new Cookie("sessionId", "abc123", ".example.com", "/", DateTime.Now.AddHours(1));
driver.Manage().Cookies.AddCookie(cookie);

// Get cookie
Cookie session = driver.Manage().Cookies.GetCookieNamed("sessionId");

// Delete cookie
driver.Manage().Cookies.DeleteCookieNamed("sessionId");

// Delete all cookies
driver.Manage().Cookies.DeleteAllCookies();
```

---

## Best Practices Summary

1. **Always wait for alerts** — use `WebDriverWait` + `driver.SwitchTo().Alert()`.
2. **Always switch back** — after iframe work, call `DefaultContent()`.
3. **Track original window** — store `CurrentWindowHandle` before opening new tabs.
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
