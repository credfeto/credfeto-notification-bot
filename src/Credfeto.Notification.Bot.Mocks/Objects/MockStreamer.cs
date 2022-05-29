using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockStreamer : MockBase<Streamer>
{
    public MockStreamer()
        : base(Streamer.FromString(nameof(Streamer)))
    {
    }

    public override Streamer Next()
    {
        return Streamer.FromString("STREAMER:" + Guid.NewGuid());
    }
}