using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumFramework.Utilities;

/// <summary>
/// All waits in the framework must go through WaitHelper.
/// Thread.Sleep is strictly forbidden — use these explicit wait methods.
/// </summary>
public static class WaitHelper
{
    public static WebDriverWait CreateWait(IWebDriver driver, int seconds) =>
        new(driver, TimeSpan.FromSeconds(seconds))
        {
            PollingInterval = TimeSpan.FromMilliseconds(500)
        };

    public static IWebElement WaitForVisible(IWebDriver driver, By locator, int seconds = 15)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el.Displayed ? el : null;
        })!;
    }

    public static IWebElement WaitForClickable(IWebDriver driver, By locator, int seconds = 15)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d =>
        {
            var el = d.FindElement(locator);
            return el.Displayed && el.Enabled ? el : null;
        })!;
    }

    public static bool WaitForUrlContains(IWebDriver driver, string partial, int seconds = 15)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d => d.Url.Contains(partial, StringComparison.OrdinalIgnoreCase));
    }

    public static bool WaitForTextPresent(IWebDriver driver, By locator, string text, int seconds = 15)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d =>
            d.FindElement(locator).Text.Contains(text, StringComparison.OrdinalIgnoreCase));
    }

    public static bool WaitForElementAbsent(IWebDriver driver, By locator, int seconds = 10)
    {
        var wait = CreateWait(driver, seconds);
        return wait.Until(d => d.FindElements(locator).Count == 0);
    }
}
