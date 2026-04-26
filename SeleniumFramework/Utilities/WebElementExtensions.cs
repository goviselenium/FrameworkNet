using OpenQA.Selenium;

namespace SeleniumFramework.Utilities;

public static class WebElementExtensions
{
    /// <summary>
    /// Scrolls the element into the viewport centre then clicks.
    /// Prevents ElementNotInteractableException in headless Edge.
    /// </summary>
    public static void ScrollAndClick(this IWebElement element, IWebDriver driver)
    {
        ((IJavaScriptExecutor)driver)
            .ExecuteScript("arguments[0].scrollIntoView({block:'center'});", element);
        element.Click();
    }

    /// <summary>Clears the field and types text.</summary>
    public static void ClearAndType(this IWebElement element, string text)
    {
        element.Clear();
        element.SendKeys(text);
    }

    /// <summary>Returns true if at least one element matching the locator exists in the DOM.</summary>
    public static bool IsPresent(this IWebDriver driver, By locator)
    {
        try   { return driver.FindElements(locator).Count > 0; }
        catch { return false; }
    }

    /// <summary>Returns the trimmed inner text of the element.</summary>
    public static string TrimmedText(this IWebElement element) =>
        element.Text.Trim();
}
