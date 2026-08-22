using System.Text.RegularExpressions;
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class HomePageTest : TestBase
{
    [Test]
    public async Task TestHomePageLoads()
    {
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));
        Assert.That(await homePage.IsProductContainerDisplayedAsync(), Is.True);
    }

    [Test]
    public async Task TestProductCardCount()
    {
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        var count = await homePage.GetProductCardCountAsync();
        Assert.That(count, Is.GreaterThan(0));
    }

    [Test]
    public async Task TestClickProductCard()
    {
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        await homePage.ClickProductCardAsync(0);
        await Expect(Page).ToHaveURLAsync(new Regex(".*/product/.*"));
    }
}
