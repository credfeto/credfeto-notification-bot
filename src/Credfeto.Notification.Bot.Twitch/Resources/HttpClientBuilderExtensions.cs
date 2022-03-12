using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Retry;

namespace Credfeto.Notification.Bot.Twitch.Resources;

/// <summary>
///     PollyHttpClientBuilderExtensions
/// </summary>
[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "TODO: Review")]
public static class HttpClientBuilderExtensions
{
    private static AsyncRetryPolicy<HttpResponseMessage> ConfigureDefaultHttpErrorPolicy(PolicyBuilder<HttpResponseMessage> policyBuilder, int maxRetries)
    {
        return policyBuilder.WaitAndRetryAsync(retryCount: maxRetries,
                                               sleepDurationProvider: (retryCount, response, _) =>
                                                                      {
                                                                          if (response.Result != null)
                                                                          {
                                                                              if (response.Result.Headers.TryGetValues(name: "Retry-After", out IEnumerable<string>? result) &&
                                                                                  int.TryParse(result.First(), out int seconds))
                                                                              {
                                                                                  return TimeSpan.FromSeconds(seconds);
                                                                              }
                                                                          }

                                                                          return CalculateRetryDelay(retryCount);
                                                                      },
                                               onRetryAsync: async (_, _, _, _) => { await Task.CompletedTask; });
    }

    private static TimeSpan CalculateRetryDelay(int attempts)
    {
        // do a fast first retry, then exponential backoff
        return attempts <= 1
            ? TimeSpan.Zero
            : TimeSpan.FromSeconds(Math.Pow(x: 2, y: attempts));
    }

    /// <summary>
    ///     Adds a <see cref="PolicyHttpMessageHandler" /> which will surround request execution.
    ///     The policy builder will be preconfigured to trigger application of the policy for requests
    ///     that fail with conditions that indicate a transient failure.
    /// </summary>
    /// <param name="builder">The <see cref="IHttpClientBuilder" />.</param>
    /// <param name="maxRetries">The maximum number of retries.</param>
    /// <returns>An <see cref="IHttpClientBuilder" /> that can be used to configure the client.</returns>
    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
    public static IHttpClientBuilder AddSensibleTransientHttpErrorPolicy(this IHttpClientBuilder builder, int maxRetries = 3)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        PolicyBuilder<HttpResponseMessage> policyBuilder = HttpPolicyExtensions.SensiblyHandleTransientHttpError();

        // Important - cache policy instances so that they are singletons per handler.
        IAsyncPolicy<HttpResponseMessage> policy = ConfigureDefaultHttpErrorPolicy(policyBuilder: policyBuilder, maxRetries: maxRetries);

        builder.AddHttpMessageHandler(() => new PolicyHttpMessageHandler(policy));

        return builder;
    }
}