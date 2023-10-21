using System.Text.RegularExpressions;

namespace Credfeto.Notification.Bot.Twitch.Services;

internal static partial class TwitchStreamStatusRegex
{
    [GeneratedRegex(pattern: "^No\\schannel\\swith\\sthe\\sname\\s\"(?<streamer>[\\w\\d_]+)\"\\scould\\sbe\\sfound\\.$",
                    RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture,
                    matchTimeoutMilliseconds: 1000)]
    public static partial Regex NoChannelWithName();
}