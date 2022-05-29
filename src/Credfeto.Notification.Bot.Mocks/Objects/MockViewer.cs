using Credfeto.Notification.Bot.Twitch.DataTypes;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockViewer : MockBase<Viewer>
{
    public MockViewer()
        : base(Viewer.FromString(nameof(Viewer)))
    {
    }

    public override Viewer Next()
    {
        return Viewer.FromString("VIEWER:" + Guid.NewGuid());
    }
}