using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class NavBar
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private readonly By homeLinkLoc = By.CssSelector("a[data-test='nav-home']");
    private readonly By categoriesBtnLoc = By.CssSelector("button[data-test='nav-categories']");
    private readonly By contactLinkLoc = By.CssSelector("a[data-test='nav-contact']");
    private readonly By userMenuBtnLoc = By.CssSelector("button[data-test='nav-menu']");
    private readonly By signOutLinkLoc = By.CssSelector("a[data-test='nav-sign-out']");
    private readonly By languageBtnLoc = By.CssSelector("button[data-test='language-select']");
    private readonly By cartQuantityLoc = By.CssSelector("[data-test='cart-quantity']");

    public NavBar(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void ClickHome()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(homeLinkLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void ClickCategories()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(categoriesBtnLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void ClickCategory(string categoryDataTest)
    {
        var loc = By.CssSelector($"a[data-test='{categoryDataTest}']");
        wait.Until(d =>
        {
            var el = d.FindElement(loc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void ClickContact()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(contactLinkLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void OpenUserMenu()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(userMenuBtnLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void ClickSignOut()
    {
        OpenUserMenu();
        wait.Until(d =>
        {
            var el = d.FindElement(signOutLinkLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void OpenLanguageSelector()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(languageBtnLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void SelectLanguage(string langCode)
    {
        var loc = By.CssSelector($"a[data-test='lang-{langCode}']");
        wait.Until(d =>
        {
            var el = d.FindElement(loc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public bool IsUserMenuDisplayed()
    {
        var el = wait.Until(d =>
        {
            TryFindElement(d, userMenuBtnLoc, out var element);
            return element != null && element.Displayed ? element : null;
        });
        return el != null;
    }

    public int GetCartCount()
    {
        try
        {
            var el = wait.Until(d =>
            {
                var element = d.FindElement(cartQuantityLoc);
                return element.Displayed ? element : null;
            });
            var text = el.Text.Trim();
            return int.TryParse(text, out var count) ? count : 0;
        }
        catch (WebDriverTimeoutException)
        {
            return 0;
        }
    }

    private static bool TryFindElement(ISearchContext context, By by, out IWebElement? element)
    {
        try
        {
            element = context.FindElement(by);
            return element != null;
        }
        catch (NoSuchElementException)
        {
            element = null;
            return false;
        }
    }
}
