namespace Credfeto.Notification.Bot.Discord.Commands;

internal static class TextExtensions
{
    public static string AsEnabledDisabled(this bool enabled)
    {
        return enabled
            ? "enabled"
            : "disabled";
    }
}