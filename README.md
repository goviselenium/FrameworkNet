# SeleniumFramework — Enterprise .NET 8 Test Automation

> **Stack:** .NET 8 · NUnit 4 · Selenium WebDriver · Microsoft Edge (Headless) · Extent Reports · Serilog · Azure DevOps CI/CD

---

## Quick Start

```bash
# 1. Clone the repo
git clone https://your-org@dev.azure.com/your-org/your-repo/_git/SeleniumFramework
cd SeleniumFramework

# 2. Restore dependencies
dotnet restore

# 3. Set environment variables (locally)
export TEST_ENV=QA
export Credentials__Username=your-test-user@domain.com
export Credentials__Password=your-password

# 4. Run Smoke tests
dotnet test --filter "Category=Smoke" --settings .runsettings

# 5. Run Regression tests
dotnet test --filter "Category=Regression" --settings .runsettings

# 6. Run both categories
dotnet test --filter "Category=Smoke|Category=Regression" --settings .runsettings
```

---

## Project Structure

```
SeleniumFramework/
├── Config/                  # appsettings per environment + TestSettings model
├── Core/                    # BaseTest — driver lifecycle, screenshots, reporting
├── Drivers/                 # DriverFactory (Edge headless) + ThreadLocal DriverManager
├── Pages/                   # Page Object Model — BasePage, LoginPage, HomePage
├── Tests/
│   ├── Smoke/               # Fast, critical-path tests
│   └── Regression/          # Full regression suite + data-driven tests
├── Utilities/               # WaitHelper, ScreenshotHelper, RetryTestAttribute, Logger
├── Reports/                 # ExtentReportManager (thread-safe HTML5 report)
├── TestData/                # JSON test data files
├── .runsettings             # NUnit parallel execution config (4 workers)
└── azure-pipelines.yml      # Full 3-stage Azure DevOps pipeline
```

---

## Configuration

Environment is driven entirely by the `TEST_ENV` environment variable. The matching
`appsettings.{ENV}.json` overlay is applied on top of the base `appsettings.json`.

| Variable | Values | How to set |
|---|---|---|
| `TEST_ENV` | Dev, QA, UAT, Prod | Pipeline variable / local env |
| `Credentials__Username` | your test user | Azure Key Vault secret |
| `Credentials__Password` | your test password | Azure Key Vault secret |
| `TestSettings__Headless` | true / false | Pipeline variable |

---

## Azure DevOps Setup

1. Import `azure-pipelines.yml` as a new pipeline.
2. Create a **Variable Group** named `selenium-framework-secrets` in Pipelines → Library.
3. Link it to your **Azure Key Vault** and add secrets:
   - `AUT-Username`
   - `AUT-Password`
4. Queue the pipeline and select `testCategory` and `targetEnvironment` at run time.

---

## Parallel Execution

Configured in `.runsettings` — 4 parallel NUnit workers by default.
Each worker gets its own `ThreadLocal<IWebDriver>` instance via `DriverManager`.
Adjust `NumberOfTestWorkers` to match your Azure agent's CPU core count.

---

## Reports & Artifacts

| Artifact | Location | Description |
|---|---|---|
| Extent HTML report | `TestArtifacts/Reports/` | Full HTML5 test report with steps & screenshots |
| Serilog log | `TestArtifacts/Logs/` | Rolling daily log file |
| Failure screenshots | `TestArtifacts/Screenshots/` | Auto-captured on test failure |
| TRX results | Azure DevOps Tests tab | Native ADO test reporting |

---

## Adding a New Page

1. Create `Pages/YourPage.cs` inheriting from `BasePage`.
2. Define locators as `static readonly By` fields.
3. Expose actions as fluent methods returning `this` or the next page.
4. Inject via `new YourPage(Driver, Settings)` in your test class.

## Adding a New Test

1. Create a class in `Tests/Smoke/` or `Tests/Regression/`.
2. Inherit from `BaseTest`.
3. Decorate with `[Category("Smoke")]` or `[Category("Regression")]`.
4. Add `[RetryTest(2)]` to every `[Test]` method.
5. Use `ExtentReportManager.Log(...)` / `.Pass(...)` for report steps.
