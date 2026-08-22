using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class ContactPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private readonly By firstNameLoc = By.Id("first_name");
    private readonly By lastNameLoc = By.Id("last_name");
    private readonly By emailLoc = By.Id("email");
    private readonly By subjectLoc = By.Id("subject");
    private readonly By messageLoc = By.Id("message");
    private readonly By btnSubmitLoc = By.ClassName("btnSubmit");
    private readonly By alertLoc = By.ClassName("alert");

    public ContactPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void FillForm(string firstName, string lastName, string email, string message)
    {
        wait.Until(d => { var el = d.FindElement(firstNameLoc); return el.Displayed ? el : null; }).SendKeys(firstName);
        wait.Until(d => { var el = d.FindElement(lastNameLoc); return el.Displayed ? el : null; }).SendKeys(lastName);
        wait.Until(d => { var el = d.FindElement(emailLoc); return el.Displayed ? el : null; }).SendKeys(email);

        var subjectElement = wait.Until(d => { var el = d.FindElement(subjectLoc); return el.Displayed ? el : null; });
        var subjectSelect = new SelectElement(subjectElement);
        subjectSelect.SelectByValue("webmaster");

        wait.Until(d => { var el = d.FindElement(messageLoc); return el.Displayed ? el : null; }).SendKeys(message);
    }

    public void SubmitForm()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(btnSubmitLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }

    public string GetAlertText()
    {
        return wait.Until(d =>
        {
            var el = d.FindElement(alertLoc);
            return el.Displayed ? el.Text : null;
        });
    }
}
