using System.Text.RegularExpressions;
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class NavBarTest : TestBase
{
    [Test]
    public async Task TestNavigationBarLinks()
    {
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        var navBar = new NavBar(Page);
        await navBar.GoToContactAsync();

        await Expect(Page).ToHaveURLAsync(new Regex(".*contact.*"));

        await navBar.GoToLoginAsync();
        await Expect(Page).ToHaveURLAsync(new Regex(".*auth/login.*"));

        await navBar.GoToHomeAsync();
        await Expect(Page).ToHaveURLAsync("https://practicesoftwaretesting.com/");
    }
}
