using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class ContactFormTest : TestBase
{
    [Test]
    public async Task TestContactFormSubmit()
    {
        var contactPage = new ContactPage(Page);
        await contactPage.NavigateToContactAsync();

        await contactPage.FillFormAsync("John", "Smith", "john.smith@example.com",
            "Hello My name is John Smith, Please make sure we have got 50 characters.");
        await contactPage.SubmitFormAsync();

        var alertText = await contactPage.GetAlertTextAsync();
        Assert.That(alertText,
            Is.EqualTo("Thanks for your message! We will contact you shortly."));
    }
}
