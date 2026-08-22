# Actions Mastery

Playwright's action API simulates real user interactions — click, type, hover, drag, scroll, keyboard, and more. All actions auto-wait for element readiness.

---

## The Action API

All Playwright actions are `async` and auto-wait:

```csharp
using Microsoft.Playwright;

// Click
await page.ClickAsync("button[type='submit']");

// Fill input (clears + types)
await page.FillAsync("#email", "test@example.com");

// Hover
await page.HoverAsync(".nav-item");

// Press keyboard key
await page.Keyboard.PressAsync("Control+A");
```

---

## Click Actions

### Basic click

```csharp
await page.Locator("#submit").ClickAsync();
```

### Click with options

```csharp
await page.Locator("#submit").ClickAsync(new()
{
    Button = MouseButton.Right,   // right-click
    ClickCount = 2,               // double-click
    Position = new() { X = 10, Y = 10 }, // click at offset
});
```

### Actions-based click (for intercepted elements)

```csharp
await page.Locator("#submit").ClickAsync(new()
{
    Force = true,  // bypasses actionability checks — use sparingly
});
```

Use `Force = true` when a regular click throws because of an overlay. Prefer fixing the root cause (wait for overlay to disappear) over `Force`.

---

## Form Fill Actions

### Fill input

```csharp
await page.Locator("#email").FillAsync("test@example.com");
```

- `FillAsync()` clears the input first, then types. Use this for single-line inputs.
- For multi-line text areas, use `FillAsync` too — it handles newlines.

### Keyboard actions

```csharp
// Type character-by-character (with human delay)
await page.Locator("#message").TypeAsync("Hello World", new()
{
    Delay = 100,  // ms between keystrokes
});

// Key combos
await page.Keyboard.PressAsync("Control+A");
await page.Keyboard.PressAsync("Control+C");
await page.Keyboard.PressAsync("Control+V");

// Special keys
await page.Keyboard.PressAsync(Keys.Enter);
await page.Keyboard.PressAsync(Keys.Tab);
await page.Keyboard.PressAsync(Keys.Escape);
```

### Special keys reference

| Key | Syntax |
|---|---|
| Enter | `Keys.Enter` |
| Tab | `Keys.Tab` |
| Escape | `Keys.Escape` |
| Backspace | `Keys.Backspace` |
| Delete | `Keys.Delete` |
| Arrow keys | `Keys.ArrowUp` / `Keys.ArrowDown` / `Keys.ArrowLeft` / `Keys.ArrowRight` |
| Control/Cmd | `Keys.Control` |
| Shift | `Keys.Shift` |
| Alt | `Keys.Alt` |
| Page Up/Down | `Keys.PageUp` / `Keys.PageDown` |

---

## Dropdown Selection

```csharp
// Select by value
await page.SelectOptionAsync("select#country", new() { Value = "us" });

// Select by label
await page.SelectOptionAsync("select#country", new() { Label = "United States" });

// Select by index
await page.SelectOptionAsync("select#country", new() { Index = 2 });

// Select multiple options (multi-select)
await page.SelectOptionAsync("select#skills", new() { Value = "java", Index = 1 });
```

---

## Drag and Drop

### Basic drag and drop

```csharp
await page.DragAndDropAsync("#draggable", "#droppable");
```

### With source and target locators

```csharp
await page.Locator("#draggable").DragAsync();
await page.Locator("#droppable").DropAsync();
```

### With offset

```csharp
await page.DragAndDropAsync("#draggable", "#droppable", new()
{
    SourcePosition = new Position { X = 10, Y = 10 },
    TargetPosition = new Position { X = 50, Y = 50 },
});
```

---

## Scroll Actions

### Scroll to element (auto)

```csharp
// Actions auto-scroll into view
await page.Locator("#submit").ClickAsync();
```

### Explicit scroll

```csharp
await page.Locator("#footer").ScrollIntoViewIfNeededAsync();
await page.EvalOnSelectorAsync("window.scrollBy(0, 500)", "scrollBy");
await page.EvaluateAsync("window.scrollTo(0, document.body.scrollHeight)");
```

---

## Mouse Actions

