using NUnit.Framework;
using SeleniumCSharpTests.Pages;
using System.Collections.Generic;
using System.Linq;

namespace SeleniumCSharpTests.Tests;

[TestFixture]
public class ProductSortingTest : TestBase
{
    private HomeSortingPage homePage;

    [SetUp]
    public void TestSetUp()
    {
        homePage = new HomeSortingPage(driver);
        homePage.Navigate();
    }

    [Test]
    public void TestSortByNameAscending()
    {
        homePage.SortByNameAscending();
        var names = homePage.GetProductNames();

        Assert.That(names.Count, Is.GreaterThan(1));
        var sorted = names.OrderBy(n => n, StringComparer.Ordinal).ToList();
        Assert.That(names, Is.EqualTo(sorted), "Products should be sorted by name A-Z");
    }

    [Test]
    public void TestSortByNameDescending()
    {
        homePage.SortByNameDescending();
        var names = homePage.GetProductNames();

        Assert.That(names.Count, Is.GreaterThan(1));
        var sorted = names.OrderByDescending(n => n, StringComparer.Ordinal).ToList();
        Assert.That(names, Is.EqualTo(sorted), "Products should be sorted by name Z-A");
    }

    [Test]
    public void TestSortByPriceLowToHigh()
    {
        homePage.SortByPriceLowToHigh();
        var prices = homePage.GetProductPricesAsDecimals();

        Assert.That(prices.Count, Is.GreaterThan(1));
        var sorted = prices.OrderBy(p => p).ToList();
        Assert.That(prices, Is.EqualTo(sorted), "Products should be sorted by price low to high");
    }

    [Test]
    public void TestSortByPriceHighToLow()
    {
        homePage.SortByPriceHighToLow();
        var prices = homePage.GetProductPricesAsDecimals();

        Assert.That(prices.Count, Is.GreaterThan(1));
        var sorted = prices.OrderByDescending(p => p).ToList();
        Assert.That(prices, Is.EqualTo(sorted), "Products should be sorted by price high to low");
    }

    [Test]
    public void TestSearch()
    {
        homePage.Search("Pliers");
        var names = homePage.GetProductNames();

        Assert.That(names.Count, Is.GreaterThan(0), "Search should return results");
        Assert.That(names.Any(n => n.Contains("Pliers")), Is.True, "Results should contain 'Pliers'");
    }

    [Test]
    public void TestCategoryFilter()
    {
        homePage.ResetSearch();

        int countBefore = homePage.GetProductCount();
        homePage.SelectCategory("category-01M0M7R9B3EHWWPPN56AN96DJV");
        int countAfter = homePage.GetProductCount();

        Assert.That(countAfter, Is.LessThan(countBefore), "Category filter should reduce product count");
    }
}