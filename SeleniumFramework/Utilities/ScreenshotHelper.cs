using OpenQA.Selenium;
using Serilog;

namespace SeleniumFramework.Utilities;

public static class ScreenshotHelper
{
    public static string Capture(IWebDriver driver, string testName)
    {
        var dir = Path.Combine(AppContext.BaseDirectory, "TestArtifacts", "Screenshots");
        Directory.CreateDirectory(dir);

        // Sanitise test name so it is safe to use as a filename
        var safe = string.Concat(
            testName.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));

        var path = Path.Combine(dir, $"{safe}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}.png");

        try
        {
            ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(path);
            Log.Debug("Screenshot saved: {Path}", path);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to capture screenshot for '{Test}'", testName);
        }

        return path;
    }
}
