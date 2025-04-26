using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace Credfeto.Notification.Bot.Twitch.Resources;

public static class HttpPolicyExtensions
{
    private static readonly Func<HttpResponseMessage, bool> TransientHttpStatusCodePredicate = response =>
    {
        return response.StatusCode switch
        {
            HttpStatusCode.TooManyRequests => true,
            HttpStatusCode.ServiceUnavailable => true,
            HttpStatusCode.BadGateway => true,
            HttpStatusCode.GatewayTimeout => true,
            HttpStatusCode.RequestTimeout => true,
            _ => false,
        };
    };

    public static PolicyBuilder<HttpResponseMessage> SensiblyHandleTransientHttpError()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrTransientHttpStatusCode()
            .Or<TimeoutRejectedException>()
            .Or<TaskCanceledException>()
            .Or<OperationCanceledException>()
            .Or<TimeoutException>();
    }

    private static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(
        this PolicyBuilder<HttpResponseMessage> policyBuilder
    )
    {
        return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
    }
}
