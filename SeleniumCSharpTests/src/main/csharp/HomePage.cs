using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class HomePage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private readonly By productCardLoc = By.CssSelector("a.card[data-test^='product-']");

    public HomePage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void Navigate()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/");
    }

    public bool IsProductContainerDisplayed()
    {
        var card = wait.Until(d =>
        {
            var elements = d.FindElements(productCardLoc);
            return elements.Count > 0 && elements[0].Displayed ? elements[0] : null;
        });
        return card != null;
    }

    public int GetProductCardCount()
    {
        wait.Until(d =>
        {
            var elements = d.FindElements(productCardLoc);
            return elements.Count > 0 && elements[0].Displayed ? elements[0] : null;
        });
        return driver.FindElements(productCardLoc).Count(e => e.Displayed);
    }

    public void ClickProductCard(int index = 0)
    {
        var card = wait.Until(d =>
        {
            var elements = d.FindElements(productCardLoc);
            return elements.Count > index && elements[index].Displayed && elements[index].Enabled ? elements[index] : null;
        });
        card.Click();
    }

    public void ClickProduct(string productId)
    {
        var loc = By.CssSelector($"a.card[data-test='{productId}']");
        var card = wait.Until(d =>
        {
            var el = d.FindElement(loc);
            return el.Displayed && el.Enabled ? el : null;
        });
        card.Click();
    }
}
