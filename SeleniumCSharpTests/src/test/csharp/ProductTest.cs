using NUnit.Framework;
using SeleniumCSharpTests.Pages;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class ProductTest : TestBase
{
    [Test]
    public void TestProductBrowseAndDetail()
    {
        var homePage = new HomePage(driver);
        homePage.Navigate();

        Assert.That(homePage.IsProductContainerDisplayed(), Is.True);
        Assert.That(homePage.GetProductCardCount(), Is.GreaterThan(0));

        homePage.ClickProductCard(0);

        var productDetailPage = new ProductDetailPage(driver);

        Assert.That(productDetailPage.GetProductName(), Is.Not.Empty);
        Assert.That(productDetailPage.GetProductPrice(), Is.Not.Empty);
        Assert.That(productDetailPage.IsAddToCartButtonDisplayed(), Is.True);
    }

    [Test]
    public void TestProductAddToCart()
    {
        var homePage = new HomePage(driver);
        homePage.Navigate();

        homePage.ClickProductCard(0);

        var navBar = new NavBar(driver);
        int cartCountBefore = navBar.GetCartCount();

        var productDetailPage = new ProductDetailPage(driver);
        productDetailPage.SetQuantity(1);
        productDetailPage.ClickAddToCart();

        int cartCountAfter = navBar.GetCartCount();
        Assert.That(cartCountAfter, Is.GreaterThan(cartCountBefore));
    }
}
