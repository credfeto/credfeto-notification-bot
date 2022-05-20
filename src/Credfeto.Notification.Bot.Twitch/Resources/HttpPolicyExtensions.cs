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
                                                                                                   switch (response.StatusCode)
                                                                                                   {
                                                                                                       case HttpStatusCode.TooManyRequests:
                                                                                                       case HttpStatusCode.ServiceUnavailable:
                                                                                                       case HttpStatusCode.BadGateway:
                                                                                                       case HttpStatusCode.GatewayTimeout:
                                                                                                       case HttpStatusCode.RequestTimeout:
                                                                                                       {
                                                                                                           return true;
                                                                                                       }
                                                                                                   }

                                                                                                   return false;
                                                                                               };

    public static PolicyBuilder<HttpResponseMessage> SensiblyHandleTransientHttpError()
    {
        return Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                                          .OrTransientHttpStatusCode()
                                          .Or<TimeoutRejectedException>()
                                          .Or<TaskCanceledException>()
                                          .Or<OperationCanceledException>()
                                          .Or<TimeoutException>();
    }

    private static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder<HttpResponseMessage> policyBuilder)
    {
        if (policyBuilder == null)
        {
            throw new ArgumentNullException(nameof(policyBuilder));
        }

        return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
    }
}