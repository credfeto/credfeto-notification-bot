using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchChannelRaidsValidator : AbstractValidator<TwitchChannelRaids>
{
    public TwitchChannelRaidsValidator()
    {
        this.RuleFor(x => x.Enabled)
            .NotNull()
            .NotEmpty();
    }
}