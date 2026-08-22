using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class ProductDetailPage : BasePage
{
    private ILocator Title => Page.Locator("h1[data-test='product-title']");
    private ILocator Price => Page.Locator("span[data-test='product-price']");
    private ILocator AddToCartBtn => Page.Locator("button[data-test='add-to-cart']");
    private ILocator Description => Page.Locator("p[data-test='product-description']");

    public ProductDetailPage(IPage page) : base(page) { }

    public async Task<string> GetProductTitleAsync() => await Title.InnerTextAsync();
    public async Task<string> GetProductPriceAsync() => await Price.InnerTextAsync();
    public async Task<string> GetProductDescriptionAsync() => await Description.InnerTextAsync();
    public async Task AddToCartAsync() => await AddToCartBtn.ClickAsync();
}
