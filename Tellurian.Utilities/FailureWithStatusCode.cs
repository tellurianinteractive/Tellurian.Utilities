namespace Tellurian.Utilities;

/// <summary>A failed result with a status code, for example an HTTP status code.</summary>
/// <param name="StatusCode">The status code telling why the operation failed.</param>
public readonly record struct FailureWithStatusCode(int StatusCode);
