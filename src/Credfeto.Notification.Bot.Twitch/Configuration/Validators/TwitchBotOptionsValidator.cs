using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchBotOptionsValidator : AbstractValidator<TwitchBotOptions>
{
    public TwitchBotOptionsValidator()
    {
        this.RuleFor(x => x.Authentication)
            .NotNull()
            .SetValidator(new TwitchAuthenticationValidator());

        this.RuleFor(x => x.Channels)
            .NotNull();

        this.RuleForEach(x => x.Channels)
            .NotNull()
            .SetValidator(new TwitchModChannelValidator());

        this.RuleFor(x => x.IgnoredUsers)
            .NotNull();

        this.RuleFor(x => x.Milestones)
            .NotNull()
            .SetValidator(new TwitchMilestonesValidator());

        this.RuleFor(x => x.ChatCommands)
            .NotNull();

        this.RuleForEach(x => x.ChatCommands)
            .NotNull()
            .SetValidator(new TwitchMarblesValidator());
    }
}