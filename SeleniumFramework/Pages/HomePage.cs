using OpenQA.Selenium;
using SeleniumFramework.Config;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Pages;

public class HomePage(IWebDriver driver, TestSettings settings) : BasePage(driver, settings)
{
    // ── Locators — update these to match your application ───────────
    private static readonly By WelcomeBanner = By.CssSelector("[data-testid='welcome-banner']");
    private static readonly By UserAvatar    = By.Id("user-avatar");
    private static readonly By LogoutButton  = By.CssSelector("[data-testid='logout']");

    // ── Queries ──────────────────────────────────────────────────────
    public bool IsLoaded() =>
        WaitHelper.WaitForUrlContains(Driver, "/home", Settings.ExplicitWait)
        && IsVisible(WelcomeBanner);

    public string GetWelcomeText()  => GetText(WelcomeBanner);

    // ── Actions ──────────────────────────────────────────────────────
    public void ClickUserAvatar()   => Click(UserAvatar);
    public void Logout()            => Click(LogoutButton);
}
