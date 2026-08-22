using NUnit.Framework;
using SeleniumCSharpTests.Pages;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class LoginTest : TestBase
{
    [Test]
    public void TestLogin()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/auth/login");

        var loginPage = new LoginPage(driver);
        loginPage.Login("john.doe@example.com", "Password@123459678");
    }
}
