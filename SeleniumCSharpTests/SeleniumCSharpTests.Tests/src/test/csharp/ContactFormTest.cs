using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class ContactFormTest : TestBase
{
    [Test]
    public void TestContactFormSubmit()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/contact");

        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

        var firstName = wait.Until(d => d.FindElement(By.Id("first_name")));
        var lastName = wait.Until(d => d.FindElement(By.Id("last_name")));
        var email = wait.Until(d => d.FindElement(By.Id("email")));
        var subject = wait.Until(d => d.FindElement(By.Id("subject")));
        var message = wait.Until(d => d.FindElement(By.Id("message")));
        var btnSubmit = wait.Until(d =>
        {
            var el = d.FindElement(By.ClassName("btnSubmit"));
            return el.Enabled && el.Displayed ? el : null;
        });

        firstName.SendKeys("John");
        lastName.SendKeys("Smith");
        email.SendKeys("john.smith@example.com");

        var subjectSelect = new SelectElement(subject);
        subjectSelect.SelectByValue("webmaster");

        message.SendKeys("Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
        btnSubmit.Click();

        var alert = wait.Until(d => d.FindElement(By.ClassName("alert")));
        Assert.That(alert.Text.Trim(), Is.EqualTo("Thanks for your message! We will contact you shortly."));
    }
}
