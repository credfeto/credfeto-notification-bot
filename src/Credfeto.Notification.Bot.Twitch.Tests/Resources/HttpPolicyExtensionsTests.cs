using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.Resources;
using FunFair.Test.Common;
using Polly;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Resources;

public sealed class HttpPolicyExtensionsTests : TestBase
{
    [Fact]
    public void SensiblyHandleTransientHttpErrorShouldReturnNonNullPolicyBuilder()
    {
        PolicyBuilder<HttpResponseMessage> builder = HttpPolicyExtensions.SensiblyHandleTransientHttpError();

        Assert.NotNull(builder);
    }

    [Theory]
    [InlineData(HttpStatusCode.TooManyRequests)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    [InlineData(HttpStatusCode.RequestTimeout)]
    public async Task TransientStatusCodesShouldTriggerRetry(HttpStatusCode statusCode)
    {
        IAsyncPolicy<HttpResponseMessage> policy = HttpPolicyExtensions
            .SensiblyHandleTransientHttpError()
            .WaitAndRetryAsync(retryCount: 1, sleepDurationProvider: _ => System.TimeSpan.Zero);

        int callCount = 0;

        using HttpResponseMessage response = await policy.ExecuteAsync(() =>
        {
            callCount++;

            return Task.FromResult(new HttpResponseMessage(statusCode));
        });

        Assert.True(condition: callCount > 1, userMessage: "Transient status code should trigger retry");
    }

    [Fact]
    public async Task NonTransientStatusCodeShouldNotTriggerRetry()
    {
        IAsyncPolicy<HttpResponseMessage> policy = HttpPolicyExtensions
            .SensiblyHandleTransientHttpError()
            .WaitAndRetryAsync(retryCount: 1, sleepDurationProvider: _ => System.TimeSpan.Zero);

        int callCount = 0;

        using HttpResponseMessage response = await policy.ExecuteAsync(() =>
        {
            callCount++;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        });

        Assert.Equal(expected: 1, actual: callCount);
    }
}
