using OpenQA.Selenium;

namespace SeleniumFramework.Drivers;

/// <summary>
/// Thread-local WebDriver storage — guarantees parallel test isolation.
/// Each NUnit worker thread gets its own independent driver instance.
/// </summary>
public static class DriverManager
{
    private static readonly ThreadLocal<IWebDriver?> _driver = new();

    public static IWebDriver Current =>
        _driver.Value
        ?? throw new InvalidOperationException(
            "WebDriver not initialised for this thread. Ensure Setup() has run.");

    public static void Set(IWebDriver driver)   => _driver.Value = driver;

    public static void Quit()
    {
        _driver.Value?.Quit();
        _driver.Value?.Dispose();
        _driver.Value = null;
    }
}
