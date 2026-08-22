using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumCSharpTests.Tests;

public class TestBase
{
    protected IWebDriver driver;
    protected WebDriverWait wait;

    [SetUp]
    public void SetUp()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        driver = new ChromeDriver(options);
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    [TearDown]
    public void TearDown()
    {
        driver?.Dispose();
    }
}
