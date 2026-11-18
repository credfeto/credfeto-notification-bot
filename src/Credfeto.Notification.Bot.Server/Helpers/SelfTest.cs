using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Credfeto.Notification.Bot.Twitch.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Credfeto.Notification.Bot.Server.Helpers;

public static class SelfTest
{
    private const string SWITCH = "--self-test";

    public const int SUCCESS = 0;
    public const int FAILURE = 1;

    public static bool IsRequested(IReadOnlyList<string> args)
    {
        return args.Contains(value: SWITCH, comparer: StringComparer.Ordinal);
    }

    public static IReadOnlyList<string> WithoutSwitch(IEnumerable<string> args)
    {
        return [.. args.Where(arg => !StringComparer.Ordinal.Equals(x: arg, y: SWITCH))];
    }

    public static int Run(Func<IHost> createApp, TextWriter output)
    {
        try
        {
            LoadNativeDependencies();

            using (IHost app = createApp())
            {
                BindConfiguration(app.Services);
            }

            output.WriteLine("Self-test passed");

            return SUCCESS;
        }
        catch (Exception exception)
        {
            output.WriteLine(
                $"Self-test failed: {exception.GetType().Name}: {exception.Message.ReplaceLineEndings(" ")}"
            );

            return FAILURE;
        }
    }

    private static void LoadNativeDependencies()
    {
        // Culture-aware comparison forces ICU to load and hashing forces OpenSSL to load, so a runtime image
        // missing either fails here rather than on first use in production.
        CompareInfo compareInfo = CultureInfo.GetCultureInfo("en-GB").CompareInfo;
        _ = compareInfo.Compare(string1: "self-test", string2: "SELF-TEST", options: CompareOptions.IgnoreCase);
        _ = SHA256.HashData("self-test"u8);
    }

    private static void BindConfiguration(IServiceProvider services)
    {
        // The image runs the self-test at build time with placeholder settings, so this proves the Twitch section
        // binds without running the options validation that real deployments must pass on start.
        _ = services.GetRequiredService<IConfiguration>().GetSection("Twitch").Get<TwitchBotOptions>();
    }
}
