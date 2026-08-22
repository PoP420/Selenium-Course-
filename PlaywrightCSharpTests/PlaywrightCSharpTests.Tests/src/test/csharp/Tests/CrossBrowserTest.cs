using System.Text.RegularExpressions;
using PlaywrightCSharpTests.Pages;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public class CrossBrowserTest : TestBase
{
    [Test]
    [TestCase("chromium")]
    [TestCase("firefox")]
    [TestCase("webkit")]
    public async Task TestLoginPageLoads(string browserName)
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/login");
        await Expect(Page).ToHaveTitleAsync(new Regex("Practice Software Testing"));
    }
}
