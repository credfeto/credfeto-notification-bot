using Microsoft.Extensions.Options;

namespace Credfeto.Notification.Bot.Twitch.Configuration;

[OptionsValidator]
public sealed partial class TwitchBotOptionsValidator : IValidateOptions<TwitchBotOptions>;
