using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchModChannelValidator : AbstractValidator<TwitchModChannel>
{
    public TwitchModChannelValidator()
    {
        this.RuleFor(x => x.ChannelName)
            .NotNull()
            .NotEmpty();

        this.RuleFor(x => x.ShoutOuts)
            .NotNull()
            .SetValidator(new TwitchChannelShoutoutValidator());

        this.RuleFor(x => x.Raids)
            .NotNull()
            .SetValidator(new TwitchChannelRaidsValidator());

        this.RuleFor(x => x.Thanks)
            .NotNull()
            .SetValidator(new TwitchChannelThanksValidator());

        this.RuleFor(x => x.MileStones)
            .NotNull()
            .SetValidator(new TwitchChannelMileStoneValidator());

        this.RuleFor(x => x.Welcome)
            .NotNull()
            .SetValidator(new TwitchChannelWelcomeValidator());
    }
}