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

    /// <summary>A result holding a success with data.</summary>
    /// <param name="value">The case.</param>
    public Result(SuccessWithData<T> value) => Value = value;

    /// <summary>A result holding a success without data.</summary>
    /// <param name="value">The case.</param>
    public Result(SuccessNoData value) => Value = value;

    /// <summary>A result holding a failure caused by an exception.</summary>
    /// <param name="value">The case.</param>
    public Result(FailureWithException value) => Value = value;

    /// <summary>A result holding a failure with a status code.</summary>
    /// <param name="value">The case.</param>
    public Result(FailureWithStatusCode value) => Value = value;

    /// <summary>A result holding a timeout.</summary>
    /// <param name="value">The case.</param>
    public Result(TimedOut value) => Value = value;

    /// <summary>The case the result holds; never null.</summary>
    public object Value { get; }

    /// <summary>Results are equal when they hold equal cases.</summary>
    public bool Equals(Result<T>? other) => other is not null && Value.Equals(other.Value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => Value.GetHashCode();

    /// <summary>Results are equal when they hold equal cases.</summary>
    public static bool operator ==(Result<T>? left, Result<T>? right) => Equals(left, right);

    /// <summary>Results are unequal when they hold different cases.</summary>
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
