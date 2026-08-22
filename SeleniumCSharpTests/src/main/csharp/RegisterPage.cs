using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Pages;

public class RegisterPage
{
    private readonly IWebDriver driver;
    private readonly WebDriverWait wait;

    private readonly By firstNameLoc = By.Id("first_name");
    private readonly By lastNameLoc = By.Id("last_name");
    private readonly By birthDateLoc = By.Id("dob");
    private readonly By countryLoc = By.CssSelector("[data-test='country']");
    private readonly By postalCodeLoc = By.Id("postal_code");
    private readonly By houseNumberLoc = By.Id("house_number");
    private readonly By streetLoc = By.Id("street");
    private readonly By cityLoc = By.Id("city");
    private readonly By stateLoc = By.Id("state");
    private readonly By phoneLoc = By.Id("phone");
    private readonly By emailLoc = By.Id("email");
    private readonly By passwordLoc = By.Id("password");
    private readonly By btnSubmitLoc = By.ClassName("btnSubmit");

    public RegisterPage(IWebDriver driver)
    {
        this.driver = driver;
        this.wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
    }

    public void Register(string firstName, string lastName, string birthDate, string postalCode,
        string houseNumber, string street, string city, string state, string phone, string email, string password)
    {
        wait.Until(d => { var el = d.FindElement(firstNameLoc); return el.Displayed ? el : null; }).SendKeys(firstName);
        wait.Until(d => { var el = d.FindElement(lastNameLoc); return el.Displayed ? el : null; }).SendKeys(lastName);
        wait.Until(d => { var el = d.FindElement(birthDateLoc); return el.Displayed ? el : null; }).SendKeys(birthDate);

        var countryElement = wait.Until(d => { var el = d.FindElement(countryLoc); return el.Displayed ? el : null; });
        var countrySelect = new SelectElement(countryElement);
        countrySelect.SelectByValue("PH");

        wait.Until(d => { var el = d.FindElement(postalCodeLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(postalCode);
        wait.Until(d => { var el = d.FindElement(houseNumberLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(houseNumber);
        wait.Until(d => { var el = d.FindElement(streetLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(street);
        wait.Until(d => { var el = d.FindElement(cityLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(city);
        wait.Until(d => { var el = d.FindElement(stateLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(state);
        wait.Until(d => { var el = d.FindElement(phoneLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(phone);
        wait.Until(d => { var el = d.FindElement(emailLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(email);
        wait.Until(d => { var el = d.FindElement(passwordLoc); return el.Displayed && el.Enabled ? el : null; }).SendKeys(password);
    }

    public void SubmitForm()
    {
        wait.Until(d =>
        {
            var el = d.FindElement(btnSubmitLoc);
            return el.Enabled && el.Displayed ? el : null;
        }).Click();
    }
}
