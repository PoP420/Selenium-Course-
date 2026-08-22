using NUnit.Framework;
using SeleniumCSharpTests.Pages;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class FullSequenceLoginAddToCartTest : TestBase
{
    private const string Email = "john.doe@example.com";
    private const string Password = "Password@123459678";

    [Test]
    public void TestFullSequenceLoginAddToCart()
    {
        driver.Navigate().GoToUrl("https://practicesoftwaretesting.com/auth/login");

        var loginPage = new LoginPage(driver);
        loginPage.LoginAndWaitForHome(Email, Password);

        var navBar = new NavBar(driver);
        navBar.ClickHome();

        var homePage = new HomePage(driver);
        Assert.That(homePage.IsProductContainerDisplayed(), Is.True);

        int cardCount = homePage.GetProductCardCount();
        Assert.That(cardCount, Is.GreaterThan(0));

        int cartCountBefore = navBar.GetCartCount();

        homePage.ClickProductCard(0);

        var productDetailPage = new ProductDetailPage(driver);
        string productName = productDetailPage.GetProductName();
        string productPrice = productDetailPage.GetProductPrice();

        Assert.That(productName, Is.Not.Empty);
        Assert.That(productPrice, Is.Not.Empty);
        Assert.That(productDetailPage.IsAddToCartButtonDisplayed(), Is.True);

        productDetailPage.ClickAddToCart();

        int cartCountAfter = navBar.GetCartCount();
        Assert.That(cartCountAfter, Is.GreaterThan(cartCountBefore));
    }
}
