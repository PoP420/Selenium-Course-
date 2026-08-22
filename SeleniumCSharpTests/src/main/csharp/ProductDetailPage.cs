using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class ProductDetailPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private readonly By productNameLoc = By.CssSelector("[data-test='product-name']");
    private readonly By productPriceLoc = By.CssSelector("[data-test='unit-price']");
    private readonly By productDescriptionLoc = By.CssSelector("[data-test='product-description']");
    private readonly By quantityInputLoc = By.CssSelector("[data-test='quantity']");
    private readonly By addToCartBtnLoc = By.CssSelector("button[data-test='add-to-cart']");
    private readonly By addToFavoritesBtnLoc = By.CssSelector("button[data-test='add-to-favorites']");
    private readonly By addToCompareBtnLoc = By.CssSelector("button[data-test='add-to-compare']");

    public ProductDetailPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public string GetProductName()
    {
        return wait.Until(d =>
        {
            var el = d.FindElement(productNameLoc);
            return el.Displayed && !string.IsNullOrEmpty(el.Text) ? el : null;
        }).Text;
    }

    public string GetProductPrice()
    {
        return wait.Until(d =>
        {
            var el = d.FindElement(productPriceLoc);
            return el.Displayed && !string.IsNullOrEmpty(el.Text) ? el : null;
        }).Text;
    }

    public string GetProductDescription()
    {
        return wait.Until(d =>
        {
            var el = d.FindElement(productDescriptionLoc);
            return el.Displayed && !string.IsNullOrEmpty(el.Text) ? el : null;
        }).Text;
    }

    public bool IsAddToCartButtonDisplayed()
    {
        var el = wait.Until(d =>
        {
            var e = d.FindElement(addToCartBtnLoc);
            return e.Displayed && e.Enabled ? e : null;
        });
        return el.Enabled && el.Displayed;
    }

    public bool IsAddToFavoritesButtonDisplayed()
    {
        var el = wait.Until(d =>
        {
            var e = d.FindElement(addToFavoritesBtnLoc);
            return e.Displayed && e.Enabled ? e : null;
        });
        return el.Enabled && el.Displayed;
    }

    public bool IsAddToCompareButtonDisplayed()
    {
        var el = wait.Until(d =>
        {
            var e = d.FindElement(addToCompareBtnLoc);
            return e.Displayed && e.Enabled ? e : null;
        });
        return el.Enabled && el.Displayed;
    }

    public void SetQuantity(int quantity)
    {
        var input = wait.Until(d => { var el = d.FindElement(quantityInputLoc); return el.Displayed && el.Enabled ? el : null; });
        input.Clear();
        input.SendKeys(quantity.ToString());
    }

    public void ClickAddToCart()
    {
        var btn = wait.Until(d =>
        {
            var el = d.FindElement(addToCartBtnLoc);
            return el.Enabled && el.Displayed ? el : null;
        });
        btn.Click();
    }

    public void ClickAddToFavorites()
    {
        var btn = wait.Until(d =>
        {
            var el = d.FindElement(addToFavoritesBtnLoc);
            return el.Enabled && el.Displayed ? el : null;
        });
        btn.Click();
    }

    public void ClickAddToCompare()
    {
        var btn = wait.Until(d =>
        {
            var el = d.FindElement(addToCompareBtnLoc);
            return el.Enabled && el.Displayed ? el : null;
        });
        btn.Click();
    }
}
