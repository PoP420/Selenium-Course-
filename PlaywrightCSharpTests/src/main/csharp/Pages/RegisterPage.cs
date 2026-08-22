using Microsoft.Playwright;

namespace PlaywrightCSharpTests.Pages;

public class RegisterPage : BasePage
{
    private ILocator FirstNameInput => Page.Locator("#first_name");
    private ILocator LastNameInput => Page.Locator("#last_name");
    private ILocator DobInput => Page.Locator("#dob");
    private ILocator CountrySelect => Page.Locator("#country");
    private ILocator PostalCodeInput => Page.Locator("#postal_code");
    private ILocator HouseNumberInput => Page.Locator("#house_number");
    private ILocator StreetInput => Page.Locator("#street");
    private ILocator CityInput => Page.Locator("#city");
    private ILocator StateInput => Page.Locator("#state");
    private ILocator PhoneInput => Page.Locator("#phone");
    private ILocator EmailInput => Page.Locator("#email");
    private ILocator PasswordInput => Page.Locator("#password");
    private ILocator BtnSubmit => Page.Locator(".btnSubmit");
    private ILocator Alert => Page.Locator(".alert");

    public RegisterPage(IPage page) : base(page) { }

    public async Task NavigateToRegisterAsync()
    {
        await Page.GotoAsync("https://practicesoftwaretesting.com/auth/register");
        await FirstNameInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
    }

    public async Task RegisterAsync(
        string firstName, string lastName, string dob, string postalCode,
        string houseNumber, string street, string city, string state,
        string phone, string email, string password)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await DobInput.FillAsync(dob);
        await CountrySelect.SelectOptionAsync("PH");
        await PostalCodeInput.FillAsync(postalCode);
        await HouseNumberInput.FillAsync(houseNumber);
        await StreetInput.FillAsync(street);
        await CityInput.FillAsync(city);
        await StateInput.FillAsync(state);
        await PhoneInput.FillAsync(phone);
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
    }

    public async Task SubmitFormAsync() => await BtnSubmit.ClickAsync();

    public async Task<string> GetAlertTextAsync() => (await Alert.InnerTextAsync()).Trim();
}
