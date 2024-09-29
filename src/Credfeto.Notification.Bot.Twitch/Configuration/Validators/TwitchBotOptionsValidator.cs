using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchBotOptionsValidator : AbstractValidator<TwitchBotOptions>
{
    public TwitchBotOptionsValidator()
    {
        this.RuleFor(x => x.Authentication)
            .NotNull()
            .SetValidator(new TwitchAuthenticationValidator());

        this.RuleFor(x => x.ChatCommands)
            .NotNull();

        this.RuleForEach(x => x.ChatCommands)
            .NotNull()
            .SetValidator(new TwitchChatCommandValidator());
    }
}