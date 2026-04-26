using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;
using Serilog;

namespace SeleniumFramework.Utilities;

/// <summary>
/// Retries a failing NUnit test up to <paramref name="count"/> times before marking it failed.
/// Configured globally at 2 retries. Usage: [RetryTest(2)]
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class RetryTestAttribute(int count) : PropertyAttribute(count), IRepeatTest
{
    private readonly int _count = count;

    public TestCommand Wrap(TestCommand command) => new RetryCommand(command, _count);

    private sealed class RetryCommand(TestCommand inner, int retryCount)
        : DelegatingTestCommand(inner)
    {
        public override TestResult Execute(TestExecutionContext ctx)
        {
            var attempts = 0;
            while (true)
            {
                attempts++;
                ctx.CurrentResult = innerCommand.Execute(ctx);

                if (ctx.CurrentResult.ResultState == ResultState.Success)
                {
                    if (attempts > 1)
                        Log.Information("Test '{Name}' passed on attempt {N}/{Max}.",
                            ctx.CurrentTest.Name, attempts, retryCount);
                    return ctx.CurrentResult;
                }

                if (attempts >= retryCount)
                {
                    Log.Warning("Test '{Name}' failed after {N} attempt(s). Marking as failed.",
                        ctx.CurrentTest.Name, attempts);
                    return ctx.CurrentResult;
                }

                Log.Warning("Test '{Name}' failed — retrying ({Attempt}/{Max})…",
                    ctx.CurrentTest.Name, attempts, retryCount);
            }
        }
    }
}
