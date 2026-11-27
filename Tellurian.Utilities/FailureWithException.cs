using System.Diagnostics.CodeAnalysis;

namespace Tellurian.Utilities;

/// <summary>A failed result caused by an exception.</summary>
/// <param name="Exception">The exception that the operation threw.</param>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Named after its content, like the other cases; it is no exception type.")]
public readonly record struct FailureWithException(Exception Exception);
