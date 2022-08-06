using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchFriendChannelValidator : AbstractValidator<TwitchFriendChannel>
{
    public TwitchFriendChannelValidator()
    {
        this.RuleFor(x => x.Channel)
            .NotEmpty();
        // this.RuleFor(x => x.Message)
        //     .Null()
        //     .NotEmpty();
    }
}