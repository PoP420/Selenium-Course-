using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class ContactPage : BasePage
{
    private ILocator FirstNameInput => Page.Locator("#first_name");
    private ILocator LastNameInput => Page.Locator("#last_name");
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator SubjectSelect => Page.Locator("#subject");
    private ILocator MessageInput => Page.Locator("#message");
    private ILocator BtnSubmit => Page.Locator(".btnSubmit");
    private ILocator Alert => Page.Locator(".alert");

    public ContactPage(IPage page) : base(page) { }

    public async Task NavigateToContactAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/contact");
    }

    public async Task FillFormAsync(string firstName, string lastName, string email, string message)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await EmailInput.FillAsync(email);
        await SubjectSelect.SelectOptionAsync("webmaster");
        await MessageInput.FillAsync(message);
    }

    public async Task SubmitFormAsync()
    {
        await BtnSubmit.ClickAsync();
    }

    public async Task<string> GetAlertTextAsync()
    {
        return (await Alert.InnerTextAsync()).Trim();
    }
}
