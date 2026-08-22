using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class NavBar : BasePage
{
    private ILocator NavHome => Page.Locator("a[data-test='nav-home']");
    private ILocator NavLogin => Page.Locator("a[data-test='nav-sign-in']");
    private ILocator NavRegister => Page.Locator("a[data-test='nav-register']");
    private ILocator NavContact => Page.Locator("a[data-test='nav-contact']");
    private ILocator NavCart => Page.Locator("a[data-test='nav-cart']");

    public NavBar(IPage page) : base(page) { }

    public async Task GoToHomeAsync() => await NavHome.ClickAsync();
    public async Task GoToLoginAsync() => await NavLogin.ClickAsync();
    public async Task GoToRegisterAsync() => await NavRegister.ClickAsync();
    public async Task GoToContactAsync() => await NavContact.ClickAsync();
    public async Task GoToCartAsync() => await NavCart.ClickAsync();
}
