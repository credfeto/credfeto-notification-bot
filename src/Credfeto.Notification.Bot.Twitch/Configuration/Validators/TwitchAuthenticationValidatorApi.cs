using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchAuthenticationValidatorApi : AbstractValidator<TwitchAuthenticationApi>
{
    public TwitchAuthenticationValidatorApi()
    {
        this.RuleFor(x => x.ClientId).NotNull().NotEmpty();

        this.RuleFor(x => x.ClientSecret).NotNull().NotEmpty();

#if THIS_IS_NEEDED_IN_FUTURE_APIS
        this.RuleFor(x => x.ClientAccessToken).NotNull().NotEmpty();
#endif
    }
}
