using OpenQA.Selenium;
using SeleniumFramework.Config;
using SeleniumFramework.Reports;

namespace SeleniumFramework.Pages;

public class LoginPage(IWebDriver driver, TestSettings settings) : BasePage(driver, settings)
{
    // ── Locators — update these to match your application ───────────
    private static readonly By UsernameField = By.Id("username");
    private static readonly By PasswordField = By.Id("password");
    private static readonly By SubmitButton  = By.CssSelector("button[type='submit']");
    private static readonly By ErrorBanner   = By.CssSelector("[data-testid='error-message']");

    // ── Actions ──────────────────────────────────────────────────────
    public LoginPage EnterUsername(string username)
    {
        ExtentReportManager.Log($"Entering username: {username}");
        Type(UsernameField, username);
        return this;
    }

    public LoginPage EnterPassword(string password)
    {
        ExtentReportManager.Log("Entering password");
        Type(PasswordField, password);
        return this;
    }

    public HomePage ClickLogin()
    {
        ExtentReportManager.Log("Clicking login button");
        Click(SubmitButton);
        return new HomePage(Driver, Settings);
    }

    public string GetErrorMessage()  => GetText(ErrorBanner);
    public bool   IsErrorDisplayed() => IsVisible(ErrorBanner);

    /// <summary>Convenience: perform a complete login in one fluent call.</summary>
    public HomePage LoginAs(string username, string password) =>
        EnterUsername(username).EnterPassword(password).ClickLogin();
}
