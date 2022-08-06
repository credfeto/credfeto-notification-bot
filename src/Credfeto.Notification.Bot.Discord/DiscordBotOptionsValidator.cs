using FluentValidation;

namespace Credfeto.Notification.Bot.Discord;

public sealed class DiscordBotOptionsValidator : AbstractValidator<DiscordBotOptions>
{
    public DiscordBotOptionsValidator()
    {
        this.RuleFor(x => x.Token).NotEmpty();
    }
}