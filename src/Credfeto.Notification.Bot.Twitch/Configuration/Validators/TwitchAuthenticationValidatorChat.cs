using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchAuthenticationValidatorChat : AbstractValidator<TwitchAuthenticationChat>
{
    public TwitchAuthenticationValidatorChat()
    {
        this.RuleFor(x => x.UserName).NotNull().NotEmpty();

        this.RuleFor(x => x.OAuthToken).NotNull().NotEmpty();
    }
}
