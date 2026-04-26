using NUnit.Framework;
using SeleniumFramework.Config;
using SeleniumFramework.Core;
using SeleniumFramework.Pages;
using SeleniumFramework.Reports;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Tests.Smoke;

[TestFixture]
[Category("Smoke")]
[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class LoginSmokeTests : BaseTest
{
    private Credentials _creds = null!;

    [SetUp]
    public override void Setup()
    {
        base.Setup();
        _creds = ConfigurationManager.GetCredentials();
    }

    [Test, RetryTest(2)]
    [Description("Valid credentials must navigate to /home and show a welcome banner.")]
    public void Login_ValidCredentials_NavigatesToHomePage()
    {
        ExtentReportManager.Log("Performing login with valid credentials");
        var home = new LoginPage(Driver, Settings).LoginAs(_creds.Username, _creds.Password);

        Assert.Multiple(() =>
        {
            Assert.That(home.IsLoaded(),       Is.True,               "Home page did not load.");
            Assert.That(Driver.Url,            Does.Contain("/home"), "URL mismatch after login.");
            Assert.That(home.GetWelcomeText(), Is.Not.Empty,          "Welcome banner is empty.");
        });

        ExtentReportManager.Pass("Login successful — home page verified.");
    }

    [Test, RetryTest(2)]
    [Description("Invalid password must stay on login page and show an error message.")]
    public void Login_InvalidPassword_ShowsErrorMessage()
    {
        var loginPage = new LoginPage(Driver, Settings);
        loginPage.EnterUsername(_creds.Username)
                 .EnterPassword("wrong_pass_!@#")
                 .ClickLogin();

        Assert.Multiple(() =>
        {
            Assert.That(loginPage.IsErrorDisplayed(), Is.True);
            Assert.That(loginPage.GetErrorMessage(),  Does.Contain("Invalid").IgnoreCase);
        });

        ExtentReportManager.Pass("Error message displayed correctly for invalid password.");
    }

    [Test, RetryTest(2)]
    [Description("Empty credentials must show a validation error — not crash or navigate.")]
    public void Login_EmptyCredentials_ShowsValidationError()
    {
        var loginPage = new LoginPage(Driver, Settings);
        loginPage.EnterUsername(string.Empty)
                 .EnterPassword(string.Empty)
                 .ClickLogin();

        Assert.That(loginPage.IsErrorDisplayed(), Is.True,
            "No validation error shown for empty credentials.");

        ExtentReportManager.Pass("Validation error shown for empty credentials.");
    }
}
