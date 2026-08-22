using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class HomePage : BasePage
{
    private ILocator ProductCard => Page.Locator("a.card[data-test^='product-']");

    public HomePage(IPage page) : base(page) { }

    public async Task NavigateAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/");
    }

    public async Task<bool> IsProductContainerDisplayedAsync()
    {
        return await ProductCard.First.IsVisibleAsync();
    }

    public async Task<int> GetProductCardCountAsync()
    {
        return await ProductCard.CountAsync();
    }

    public async Task ClickProductCardAsync(int index = 0)
    {
        await ProductCard.Nth(index).ClickAsync();
    }

    public async Task ClickProductAsync(string productId)
    {
        await Find($"a.card[data-test='{productId}']").ClickAsync();
    }
}
