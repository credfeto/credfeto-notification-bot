using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchChatCommandValidator : AbstractValidator<TwitchChatCommand>
{
    public TwitchChatCommandValidator()
    {
        this.RuleFor(x => x.Streamer)
            .NotNull()
            .NotEmpty();

        this.RuleFor(x => x.Bot)
            .NotNull()
            .NotEmpty();

        this.RuleFor(x => x.Match)
            .NotNull()
            .NotEmpty();

        this.RuleFor(x => x.Issue)
            .NotNull()
            .NotEmpty();
    }
}