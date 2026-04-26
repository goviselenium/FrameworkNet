using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Serilog;
using SeleniumFramework.Config;
using SeleniumFramework.Drivers;
using SeleniumFramework.Reports;
using SeleniumFramework.Utilities;

namespace SeleniumFramework.Core;

/// <summary>
/// All test fixtures inherit from BaseTest.
/// FixtureLifeCycle.InstancePerTestCase is mandatory for NUnit parallel execution —
/// it ensures each test gets its own class instance and therefore its own driver.
/// </summary>
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Parallelizable(ParallelScope.All)]
public abstract class BaseTest
{
    protected IWebDriver   Driver   => DriverManager.Current;
    protected TestSettings Settings { get; private set; } = null!;

    // ── One-time global setup (runs once per fixture class) ──────────
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        LoggerHelper.Initialize();

        var s = ConfigurationManager.GetTestSettings();
        ExtentReportManager.InitializeReport(
            reportDir:   Path.GetFullPath(s.ReportDir),
            environment: s.Environment,
            browser:     s.Browser);
    }

    // ── Per-test setup ───────────────────────────────────────────────
    [SetUp]
    public virtual void Setup()
    {
        Settings = ConfigurationManager.GetTestSettings();

        var testName = TestContext.CurrentContext.Test.Name;
        var fullName = TestContext.CurrentContext.Test.FullName;

        Log.Information("▶ Starting: {Name} | Env: {Env} | Browser: {Browser} | Headless: {H}",
            testName, Settings.Environment, Settings.Browser, Settings.Headless);

        ExtentReportManager.CreateTest(testName, fullName);

        var driver = DriverFactory.Create(Settings);
        DriverManager.Set(driver);
        Driver.Navigate().GoToUrl(Settings.BaseUrl);

        ExtentReportManager.Log($"Navigated to: {Settings.BaseUrl}");
    }

    // ── Per-test teardown ────────────────────────────────────────────
    [TearDown]
    public virtual void TearDown()
    {
        var result   = TestContext.CurrentContext.Result;
        var status   = result.Outcome.Status;
        var testName = TestContext.CurrentContext.Test.Name;

        Log.Information("■ Finished: {Name} | Status: {Status}", testName, status);

        if (status == TestStatus.Failed)
        {
            var screenshotPath = ScreenshotHelper.Capture(Driver, testName);
            ExtentReportManager.AttachScreenshot(screenshotPath);
            TestContext.AddTestAttachment(screenshotPath, "Failure Screenshot");

            var message = result.Message ?? "Test failed with no message.";
            ExtentReportManager.Fail(message);
            Log.Error("Test failed: {Name} | Reason: {Msg}", testName, message);
        }
        else
        {
            ExtentReportManager.Pass();
        }

        DriverManager.Quit();
    }

    // ── One-time global teardown ─────────────────────────────────────
    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        ExtentReportManager.FlushReport();
        Log.CloseAndFlush();
    }
}
