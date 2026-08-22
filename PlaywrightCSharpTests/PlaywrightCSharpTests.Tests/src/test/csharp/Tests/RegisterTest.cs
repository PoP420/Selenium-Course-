using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class RegisterTest : TestBase
{
    [Test]
    public async Task TestRegisterPageLoads()
    {
        var registerPage = new RegisterPage(Page);
        await registerPage.NavigateToRegisterAsync();

        await Expect(Page).ToHaveTitleAsync("Practice Software Testing - Register");
    }
}
