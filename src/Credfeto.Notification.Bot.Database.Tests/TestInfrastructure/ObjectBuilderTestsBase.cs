using System;
using Credfeto.Notification.Bot.Database.Interfaces.Builders;
using FunFair.Test.Common;
using Xunit;

namespace Credfeto.Notification.Bot.Database.Tests.TestInfrastructure;

public abstract class ObjectBuilderTestsBase<TBuilder, TSourceObject, TDestinationObject> : TestBase
    where TBuilder : class, IObjectBuilder<TSourceObject, TDestinationObject> where TSourceObject : class, new() where TDestinationObject : class

{
    private readonly IObjectBuilder<TSourceObject, TDestinationObject> _builder;

    protected ObjectBuilderTestsBase(TBuilder builder)
    {
        this._builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    protected TDestinationObject? Build(TSourceObject source)
    {
        return this._builder.Build(source);
    }

    protected abstract TSourceObject CreateValidObject();

    protected void ShouldThrow<TException>(TSourceObject source)
        where TException : Exception
    {
        Assert.Throws<TException>(() => this.Build(source));
    }

    protected void ShouldBeNull(TSourceObject source)
    {
        TDestinationObject? result = this.Build(source);
        Assert.Null(result);
    }

    protected void ShouldNotBeNull(TSourceObject source)
    {
        TDestinationObject? result = this.Build(source);
        Assert.NotNull(result);
    }

    [Fact]
    public void CreatingValidAlwaysWorks()
    {
        TSourceObject source = this.CreateValidObject();
        this.ShouldNotBeNull(source);
    }
}