using Credfeto.Notification.Bot.Shared;
using Credfeto.Notification.Bot.Twitch.DataTypes;
using Credfeto.Notification.Bot.Twitch.Services;
using FunFair.Test.Common;
using NSubstitute;
using Xunit;

namespace Credfeto.Notification.Bot.Twitch.Tests.Services;

public sealed class TwitchChatMessageGeneratorTests : TestBase
{
    private const int BITS_GIVEN = 1000;
    private static readonly Viewer Viewer = Viewer.FromString("viewer");
    private readonly IRandomNumberGenerator _randomNumberGenerator;
    private readonly ITwitchChatMessageGenerator _twitchChatMessageGenerator;

    public TwitchChatMessageGeneratorTests()
    {
        this._randomNumberGenerator = GetSubstitute<IRandomNumberGenerator>();

        this._twitchChatMessageGenerator = new TwitchChatMessageGenerator(this._randomNumberGenerator);
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for the bits!")]
    [InlineData(1, "Thanks @viewer for the bits! You're awesome!")]
    [InlineData(2, "Thanks @viewer for bits!")]
    [InlineData(3, "Thanks @viewer for giving 1000 bits!")]
    [InlineData(4, "@viewer, Thanks for the bits!")]
    [InlineData(5, "@viewer, Thanks for bits!")]
    [InlineData(6, "@viewer, Thanks for the bits! You're awesome!")]
    [InlineData(7, "@viewer, Thanks for giving 1000 bits!")]
    public void ThankForBits(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForBits(giftedBy: Viewer, bitsGiven: BITS_GIVEN);
        Assert.Equal(expected: message, actual: result);
    }

    private void MockRandomNumberGenerator(int number)
    {
        this._randomNumberGenerator.Next(Arg.Any<int>())
            .Returns(x =>
                     {
                         int max = x.Arg<int>();

                         return number % max;
                     });
    }
}