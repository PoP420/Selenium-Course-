using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class LoginPage : BasePage
{
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator PasswordInput => Page.Locator("#password");
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { NameString = "Login" });
    private ILocator AlertMessage => Page.Locator(".alert");
    private ILocator NavHomeLink => Page.Locator("a[data-test='nav-home']");

    public LoginPage(IPage page) : base(page) { }

    public async Task NavigateToLoginAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/login");
        await EmailInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task LoginAndWaitForHomeAsync(string email, string password)
    {
        await LoginAsync(email, password);
        await Page.Locator("a[data-test='nav-home']").WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task<string> GetErrorMessageAsync()
    {
        return (await AlertMessage.InnerTextAsync()).Trim();
    }
}
