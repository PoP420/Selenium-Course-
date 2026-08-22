using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected ILocator Find(string selector) => Page.Locator(selector);

    protected ILocator FindByTestId(string testId) => Page.GetByTestId(testId);
}
