using NUnit.Framework;
using SeleniumFramework.Config;
using SeleniumFramework.Core;
using SeleniumFramework.Pages;
using SeleniumFramework.Reports;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Tests.Regression;

[TestFixture]
[Category("Regression")]
[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class LoginRegressionTests : BaseTest
{
    private Credentials _creds = null!;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _creds = ConfigurationManager.GetCredentials();
    }

    [Test, RetryTest(2)]
    [Description("Session must persist after a hard browser page refresh.")]
    public void Login_SessionPersistsAfterRefresh()
    {
        var home = new LoginPage(Driver, Settings).LoginAs(_creds.Username, _creds.Password);
        Assert.That(home.IsLoaded(), Is.True, "Initial login failed.");

        Driver.Navigate().Refresh();

        Assert.That(home.IsLoaded(), Is.True, "Session was lost after page refresh.");
        ExtentReportManager.Pass("Session correctly persisted after hard refresh.");
    }

    [Test, RetryTest(2)]
    [Description("Clicking logout must redirect the user back to the login page.")]
    public void Login_Logout_RedirectsToLoginPage()
    {
        var home = new LoginPage(Driver, Settings).LoginAs(_creds.Username, _creds.Password);
        Assert.That(home.IsLoaded(), Is.True, "Login failed — cannot test logout.");

        home.Logout();

        Assert.That(Driver.Url, Does.Contain("/login").IgnoreCase,
            "Logout did not redirect to the login page.");

        ExtentReportManager.Pass("Logout successfully redirected to login page.");
    }

    [Test, RetryTest(2)]
    [Description("Browser back button after login must not expose a protected page.")]
    public void Login_BrowserBackAfterLogout_DoesNotExposeProtectedPage()
    {
        var home = new LoginPage(Driver, Settings).LoginAs(_creds.Username, _creds.Password);
        Assert.That(home.IsLoaded(), Is.True, "Login failed.");

        home.Logout();
        Driver.Navigate().Back();

        // App should redirect back to login — not show a protected page
        Assert.That(Driver.Url, Does.Contain("/login").IgnoreCase,
            "Protected page exposed after logout + back navigation.");

        ExtentReportManager.Pass("Back navigation after logout correctly redirected to login.");
    }

    // ── Data-Driven Tests ──────────────────────────────────────────────────────
    [TestCase("admin@qa.com",    "validPass1!", true,  TestName = "Regression_AdminLogin_Succeeds")]
    [TestCase("readonly@qa.com", "readPass99!", true,  TestName = "Regression_ReadOnlyLogin_Succeeds")]
    [TestCase("invalid@qa.com",  "wrongPass",   false, TestName = "Regression_BadCredentials_Fail")]
    [TestCase("",                "anyPassword", false, TestName = "Regression_EmptyUsername_Fails")]
    [TestCase("user@qa.com",     "",            false, TestName = "Regression_EmptyPassword_Fails")]
    [RetryTest(2)]
    public void Login_DataDriven_Scenarios(string username, string password, bool expectSuccess)
    {
        ExtentReportManager.Log($"Data-driven: user='{username}' | expectSuccess={expectSuccess}");
        var loginPage = new LoginPage(Driver, Settings);

        if (expectSuccess)
        {
            var home = loginPage.LoginAs(username, password);
            Assert.That(home.IsLoaded(), Is.True,
                $"Expected successful login for '{username}' but home page did not load.");
            ExtentReportManager.Pass($"Login succeeded as expected for '{username}'.");
        }
        else
        {
            loginPage.EnterUsername(username).EnterPassword(password).ClickLogin();
            Assert.That(loginPage.IsErrorDisplayed(), Is.True,
                $"Expected an error for '{username}' but none was shown.");
            ExtentReportManager.Pass($"Login correctly rejected for '{username}'.");
        }
    }
}
