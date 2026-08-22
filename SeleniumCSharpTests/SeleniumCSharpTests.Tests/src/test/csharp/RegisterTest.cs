using NUnit.Framework;
using SeleniumCSharpTests.Pages;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class RegisterTest : TestBase
{
    [Test]
    public void TestRegister()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/auth/register");

        var registerPage = new RegisterPage(driver);
        registerPage.Register(
            "John", "Doe", "2001-01-01", "12345", "123",
            "Main Street", "Cityville", "Stateville", "1234567890",
            "john.doe@example.com", "Password@123459678"
        );
        registerPage.SubmitForm();
    }
}
