using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Interfaces;
using Credfeto.Notification.Bot.Twitch.Resources;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Credfeto.Notification.Bot.Twitch.Services;

public sealed class ChannelFollowCount : IChannelFollowCount
{
    private const string HTTP_CLIENT_NAME = nameof(ChannelFollowCount);
    private static readonly TimeSpan ClientTimeout = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan ClientPolicyTimeout = ClientTimeout.Add(TimeSpan.FromSeconds(1));

    private static readonly string UserAgentProduct = DetermineUserAgentProduct();
    private static readonly string UserAgentVersion = DetermineUserAgentVersion();
    private static readonly Uri BaseUri = new("https://api.crunchprank.net/");

    private readonly IHttpClientFactory _httpClientFactory;

    public ChannelFollowCount(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<int> GetCurrentFollowerCountAsync(Streamer streamer, CancellationToken cancellationToken)
    {
        HttpClient client = this.GetClient();

        Uri followCountUri = new("/twitch/followcount/" + streamer, uriKind: UriKind.Relative);
        string result = await client.GetStringAsync(requestUri: followCountUri, cancellationToken: cancellationToken);

        if (int.TryParse(s: result, style: NumberStyles.Integer, provider: CultureInfo.InvariantCulture, out int followers))
        {
            return followers;
        }

        return -1;
    }

    private HttpClient GetClient()
    {
        return this._httpClientFactory.CreateClient(HTTP_CLIENT_NAME);
    }

    public static IServiceCollection RegisterHttpClient(IServiceCollection services)
    {
        return ConfigureCommon(services.AddHttpClient(name: HTTP_CLIENT_NAME, configureClient: InitialiseClientCommon));
    }

    private static string DetermineUserAgentProduct()
    {
        return ThisAssembly.Info.Product;
    }

    private static string DetermineUserAgentVersion()
    {
        const string fileVersion = ThisAssembly.Info.FileVersion;
        int plusIndex = fileVersion.IndexOf(value: '+', comparisonType: StringComparison.Ordinal);

        return plusIndex >= 0
            ? fileVersion.Substring(startIndex: 0, length: plusIndex)
            : fileVersion;
    }

    private static void InitialiseClientCommon(HttpClient httpClient)
    {
        httpClient.BaseAddress = BaseUri;
        httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher;
        httpClient.Timeout = ClientTimeout;
        httpClient.DefaultRequestVersion = new(major: 1, minor: 1);
        httpClient.DefaultRequestHeaders.Accept.Add(new(mediaType: "application/json"));
        httpClient.DefaultRequestHeaders.UserAgent.Add(new(new ProductHeaderValue(name: UserAgentProduct, version: UserAgentVersion)));
    }

    private static IServiceCollection ConfigureCommon(IHttpClientBuilder builder)
    {
        return builder.SetHandlerLifetime(TimeSpan.FromMinutes(5))
                      .ConfigurePrimaryHttpMessageHandler(configureHandler: _ => new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All })
                      .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(ClientPolicyTimeout))
                      .AddSensibleTransientHttpErrorPolicy()
                      .Services;
    }
}