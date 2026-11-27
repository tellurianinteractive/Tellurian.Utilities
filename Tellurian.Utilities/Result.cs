using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Tellurian.Utilities;

/// <summary>
/// The outcome of an operation: a success with or without data, or a failure with an exception, a status code or a timeout.
/// Each case converts implicitly to the result, and a <c>switch</c> over the cases is exhaustive:
/// <code>
/// var text = result switch
/// {
///     SuccessWithData&lt;Module&gt; success => success.Data.Name,
///     SuccessNoData => "Saved",
///     FailureWithException failure => failure.Exception.Message,
///     FailureWithStatusCode failure => $"Status {failure.StatusCode}",
///     TimedOut => "Timed out",
/// };
/// </code>
/// A result always holds one of the cases. It is a class, not a struct union, so there is no empty <c>default</c> result,
/// and nullable reference types keep a null result away.
/// </summary>
/// <typeparam name="T">The type of data of a successful result.</typeparam>
[Union]
public sealed class Result<T> : IUnion, IEquatable<Result<T>>
{
    // The constructors define the case types and give the implicit conversions from them.
    public Result(SuccessWithData<T> value) => Value = value;
    public Result(SuccessNoData value) => Value = value;
    public Result(FailureWithException value) => Value = value;
    public Result(FailureWithStatusCode value) => Value = value;
    public Result(TimedOut value) => Value = value;

    /// <summary>The case the result holds; never null.</summary>
    public object Value { get; }

    /// <summary>Results are equal when they hold equal cases.</summary>
    public bool Equals(Result<T>? other) => other is not null && Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public static bool operator ==(Result<T>? left, Result<T>? right) => Equals(left, right);
    public static bool operator !=(Result<T>? left, Result<T>? right) => !Equals(left, right);

    /// <summary>True for <see cref="SuccessWithData{T}"/> and <see cref="SuccessNoData"/>.</summary>
    public bool IsSuccess => this is SuccessWithData<T> or SuccessNoData;

    /// <summary>True for <see cref="FailureWithException"/>, <see cref="FailureWithStatusCode"/> and <see cref="TimedOut"/>.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The data of a <see cref="SuccessWithData{T}"/>, otherwise the default of <typeparamref name="T"/>.</summary>
    public T? DataOrDefault => this is SuccessWithData<T> success ? success.Data : default;

    /// <summary>Gets the data of a <see cref="SuccessWithData{T}"/>.</summary>
    /// <param name="data">The data, or the default of <typeparamref name="T"/> when the result holds no data.</param>
    /// <returns>True if the result is a <see cref="SuccessWithData{T}"/>.</returns>
    public bool TryGetData([MaybeNullWhen(false)] out T data)
    {
        if (this is SuccessWithData<T> success)
        {
            data = success.Data;
            return true;
        }
        data = default;
        return false;
    }
}

/// <summary>A successful result with data.</summary>
/// <param name="Data">The data the operation gave.</param>
public readonly record struct SuccessWithData<T>(T Data);

/// <summary>A successful result without data, for example after a save or a delete.</summary>
public readonly record struct SuccessNoData;

/// <summary>A failed result caused by an exception.</summary>
/// <param name="Exception">The exception that the operation threw.</param>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Named after its content, like the other cases; it is no exception type.")]
public readonly record struct FailureWithException(Exception Exception);

/// <summary>A failed result with a status code, for example an HTTP status code.</summary>
/// <param name="StatusCode">The status code telling why the operation failed.</param>
public readonly record struct FailureWithStatusCode(int StatusCode);

/// <summary>A failed result because the operation took too long.</summary>
public readonly record struct TimedOut;

public static class ResultExtensions
{
    // Classic static methods: generic extension members can fail under Blazor WebAssembly (TypeLoadException).

    /// <summary>
    /// The exception as a failed result: <see cref="TimedOut"/> for a <see cref="TimeoutException"/> and for the
    /// <see cref="TaskCanceledException"/> that <see cref="HttpClient"/> throws on its timeout, otherwise <see cref="FailureWithException"/>.
    /// </summary>
    public static Result<T> AsResult<T>(this Exception exception) => exception switch
    {
        TimeoutException or TaskCanceledException { InnerException: TimeoutException } => new TimedOut(),
        _ => new FailureWithException(exception),
    };

    /// <summary>
    /// A short text telling the outcome, for every case: "Succeeded.", the exception message,
    /// "Failed with status code 404.", "Timed out.".
    /// </summary>
    public static string GetMessage<T>(this Result<T> result) => result switch
    {
        SuccessWithData<T> or SuccessNoData => "Succeeded.",
        FailureWithException failure => failure.Exception?.Message ?? "Failed.",
        FailureWithStatusCode failure => $"Failed with status code {failure.StatusCode}.",
        TimedOut => "Timed out.",
    };
}
