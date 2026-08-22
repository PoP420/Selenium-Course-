using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class LoginPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private readonly By emailLoc = By.Id("email");
    private readonly By passwordLoc = By.Id("password");
    private readonly By btnLoginLoc = By.CssSelector("[data-test='login-submit']");
    private readonly By homeLinkLoc = By.CssSelector("a[data-test='nav-home']");

    public LoginPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void Login(string email, string password)
    {
        wait.Until(d => { var el = d.FindElement(emailLoc); return el.Displayed ? el : null; }).SendKeys(email);
        wait.Until(d => { var el = d.FindElement(passwordLoc); return el.Displayed ? el : null; }).SendKeys(password);
        wait.Until(d =>
        {
            var el = d.FindElement(btnLoginLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public void LoginAndWaitForHome(string email, string password)
    {
        Login(email, password);
        var redirectWait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        redirectWait.Until(d =>
        {
            var el = d.FindElement(homeLinkLoc);
            return el.Displayed && el.Enabled ? el : null;
        });
    }
}
