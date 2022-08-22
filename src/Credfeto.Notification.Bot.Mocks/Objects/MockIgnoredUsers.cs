using System;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockIgnoredUsers : MockBase<string[]>
{
    public MockIgnoredUsers()
        : base(Array.Empty<string>())
    {
    }

    public override string[] Next()
    {
        return Array.Empty<string>();
    }
}