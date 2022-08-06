using FluentValidation;

namespace Credfeto.Notification.Bot.Twitch.Configuration.Validators;

public sealed class TwitchMilestonesValidator : AbstractValidator<TwitchMilestones>
{
    public TwitchMilestonesValidator()
    {
        this.RuleFor(x => x.Followers)
            .NotNull();

        this.RuleForEach(x => x.Subscribers)
            .NotNull();
    }
}