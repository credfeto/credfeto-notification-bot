using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchAuthenticationValidator : AbstractValidator<TwitchAuthentication>
{
    public TwitchAuthenticationValidator()
    {
        // Chat
        this.RuleFor(x => x.UserName)
            .NotNull()
            .NotEmpty();

        this.RuleFor(x => x.OAuthToken)
            .NotNull()
            .NotEmpty();

        // Api
        this.RuleFor(x => x.ClientId)
            .NotNull()
            .NotEmpty();

        this.RuleFor(x => x.ClientSecret)
            .NotNull()
            .NotEmpty();

#if THIS_IS_NEEDED_IN_FUTURE_APIS
        this.RuleFor(x=>x.ClientAccessToken)
            .NotNull()
            .NotEmpty();
#endif
    }
}