using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using SeleniumFramework.Config;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace SeleniumFramework.Drivers;

public static class DriverFactory
{
    /// <summary>
    /// Creates a fully configured Edge WebDriver.
    /// WebDriverManager auto-downloads the correct msedgedriver version —
    /// no manual driver management ever needed.
    /// </summary>
    public static IWebDriver Create(TestSettings settings)
    {
        // Auto-match msedgedriver to installed Edge version on the agent
        new DriverManager().SetUpDriver(
            new EdgeConfig(),
            VersionResolveStrategy.MatchingBrowser);

        var options = BuildEdgeOptions(settings.Headless);
        var driver  = new EdgeDriver(options);

        driver.Manage().Timeouts().PageLoad               = TimeSpan.FromSeconds(30);
        driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
        // Always use explicit waits — implicit wait intentionally zero
        driver.Manage().Timeouts().ImplicitWait            = TimeSpan.Zero;

        return driver;
    }

    private static EdgeOptions BuildEdgeOptions(bool headless)
    {
        var options = new EdgeOptions();

        if (headless)
        {
            options.AddArgument("--headless=new");        // Modern headless — no deprecated flag
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-gpu");
        }

        // CI/CD hardening
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-popup-blocking");
        options.AddArgument("--ignore-certificate-errors");
        options.AddArgument("--disable-infobars");

        // Suppress the "controlled by automated software" banner
        options.AddExcludedArgument("enable-automation");
        options.AddUserProfilePreference("credentials_enable_service", false);

        return options;
    }
}
