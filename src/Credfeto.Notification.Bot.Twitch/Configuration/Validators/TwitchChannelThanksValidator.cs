using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchChannelThanksValidator : AbstractValidator<TwitchChannelThanks>
{
    public TwitchChannelThanksValidator()
    {
        this.RuleFor(x => x.Enabled)
            .NotNull()
            .NotEmpty();
    }

}