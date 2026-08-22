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
        loginPage.Login("john.doe@manager.com", "me4-vTs5Cadj-Gn");
    }

    
}
