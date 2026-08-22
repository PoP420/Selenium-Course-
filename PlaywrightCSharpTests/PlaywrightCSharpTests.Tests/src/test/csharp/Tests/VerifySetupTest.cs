using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightCSharpTests.Tests;

[TestFixture]
public class VerifySetupTest : TestBase
{
    [Test]
    public async Task VerifyChromiumLaunches()
    {
        await Page.GotoAsync("https://example.com");
        await Expect(Page).ToHaveTitleAsync("Example Domain");
    }

    [Test]
    public async Task VerifyExamplePageContent()
    {
        await Page.GotoAsync("https://example.com");
        await Expect(Page.GetByRole(AriaRole.Heading, new() { NameString = "Example Domain" }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("p", new() { HasText = "This domain is for use in" })).ToContainTextAsync("This domain is for use in documentation examples");
    }
}
