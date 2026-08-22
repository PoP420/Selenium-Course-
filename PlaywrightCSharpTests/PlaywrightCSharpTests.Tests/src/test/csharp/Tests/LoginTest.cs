using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class LoginTest : TestBase
{
    [Test]
    public async Task TestLoginWithValidCredentials()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateToLoginAsync();

        await loginPage.LoginAsync("john.doe@example.com", "Password@123459678");

        await Expect(Page).ToHaveURLAsync("https://practicesoftwaretesting.com/authorize");
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
