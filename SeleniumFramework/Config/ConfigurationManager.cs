using Microsoft.Extensions.Configuration;

namespace SeleniumFramework.Config;

public static class ConfigurationManager
{
    private static IConfiguration? _instance;

    public static IConfiguration Instance => _instance ??= Build();

    private static IConfiguration Build()
    {
        // TEST_ENV drives the environment-specific overlay file.
        // Injected by Azure DevOps pipeline; set locally in your IDE run config.
        var env = System.Environment.GetEnvironmentVariable("TEST_ENV") ?? "QA";

        return new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("Config/appsettings.json",         optional: false)
            .AddJsonFile($"Config/appsettings.{env}.json",  optional: true)
            .AddEnvironmentVariables()   // Azure DevOps variable groups resolve here
            .Build();
    }

    public static TestSettings GetTestSettings() =>
        Instance.GetSection("TestSettings").Get<TestSettings>()
        ?? throw new InvalidOperationException("TestSettings section is missing from config.");

    public static Credentials GetCredentials() =>
        Instance.GetSection("Credentials").Get<Credentials>()
        ?? throw new InvalidOperationException("Credentials section is missing from config.");
}
