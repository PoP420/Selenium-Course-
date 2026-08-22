# Actions API Deep Dive

Selenium's Actions API lets you simulate real user interactions — hover, drag, scroll, key combos. Master this and you can automate any UI interaction pattern.

---

## The Actions Class

```java
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.WebElement;

Actions actions = new Actions(driver);
```

All Actions methods return the `Actions` instance, so you can **chain** them:

```java
actions.moveToElement(element).click().perform();
```

`perform()` executes the entire action sequence. Without it, nothing happens.

---

## Click Actions

### Basic click

```java
WebElement button = driver.findElement(By.id("submit"));
button.click(); // simple, direct
```

### Actions click (for intercepted elements)

```java
WebElement button = driver.findElement(By.id("submit"));
actions.moveToElement(button).click().perform();
```

Use `Actions.click()` when a regular `WebElement.click()` throws `ElementClickInterceptedException`.

### Double-click

```java
WebElement item = driver.findElement(By.cssSelector(".item"));
actions.doubleClick(item).perform();
```

### Right-click (context menu)

```java
WebElement item = driver.findElement(By.cssSelector(".item"));
actions.contextClick(item).perform();
```

---

## Form Fill Actions

### Type with pause (human-like)

```java
WebElement input = driver.findElement(By.id("email"));
actions.click(input)
    .keyDown(Keys.CONTROL).sendKeys("a").keyUp(Keys.CONTROL)
    .sendKeys("test@example.com")
    .perform();
```

### Clear and type

```java
WebElement input = driver.findElement(By.id("email"));
input.clear();
input.sendKeys("test@example.com");
```

### Key combos

```java
WebElement input = driver.findElement(By.id("message"));
actions.keyDown(Keys.CONTROL).sendKeys("a").keyUp(Keys.CONTROL).perform(); // Ctrl+A
actions.keyDown(Keys.CONTROL).sendKeys("c").keyUp(Keys.CONTROL).perform(); // Ctrl+C
actions.keyDown(Keys.CONTROL).sendKeys("v").keyUp(Keys.CONTROL).perform(); // Ctrl+V
```

---

## Dropdowns

### Using Select class

```java
import org.openqa.selenium.support.ui.Select;

WebElement dropdown = driver.findElement(By.id("country"));
Select select = new Select(dropdown);

select.selectByValue("us");           // <option value="us">
select.selectByVisibleText("United States");
select.selectByIndex(0);              // first option
```

### Multi-select

```java
Select multiSelect = new Select(driver.findElement(By.id("skills")));
multiSelect.selectByValue("java");
multiSelect.selectByValue("selenium");

List<WebElement> selected = multiSelect.getAllSelectedOptions();
```

---

## Hover Actions

```java
WebElement menuItem = driver.findElement(By.cssSelector(".nav-item"));
actions.moveToElement(menuItem).perform();

// Hover and click submenu
WebElement submenu = driver.findElement(By.cssSelector(".submenu-item"));
actions.moveToElement(menuItem).moveToElement(submenu).click().perform();
```

---

## Drag and Drop

### Method 1: clickAndHold + moveToElement + release

```java
WebElement source = driver.findElement(By.id("draggable"));
WebElement target = driver.findElement(By.id("droppable"));

actions.clickAndHold(source)
    .moveToElement(target)
    .release()
    .perform();
```

### Method 2: dragAndDrop

```java
actions.dragAndDrop(source, target).perform();
```

### Method 3: with offset (pixel-based)

```java
actions.clickAndHold(source)
    .moveByOffset(200, 0)  // move 200px right
    .release()
    .perform();
```

---

## Scroll Actions

### Scroll to element

```java
WebElement footer = driver.findElement(By.tagName("footer"));
((JavascriptExecutor) driver).executeScript("arguments[0].scrollIntoView(true);", footer);
```

### Scroll by pixels

```java
((JavascriptExecutor) driver).executeScript("window.scrollBy(0, 500)");
```

### Scroll to bottom

```java
((JavascriptExecutor) driver).executeScript("window.scrollTo(0, document.body.scrollHeight)");
```

### Actions-based scroll

```java
actions.scrollToElement(footer).perform();
```

---

## Keyboard Actions

### Special keys

```java
Keys.TAB
Keys.ENTER
Keys.SPACE
Keys.ESCAPE
Keys.PAGE_DOWN
Keys.PAGE_UP
Keys.ARROW_UP / ARROW_DOWN / ARROW_LEFT / ARROW_RIGHT
Keys.CONTROL / COMMAND / SHIFT / ALT
Keys.DELETE / BACK_SPACE
```

### Key sequences

```java
actions.sendKeys(Keys.TAB)
    .sendKeys("John")
    .sendKeys(Keys.TAB)
    .sendKeys("Smith")
    .sendKeys(Keys.TAB)
    .sendKeys(Keys.ENTER)
    .perform();
```

---

## Touch Actions (Mobile)

```java
import org.openqa.selenium.interactions.PointerInput;
import org.openqa.selenium.interactions.Sequence;

PointerInput finger = new PointerInput(PointerInput.Kind.TOUCH, "finger");
Sequence swipe = new Sequence(finger, 0);
swipe.addAction(finger.createPointerMove(Duration.ZERO, PointerInput.Origin.viewport(), 500, 1000));
swipe.addAction(finger.createPointerDown(PointerInput.MouseButton.LEFT.asArg()));
swipe.addAction(finger.createPointerMove(Duration.ofMillis(300), PointerInput.Origin.viewport(), 500, 200));
swipe.addAction(finger.createPointerUp(PointerInput.MouseButton.LEFT.asArg()));
driver.perform(Arrays.asList(swipe));
```

---

## Complete Action Patterns

### Login flow

```java
actions.click(emailField)
    .sendKeys("user@test.com")
    .click(passwordField)
    .sendKeys("password123")
    .click(loginButton)
    .perform();
```

### Multi-step form navigation

```java
actions.sendKeys(Keys.TAB)
    .sendKeys("John")
    .sendKeys(Keys.TAB)
    .sendKeys("Smith")
    .sendKeys(Keys.TAB)
    .sendKeys(Keys.ENTER)
    .perform();
```

### File upload (non-input)

```java
// For non-input file uploads
WebElement dropzone = driver.findElement(By.id("dropzone"));
String filePath = "C:\\path\\to\\file.pdf";
((JavascriptExecutor) driver).executeScript(
    "arguments[0].scrollIntoView(true);", dropzone
);
```

---

## Best Practices Summary

1. **Use `perform()`** — forgetting it means nothing executes.
2. **Chain actions** — more readable than separate calls.
3. **Prefer `Actions.click()`** when `WebElement.click()` fails due to interception.
4. **Hover before clicking** — some menus require hover before revealing sub-items.
5. **Scroll into view** for elements outside the viewport.
6. **Use Select for dropdowns** — don't click `<option>` elements directly.

---

## Interview Questions

1. "How do you drag and drop an element? What are the three methods?"
2. "What's the difference between `WebElement.click()` and `Actions.click()`?"
3. "How do you upload a file when the input is hidden?"
4. "How do you hover over a menu and click a submenu item?"
5. "What's the `perform()` method and what happens if you forget it?"
