namespace Tellurian.Utilities;

/// <summary>
/// Extensions for <see cref="Result{T}"/>.
/// </summary>
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
