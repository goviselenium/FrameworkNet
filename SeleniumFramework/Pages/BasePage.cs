using OpenQA.Selenium;
using SeleniumFramework.Config;
using SeleniumFramework.Reports;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Pages;

public abstract class BasePage(IWebDriver driver, TestSettings settings)
{
    protected readonly IWebDriver   Driver   = driver;
    protected readonly TestSettings Settings = settings;

    protected IWebElement WaitForVisible(By locator)
        => WaitHelper.WaitForVisible(Driver, locator, Settings.ExplicitWait);

    protected IWebElement WaitForClickable(By locator)
        => WaitHelper.WaitForClickable(Driver, locator, Settings.ExplicitWait);

    protected void Click(By locator)
    {
        ExtentReportManager.Log($"Click: {locator}");
        WaitForClickable(locator).Click();
    }

    protected void Type(By locator, string text)
    {
        ExtentReportManager.Log($"Type into: {locator}");
        var el = WaitForVisible(locator);
        el.Clear();
        el.SendKeys(text);
    }

    protected string GetText(By locator)
        => WaitForVisible(locator).Text;

    protected bool IsVisible(By locator)
    {
        try   { return WaitForVisible(locator).Displayed; }
        catch { return false; }
    }
}
