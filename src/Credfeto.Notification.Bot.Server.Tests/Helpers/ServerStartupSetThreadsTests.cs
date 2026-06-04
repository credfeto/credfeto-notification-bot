using System;
using System.Threading;
using Credfeto.Notification.Bot.Server.Helpers;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Server.Tests.Helpers;

public sealed class ServerStartupSetThreadsTests : TestBase
{
    private const int PROCESSOR_COUNT_MULTIPLIER = 10;
    private const int THREAD_THRESHOLD_OFFSET = 5;

    public static TheoryData<int, int, int> SetThreadsScenarios =>
        new()
        {
            // initialWorker, initialIoc, threshold
            // Scenario: worker below, IOC at or above threshold
            {
                Environment.ProcessorCount,
                Environment.ProcessorCount * PROCESSOR_COUNT_MULTIPLIER,
                Environment.ProcessorCount + THREAD_THRESHOLD_OFFSET
            },
            // Scenario: IOC below, worker at or above threshold
            {
                Environment.ProcessorCount * PROCESSOR_COUNT_MULTIPLIER,
                Environment.ProcessorCount,
                Environment.ProcessorCount + THREAD_THRESHOLD_OFFSET
            },
            // Scenario: neither below threshold
            {
                Environment.ProcessorCount * PROCESSOR_COUNT_MULTIPLIER,
                Environment.ProcessorCount * PROCESSOR_COUNT_MULTIPLIER,
                Environment.ProcessorCount
            },
        };

    [Theory]
    [MemberData(nameof(SetThreadsScenarios))]
    public void SetThreadsAdjustsPoolCorrectly(int initialWorker, int initialIoc, int threshold)
    {
        ThreadPool.GetMinThreads(out int savedWorker, out int savedIoc);

        try
        {
            ThreadPool.SetMinThreads(workerThreads: initialWorker, completionPortThreads: initialIoc);

            ServerStartup.SetThreads(threshold);

            ThreadPool.GetMinThreads(out int newWorker, out int newIoc);

            int expectedWorker = initialWorker < threshold ? threshold : initialWorker;
            int expectedIoc = initialIoc < threshold ? threshold : initialIoc;

            Assert.Equal(expectedWorker, newWorker);
            Assert.Equal(expectedIoc, newIoc);
        }
        finally
        {
            ThreadPool.SetMinThreads(workerThreads: savedWorker, completionPortThreads: savedIoc);
        }
    }

    [Fact]
    public void SetThreadsBothBelowThreshold()
    {
        ThreadPool.GetMinThreads(out int savedWorker, out int savedIoc);

        try
        {
            int threshold = Math.Max(savedWorker, savedIoc) + 100;
            ServerStartup.SetThreads(threshold);

            ThreadPool.GetMinThreads(out int newWorker, out int newIoc);

            Assert.Equal(threshold, newWorker);
            Assert.Equal(threshold, newIoc);
        }
        finally
        {
            ThreadPool.SetMinThreads(workerThreads: savedWorker, completionPortThreads: savedIoc);
        }
    }
}