```csharp
using Microsoft.Playwright;

// Move mouse to coordinates
await page.Mouse.MoveAsync(100, 200);

// Mouse down/up
await page.Mouse.DownAsync();
await page.Mouse.UpAsync();

// Wheel scroll
await page.Mouse.WheelAsync(0, 500);
```

---

## Touch Actions (Mobile)

```csharp
// Simulate touch tap
await page.Locator("#button").ClickAsync(new() { Force = true });

// Or use the Touchscreen class (if available)
await page.Touchscreen.TapAsync(100, 200);
```

> Playwright's touch support is limited compared to Selenium's mobile APIs. For native mobile, use Appium.

---

## File Upload

### Standard input file upload

```csharp
// Single file
await page.Locator("input[type='file']").SetInputFilesAsync("path/to/file.pdf");

// Multiple files
await page.Locator("input[type='file']").SetInputFilesAsync(new[]
{
    "path/to/file1.pdf", "path/to/file2.pdf"
});

// From byte array (no disk write)
await page.Locator("input[type='file']").SetInputFilesAsync(new()
{
    Name = "test.txt",
    Buffer = Encoding.UTF8.GetBytes("Hello World"),
    MimeType = "text/plain"
});
```

### Non-input file upload (drag-and-drop simulation)

```csharp
// Drop files onto a dropzone
var fileList = new List<FilePayload>
{
    new FilePayload
    {
        Name = "test.txt",
        Buffer = Encoding.UTF8.GetBytes("Hello World"),
        MimeType = "text/plain"
    }
};

// Use the drop zone locator
await page.Locator(".dropzone").OnFileChooserAsync(async fileChooser =>
{
    await fileChooser.AcceptAsync(fileList);
});

// Or use page.route to simulate
await page.Locator(".dropzone").DropAsync(new()
{
    // ...
});
```

---

## File Download

```csharp
// Click a link that triggers download
var downloadTask = page.WaitForDownloadAsync();
await page.Locator("a[href='/download/report.pdf']").ClickAsync();
var download = await downloadTask;

// Save to disk
await download.SaveAsAsync("downloads/report.pdf");

// Read as buffer
var buffer = await download.CreateReadStreamAsync();
```

---

## Complete Action Patterns

### Login flow

```csharp
await page.Locator("#email").FillAsync("user@test.com");
await page.Locator("#password").FillAsync("password123");
await page.Locator("button[type='submit']").ClickAsync();
```

### Multi-step form navigation

```csharp
await page.Locator("#firstName").FillAsync("John");
await page.Keyboard.PressAsync(Keys.Tab);
await page.Locator("#lastName").FillAsync("Smith");
await page.Keyboard.PressAsync(Keys.Tab);
await page.Keyboard.PressAsync(Keys.Enter);
```

### Form with dropdown and assertion

```csharp
await page.Locator("#firstName").FillAsync("John");
await page.Locator("#lastName").FillAsync("Smith");
await page.SelectOptionAsync("#subject", new() { Value = "webmaster" });
await page.Locator("#message").FillAsync("Hello World");
await page.Locator(".btnSubmit").ClickAsync();

await Expect(page.Locator(".alert")).ToContainTextAsync("Thanks for your message!");
```

---

## Best Practices Summary

1. **Use `FillAsync` for inputs** — it clears and types in one step. Use `TypeAsync` only when you need per-key delays.
2. **Use `Force = true` sparingly** — fix the root cause (overlay, animation) instead.
3. **Prefer `GetByRole` locators** — they're the most resilient to DOM changes.
4. **Let actions auto-scroll** — Playwright scrolls elements into view automatically.
5. **Use `DragAndDropAsync`** for simple drag — don't roll your own with mouse events.
6. **Use `SetInputFilesAsync`** for file uploads — it handles the browser's file picker natively.
7. **Await downloads with `WaitForDownloadAsync`** — don't guess the timing.

---

## Interview Questions

1. "How does Playwright's `ClickAsync` differ from Selenium's `WebElement.click()`?"
2. "When would you use `FillAsync` vs `TypeAsync`?"
3. "How do you upload a file when the input is hidden behind a dropzone?"
4. "How do you drag and drop in Playwright?"
5. "What happens if you forget `await` on an action?"
6. "How does Playwright handle scrolling before a click?"
7. "What's the difference between `Mouse.PressAsync` and `Locator.ClickAsync`?"
