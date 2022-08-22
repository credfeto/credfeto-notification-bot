using System.Collections.Generic;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockIgnoredUsers : MockBase<List<string>>
{
    public MockIgnoredUsers()
        : base(new())
    {
    }

    public override List<string> Next()
    {
        return new();
    }
}