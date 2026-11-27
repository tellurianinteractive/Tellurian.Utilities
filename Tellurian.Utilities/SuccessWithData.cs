namespace Tellurian.Utilities;

/// <summary>A successful result with data.</summary>
/// <param name="Data">The data the operation gave.</param>
public readonly record struct SuccessWithData<T>(T Data);
