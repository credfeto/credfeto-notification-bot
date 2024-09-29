using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchAuthenticationValidator : AbstractValidator<TwitchAuthentication>
{
    public TwitchAuthenticationValidator()
    {
        // Chat
        this.RuleFor(x => x.Chat)
            .NotNull()
            .NotEmpty()
            .SetValidator(new TwitchAuthenticationValidatorChat());

        this.RuleFor(x => x.Api)
            .NotNull()
            .NotEmpty()
            .SetValidator(new TwitchAuthenticationValidatorApi());
    }
}