using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.Timeout;

namespace Credfeto.Notification.Bot.Twitch.Resources;

/// <summary>
///     Contains opinionated convenience methods for configuring policies to handle conditions typically representing transient faults when making <see cref="HttpClient" /> requests.
/// </summary>
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

    /// <summary>
    ///     Builds a <see cref="PolicyBuilder{HttpResponseMessage}" /> to configure a <see cref="Policy{HttpResponseMessage}" /> which will handle <see cref="HttpClient" /> requests that fail with conditions
    ///     indicating a transient failure.
    ///     <para>
    ///         The conditions configured to be handled are:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>Network failures (as <see cref="HttpRequestException" />)</description>
    ///             </item>
    ///             <item>
    ///                 <description>HTTP 5XX status codes (server errors)</description>
    ///             </item>
    ///             <item>
    ///                 <description>HTTP 408 status code (request timeout)</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <returns>The <see cref="PolicyBuilder{HttpResponseMessage}" /> pre-configured to handle <see cref="HttpClient" /> requests that fail with conditions indicating a transient failure. </returns>
    public static PolicyBuilder<HttpResponseMessage> SensiblyHandleTransientHttpError()
    {
        return Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                                          .OrTransientHttpStatusCode()
                                          .Or<TimeoutRejectedException>()
                                          .Or<TaskCanceledException>()
                                          .Or<OperationCanceledException>()
                                          .Or<TimeoutException>();
    }

    /// <summary>
    ///     Configures the <see cref="PolicyBuilder{HttpResponseMessage}" /> to handle <see cref="HttpClient" /> requests that fail with <see cref="HttpStatusCode" />s indicating a transient failure.
    ///     <para>
    ///         The <see cref="HttpStatusCode" />s configured to be handled are:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>HTTP 5XX status codes (server errors)</description>
    ///             </item>
    ///             <item>
    ///                 <description>HTTP 408 status code (request timeout)</description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </summary>
    /// <returns>
    ///     The <see cref="PolicyBuilder{HttpResponseMessage}" /> pre-configured to handle <see cref="HttpClient" /> requests that fail with <see cref="HttpStatusCode" />s indicating a transient
    ///     failure.
    /// </returns>
    private static PolicyBuilder<HttpResponseMessage> OrTransientHttpStatusCode(this PolicyBuilder<HttpResponseMessage> policyBuilder)
    {
        if (policyBuilder == null)
        {
            throw new ArgumentNullException(nameof(policyBuilder));
        }

        return policyBuilder.OrResult(TransientHttpStatusCodePredicate);
    }
}