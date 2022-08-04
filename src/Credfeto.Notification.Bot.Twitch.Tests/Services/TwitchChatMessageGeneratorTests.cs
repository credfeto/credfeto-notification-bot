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

    private void ReceivedGetRandomNumber()
    {
        this._randomNumberGenerator.Received(1)
            .Next(Arg.Any<int>());
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for the bits!")]
    [InlineData(1, "Thanks @viewer for the bits! You're awesome!")]
    [InlineData(2, "Thanks @viewer for bits!")]
    [InlineData(3, "Thanks @viewer for giving 1000 bits!")]
    [InlineData(4, "Thanks @viewer for the 1000 bits!")]
    [InlineData(5, "@viewer, Thanks for the bits!")]
    [InlineData(6, "@viewer, Thanks for bits!")]
    [InlineData(7, "@viewer, Thanks for the bits! You're awesome!")]
    [InlineData(8, "@viewer, Thanks for the 1000 bits! You're awesome!")]
    [InlineData(9, "@viewer, Thanks for giving 1000 bits!")]
    [InlineData(10, "Thank you for the bits @viewer! VirtualHug")]
    [InlineData(11, "Thank you for the 1000 bits @viewer! VirtualHug")]
    [InlineData(12, "Thanks for the bits @viewer!")]
    [InlineData(13, "Thanks for the 1000 bits @viewer!")]
    [InlineData(14, "Thank you for cheering the bits @viewer!")]
    [InlineData(15, "Thank you for cheering 1000 bits @viewer")]
    [InlineData(16, "Thanks @viewer for the bits!")] // should have looped back to 0 with this
    public void ThankForBits(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForBits(giftedBy: Viewer, bitsGiven: BITS_GIVEN);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for subscribing!")]
    [InlineData(1, "Thanks @viewer for subscribing!")] // should have looped back to 0 with this
    public void ThanksForNewPaidSub(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForNewPaidSub(Viewer);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for subscribing!")]
    [InlineData(1, "Thanks @viewer for subscribing and using your prime sub here!")]
    [InlineData(2, "Thanks @viewer for subscribing!")] // should have looped back to 0 with this
    public void ThanksForNewPrimeSub(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForNewPrimeSub(Viewer);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for resubscribing!")]
    [InlineData(1, "Thanks @viewer for resubscribing!")] // should have looped back to 0 with this
    public void ThanksForPaidReSub(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForPaidReSub(Viewer);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for resubscribing!")]
    [InlineData(1, "Thanks @viewer for resubscribing!")] // should have looped back to 0 with this
    public void ThanksForPrimeReSub(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForPrimeReSub(Viewer);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for gifting subs!")]
    [InlineData(1, "Thanks @viewer for gifting subs!")] // should have looped back to 0 with this
    public void ThanksForGiftingMultipleSubs(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForGiftingMultipleSubs(Viewer);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
    }

    [Theory]
    [InlineData(0, "Thanks @viewer for gifting sub!")]
    [InlineData(1, "Thanks @viewer for gifting sub!")] // should have looped back to 0 with this
    public void ThanksForGiftingOneSub(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.ThanksForGiftingOneSub(Viewer);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
    }

    [Theory]
    [InlineData(0, "Hi @viewer")]
    [InlineData(1, "Hi @viewer")] // should have looped back to 0 with this
    public void WelcomeMessage(int number, string message)
    {
        this.MockRandomNumberGenerator(number);

        string result = this._twitchChatMessageGenerator.WelcomeMessage(Viewer);
        Assert.Equal(expected: message, actual: result);

        this.ReceivedGetRandomNumber();
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