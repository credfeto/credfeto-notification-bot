using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace Credfeto.NotificationBot.Shared.Configuration;

public sealed class ConfigurationErrorsException : Exception
{
    public ConfigurationErrorsException(IReadOnlyList<ValidationFailure> errors)
        : this("Configuration is invalid")
    {
        this.Errors = errors;
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Standard exception constructor")]
    public ConfigurationErrorsException()
        : this("Configuration is invalid")
    {
        this.Errors = Array.Empty<ValidationFailure>();
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Standard exception constructor")]
    public ConfigurationErrorsException(string? message)
        : base(message)
    {
        this.Errors = Array.Empty<ValidationFailure>();
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Standard exception constructor")]
    public ConfigurationErrorsException(string? message, Exception? innerException)
        : base(message: message, innerException: innerException)
    {
        this.Errors = Array.Empty<ValidationFailure>();
    }

    [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "Used buy clients")]
    public IReadOnlyList<ValidationFailure> Errors { get; }
}