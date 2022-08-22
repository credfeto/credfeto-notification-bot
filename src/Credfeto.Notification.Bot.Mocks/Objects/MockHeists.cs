using System;
using FunFair.Test.Common.Mocks;

namespace Credfeto.Notification.Bot.Mocks.Objects;

internal sealed class MockHeists : MockBase<string[]>
{
    public MockHeists()
        : base(Array.Empty<string>())
    {
    }

    public override string[] Next()
    {
        return Array.Empty<string>();
    }
}