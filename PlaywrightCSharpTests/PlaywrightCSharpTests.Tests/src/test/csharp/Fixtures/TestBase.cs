using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using PlaywrightCSharpTests.Utils;

namespace PlaywrightCSharpTests.Tests;

public class TestBase : PageTest
{
    [SetUp]
    public async Task BaseSetUp()
    {
        TestLogger.LogInformation($"Starting test: {TestContext.CurrentContext.Test.Name}");
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
        {
            TestLogger.LogWarning($"Test FAILED: {TestContext.CurrentContext.Test.Name}");

            var testName = TestContext.CurrentContext.Test.Name;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var safeName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));
            var screenshotPath = Path.Combine("test-results", $"{safeName}_{timestamp}.png");

            Directory.CreateDirectory("test-results");
            await Page.ScreenshotAsync(new() { Path = screenshotPath });
            TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");
        }
    }
}
