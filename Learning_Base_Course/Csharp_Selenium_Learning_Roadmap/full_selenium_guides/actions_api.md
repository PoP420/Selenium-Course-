# Actions API Deep Dive

Selenium's Actions API lets you simulate real user interactions — hover, drag, scroll, key combos. Master this and you can automate any UI interaction pattern.

---

## The Actions Class

```csharp
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

Actions actions = new Actions(driver);
```

All Actions methods return the `Actions` instance, so you can **chain** them:

```csharp
actions.MoveToElement(element).Click().Perform();
```

`Perform()` executes the entire action sequence. Without it, nothing happens.

---

## Click Actions

### Basic click

```csharp
IWebElement button = driver.FindElement(By.Id("submit"));
button.Click(); // simple, direct
```

### Actions click (for intercepted elements)

```csharp
IWebElement button = driver.FindElement(By.Id("submit"));
actions.MoveToElement(button).Click().Perform();
```

Use `Actions.Click()` when a regular `IWebElement.Click()` throws `ElementClickInterceptedException`.

### Double-click

```csharp
IWebElement item = driver.FindElement(By.CssSelector(".item"));
actions.DoubleClick(item).Perform();
```

### Right-click (context menu)

```csharp
IWebElement item = driver.FindElement(By.CssSelector(".item"));
actions.ContextClick(item).Perform();
```

---

## Form Fill Actions

### Type with pause (human-like)

```csharp
IWebElement input = driver.FindElement(By.Id("email"));
actions.Click(input)
    .KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control)
    .SendKeys("test@example.com")
    .Perform();
```

### Clear and type

```csharp
IWebElement input = driver.FindElement(By.Id("email"));
input.Clear();
input.SendKeys("test@example.com");
```

### Key combos

```csharp
IWebElement input = driver.FindElement(By.Id("message"));
actions.KeyDown(Keys.Control).SendKeys("a").KeyUp(Keys.Control).Perform(); // Ctrl+A
actions.KeyDown(Keys.Control).SendKeys("c").KeyUp(Keys.Control).Perform(); // Ctrl+C
actions.KeyDown(Keys.Control).SendKeys("v").KeyUp(Keys.Control).Perform(); // Ctrl+V
```

---

## Dropdowns

### Using SelectElement class

```csharp
using OpenQA.Selenium.Support.UI;

IWebElement dropdown = driver.FindElement(By.Id("country"));
SelectElement select = new SelectElement(dropdown);

select.SelectByValue("us");           // <option value="us">
select.SelectByText("United States");
select.SelectByIndex(0);              // first option
```

### Multi-select

```csharp
SelectElement multiSelect = new SelectElement(driver.FindElement(By.Id("skills")));
multiSelect.SelectByValue("java");
multiSelect.SelectByValue("selenium");

IReadOnlyCollection<IWebElement> selected = multiSelect.AllSelectedOptions;
```

---

## Hover Actions

```csharp
IWebElement menuItem = driver.FindElement(By.CssSelector(".nav-item"));
actions.MoveToElement(menuItem).Perform();

// Hover and click submenu
IWebElement submenu = driver.FindElement(By.CssSelector(".submenu-item"));
actions.MoveToElement(menuItem).MoveToElement(submenu).Click().Perform();
```

---

## Drag and Drop

### Method 1: ClickAndHold + MoveToElement + Release

```csharp
IWebElement source = driver.FindElement(By.Id("draggable"));
IWebElement target = driver.FindElement(By.Id("droppable"));

actions.ClickAndHold(source)
    .MoveToElement(target)
    .Release()
    .Perform();
```

### Method 2: DragAndDrop

```csharp
actions.DragAndDrop(source, target).Perform();
```

### Method 3: With offset (pixel-based)

```csharp
actions.ClickAndHold(source)
    .MoveByOffset(200, 0)  // move 200px right
    .Release()
    .Perform();
```

---

## Scroll Actions

### Scroll to element

```csharp
IWebElement footer = driver.FindElement(By.TagName("footer"));
((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", footer);
```

### Scroll by pixels

```csharp
((IJavaScriptExecutor)driver).ExecuteScript("window.scrollBy(0, 500)");
```

### Scroll to bottom

```csharp
((IJavaScriptExecutor)driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
```

---

## Keyboard Actions

### Special keys

```csharp
Keys.Tab
Keys.Enter
Keys.Space
Keys.Escape
Keys.PageDown
Keys.PageUp
Keys.ArrowUp / ArrowDown / ArrowLeft / ArrowRight
Keys.Control / Shift / Alt
Keys.Delete / BackSpace
```

### Key sequences

```csharp
actions.SendKeys(Keys.Tab)
    .SendKeys("John")
    .SendKeys(Keys.Tab)
    .SendKeys("Smith")
    .SendKeys(Keys.Tab)
    .SendKeys(Keys.Enter)
    .Perform();
```

---

## Best Practices Summary

1. **Use `Perform()`** — forgetting it means nothing executes.
2. **Chain actions** — more readable than separate calls.
3. **Prefer `Actions.Click()`** when `IWebElement.Click()` fails due to interception.
4. **Hover before clicking** — some menus require hover before revealing sub-items.
5. **Scroll into view** for elements outside the viewport.
6. **Use SelectElement for dropdowns** — don't click `<option>` elements directly.

---

## Interview Questions

1. "How do you drag and drop an element? What are the three methods?"
2. "What's the difference between `IWebElement.Click()` and `Actions.Click()`?"
3. "How do you upload a file when the input is hidden?"
4. "How do you hover over a menu and click a submenu item?"
5. "What's the `Perform()` method and what happens if you forget it?"
