using NUnit.Framework;
using SeleniumCSharpTests.Pages;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class ContactFormTest : TestBase
{
    [Test]
    public void TestContactFormSubmit()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/contact");

        var contactPage = new ContactPage(driver);

        contactPage.FillForm("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters in this message.");
        contactPage.SubmitForm();

        Assert.That(contactPage.GetAlertText().Trim(),
            Is.EqualTo("Thanks for your message! We will contact you shortly."));
    }
}
