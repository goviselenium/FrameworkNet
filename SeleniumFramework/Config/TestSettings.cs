namespace SeleniumFramework.Config;

public sealed class TestSettings
{
    public string BaseUrl       { get; init; } = string.Empty;
    public string Browser       { get; init; } = "Edge";
    public bool   Headless      { get; init; } = true;
    public int    ExplicitWait  { get; init; } = 15;
    public int    RetryCount    { get; init; } = 2;
    public string ScreenshotDir { get; init; } = "TestArtifacts/Screenshots";
    public string ReportDir     { get; init; } = "TestArtifacts/Reports";
    public string LogDir        { get; init; } = "TestArtifacts/Logs";
    public string Environment   { get; init; } = "QA";
}

public sealed class Credentials
{
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
