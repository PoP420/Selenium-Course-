using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace SeleniumCSharpTests.Pages;

public class HomeSortingPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;
    private readonly IJavaScriptExecutor js;

    private readonly By sortSelectLoc = By.CssSelector("select[data-test='sort']");

    public HomeSortingPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        this.js = (IJavaScriptExecutor)driver;
    }

    public void Navigate()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/");
    }

    public void SortBy(string sortValue)
    {
        var selectElement = wait.Until(d =>
        {
            var el = d.FindElement(sortSelectLoc);
            return el.Displayed && el.Enabled ? el : null;
        });

        selectElement.Click();

        try
        {
            var option = selectElement.FindElement(By.CssSelector($"option[value='{sortValue}']"));
            option.Click();
        }
        catch (NoSuchElementException)
        {
            var select = new SelectElement(selectElement);
            select.SelectByValue(sortValue);
        }

        for (int attempt = 0; attempt < 3; attempt++)
        {
            Thread.Sleep(1500);
            WaitForProductsStable();

            var names = GetProductNames();
            var prices = GetProductPricesAsDecimals();
            bool isSorted = sortValue switch
            {
                "name,asc" => names.SequenceEqual(names.OrderBy(n => n, StringComparer.Ordinal).ToList()),
                "name,desc" => names.SequenceEqual(names.OrderByDescending(n => n, StringComparer.Ordinal).ToList()),
                "price,asc" => prices.SequenceEqual(prices.OrderBy(p => p).ToList()),
                "price,desc" => prices.SequenceEqual(prices.OrderByDescending(p => p).ToList()),
                _ => true
            };

            if (isSorted) return;

            js.ExecuteScript(@"
                var select = arguments[0];
                var value = arguments[1];
                var nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLSelectElement.prototype, 'value').set;
                nativeInputValueSetter.call(select, value);
                var event = new Event('change', { bubbles: true });
                select.dispatchEvent(event);
            ", selectElement, sortValue);
        }
    }

    public void SortByNameAscending()
    {
        SortBy("name,asc");
    }

    public void SortByNameDescending()
    {
        SortBy("name,desc");
    }

    public void SortByPriceLowToHigh()
    {
        SortBy("price,asc");
    }

    public void SortByPriceHighToLow()
    {
        SortBy("price,desc");
    }

    public void WaitForProducts()
    {
        wait.Until(d =>
        {
            try
            {
                var elements = d.FindElements(By.CssSelector("a.card[data-test^='product-']"));
                return elements.Count > 0 && elements[0].Displayed ? elements[0] : null;
            }
            catch (StaleElementReferenceException)
            {
                return null;
            }
        });
    }

    public void WaitForProductsStable()
    {
        WaitForProducts();
        var stableWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        string prevSignature = "";
        stableWait.Until(d =>
        {
            var current = GetProductSignature();
            if (current == prevSignature && current.Length > 0)
            {
                return true;
            }
            prevSignature = current;
            return false;
        });
    }

    private string GetProductSignature()
    {
        var result = js.ExecuteScript(
            "return Array.from(document.querySelectorAll('a.card[data-test^=\"product-\"]'))" +
            ".map(card => { const el = card.querySelector('[data-test=\"product-name\"]'); return el ? el.textContent.trim() : ''; })" +
            ".filter(text => text.length > 0).join('|');"
        )?.ToString() ?? "";
        return result;
    }

    public int GetProductCount()
    {
        WaitForProductsStable();
        var count = Convert.ToInt32(js.ExecuteScript(
            "return document.querySelectorAll('a.card[data-test^=\"product-\"]').length;") ?? 0);
        return count;
    }

    public IList<string> GetProductNames()
    {
        return GetProductDataViaJs("product-name");
    }

    public IList<string> GetProductPrices()
    {
        return GetProductDataViaJs("product-price");
    }

    public IList<decimal> GetProductPricesAsDecimals()
    {
        return GetProductPrices()
            .Select(p => new string(p.Where(c => char.IsDigit(c) || c == '.').ToArray()))
            .Select(p => decimal.Parse(p, CultureInfo.InvariantCulture))
            .ToList();
    }

    private IList<string> GetProductDataViaJs(string dataTest)
    {
        WaitForProductsStable();
        var result = js.ExecuteScript(
            "return Array.from(document.querySelectorAll('a.card[data-test^=\"product-\"]'))" +
            ".map(card => { const el = card.querySelector('[data-test=\"" + dataTest + "\"]'); return el ? el.textContent.trim() : ''; })" +
            ".filter(text => text.length > 0);"
        );
        if (result == null) return new List<string>();
        return ((IEnumerable)result).Cast<object>().Select(o => o?.ToString() ?? "").ToList();
    }

    public void Search(string term)
    {
        wait.Until(d => { var el = d.FindElement(By.CssSelector("[data-test='search-query']")); return el.Displayed ? el : null; }).Clear();
        wait.Until(d => { var el = d.FindElement(By.CssSelector("[data-test='search-query']")); return el.Displayed ? el : null; }).SendKeys(term);

        wait.Until(d =>
        {
            var el = d.FindElement(By.CssSelector("button[data-test='search-submit']"));
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
        WaitForProductsStable();
    }

    public void ResetSearch()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(By.CssSelector("button[data-test='search-reset']"));
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
        WaitForProductsStable();
    }

    public void SelectCategory(string categoryId)
    {
        var cssSelector = $"[data-test='{categoryId}']";

        wait.Until(d =>
        {
            try
            {
                d.FindElement(By.CssSelector(cssSelector));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        });

        js.ExecuteScript(@"
            var el = document.querySelector(arguments[0]);
            if (el) {
                el.scrollIntoView({ behavior: 'instant', block: 'center' });
                el.click();
            }
        ", cssSelector);

        Thread.Sleep(2000);
        WaitForProductsStable();
    }


    public void SelectBrand(string brandId)
    {
        var loc = By.CssSelector($"[data-test='{brandId}']");
        var checkbox = wait.Until(d =>
        {
            var el = d.FindElement(loc);
            return el.Enabled ? el : null;
        });
        checkbox.Click();
        WaitForProductsStable();
    }

    public void SetEcoFriendly(bool check)
    {
        var checkbox = wait.Until(d =>
        {
            var el = d.FindElement(By.CssSelector("[data-test='eco-friendly-filter']"));
            return el.Enabled ? el : null;
        });

        if (check != checkbox.Selected)
        {
            checkbox.Click();
            WaitForProductsStable();
        }
    }
}