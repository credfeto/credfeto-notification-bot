using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchChannelWelcomeValidator : AbstractValidator<TwitchChannelWelcome>
{

    public TwitchChannelWelcomeValidator()
    {
        this.RuleFor(x => x.Enabled)
            .NotNull()
            .NotEmpty();
    }
}