using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchMarblesValidator : AbstractValidator<TwitchMarbles>
{
    public TwitchMarblesValidator()
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
    }
}