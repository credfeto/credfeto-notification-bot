using System.Collections.Generic;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockHeists : MockBase<List<string>>
{
    public MockHeists()
        : base(new())
    {
    }

    public override List<string> Next()
    {
        return new();
    }
}