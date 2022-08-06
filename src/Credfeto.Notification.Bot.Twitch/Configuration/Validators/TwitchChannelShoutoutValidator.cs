using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchChannelShoutoutValidator : AbstractValidator<TwitchChannelShoutout>
{
    public TwitchChannelShoutoutValidator()
    {
        // this.RuleFor(x => x.FriendChannels)
        //     .NotNull();

        this.RuleForEach(x => x.FriendChannels)
            .SetValidator(new TwitchFriendChannelValidator());
    }
}