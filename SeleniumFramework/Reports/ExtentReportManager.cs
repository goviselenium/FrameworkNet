using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Config;
using Serilog;

namespace SeleniumFramework.Reports;

/// <summary>
/// Thread-safe singleton Extent Reports manager.
/// Produces a single consolidated HTML5 report across all parallel NUnit workers.
/// </summary>
public static class ExtentReportManager
{
    private static ExtentReports?                    _extent;
    private static ExtentSparkReporter?              _spark;
    private static readonly object                   _lock  = new();
    private static readonly ThreadLocal<ExtentTest?> _test  = new();

    public static ExtentTest CurrentTest =>
        _test.Value ?? throw new InvalidOperationException("ExtentTest not initialised for this thread.");

    /// <summary>Call once in [OneTimeSetUp] of BaseTest.</summary>
    public static void InitializeReport(string reportDir, string environment, string browser)
    {
        lock (_lock)
        {
            if (_extent is not null) return;

            Directory.CreateDirectory(reportDir);
            var reportPath = Path.Combine(reportDir, $"ExtentReport_{DateTime.UtcNow:yyyyMMdd_HHmmss}.html");

            _spark = new ExtentSparkReporter(reportPath);
            _spark.Config.Theme         = Theme.Dark;
            _spark.Config.DocumentTitle = "Selenium Automation Report";
            _spark.Config.ReportName    = $"Edge Headless · {environment} · {DateTime.UtcNow:dd-MMM-yyyy}";
            _spark.Config.Encoding      = "utf-8";

            _extent = new ExtentReports();
            _extent.AttachReporter(_spark);
            _extent.AddSystemInfo("OS",          System.Runtime.InteropServices.RuntimeInformation.OSDescription);
            _extent.AddSystemInfo("Browser",     browser);
            _extent.AddSystemInfo("Environment", environment);
            _extent.AddSystemInfo(".NET",        "8.0");

            Log.Information("Extent report initialised at: {Path}", reportPath);
        }
    }

    /// <summary>Call at the start of each test in SetUp.</summary>
    public static ExtentTest CreateTest(string testName, string description = "")
    {
        lock (_lock)
        {
            var test = _extent!.CreateTest(testName, description);
            _test.Value = test;
            return test;
        }
    }

    /// <summary>Attach a failure screenshot to the current test node.</summary>
    public static void AttachScreenshot(string screenshotPath)
    {
        _test.Value?.Fail("Test failed — screenshot attached:")
                    .AddScreenCaptureFromPath(screenshotPath);
    }

    public static void Pass(string message = "Test passed.")
        => _test.Value?.Pass(message);

    public static void Fail(string message)
        => _test.Value?.Fail(message);

    public static void Log(string message)
        => _test.Value?.Info(message);

    /// <summary>Call once in [OneTimeTearDown] to flush and write the HTML file.</summary>
    public static void FlushReport()
    {
        lock (_lock)
        {
            _extent?.Flush();
            Log.Information("Extent report flushed.");
        }
    }
}
