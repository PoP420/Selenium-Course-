using System.Text.RegularExpressions;
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class LoginTest : TestBase
{
    [Test]
    public async Task TestLoginPageLoads()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateToLoginAsync();

        await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));
        await Expect(Page).ToHaveURLAsync("https://practicesoftwaretesting.com/auth/login");
    }

    [Test]
    public async Task TestLoginWithInvalidCredentials()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateToLoginAsync();

        await loginPage.LoginAsync("invalid@example.com", "wrongpassword");

        var errorMessage = await loginPage.GetErrorMessageAsync();
        Assert.That(errorMessage, Does.Contain("Invalid email or password"));
    }
}
