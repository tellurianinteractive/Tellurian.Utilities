using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Tellurian.Utilities.Web;

/// <summary>
/// Extensions for HTTP status codes and responses.
/// </summary>
public static class HttpExtensions
{
    private const string ReflectionJustification =
        "Deserializes with reflection-based JSON metadata; use the overload that takes a JsonTypeInfo<T> from a JsonSerializerContext under trimming or Native AOT.";

    extension(HttpStatusCode code)
    {
        /// <summary>
        /// Gets a value indicating whether the response represents a successful status code.
        /// </summary>
        public bool IsSuccess => (int)code >= 200 && (int)code < 300;
    }

    // Classic static methods: generic extension members can fail under Blazor WebAssembly (TypeLoadException).

    /// <summary>
    /// The response as a <see cref="Result{T}"/>: for a 2xx status, <see cref="SuccessWithData{T}"/> with the JSON body,
    /// or <see cref="SuccessNoData"/> when the body is empty or JSON <c>null</c>; otherwise <see cref="FailureWithStatusCode"/>.
    /// A body that isn't valid JSON for <typeparamref name="T"/> gives <see cref="FailureWithException"/>.
    /// The response is not disposed. Uses reflection; under trimming or Native AOT use
    /// <see cref="ToResultAsync{T}(HttpResponseMessage, JsonTypeInfo{T}, CancellationToken)"/>.
    /// </summary>
    /// <param name="response">The response to read.</param>
    /// <param name="options">JSON options; the default is <see cref="JsonSerializerOptions.Web"/>.</param>
    /// <param name="cancellationToken">Cancels reading the body; a cancellation is thrown, not returned as a result.</param>
    [RequiresUnreferencedCode(ReflectionJustification)]
    [RequiresDynamicCode(ReflectionJustification)]
    public static Task<Result<T>> ToResultAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default) =>
        response.ToResultAsync(body => JsonSerializer.Deserialize<T>(body, options ?? JsonSerializerOptions.Web), cancellationToken);

    /// <summary>
    /// The response as a <see cref="Result{T}"/>, deserialized with source-generated JSON metadata, which is safe under trimming
    /// and Native AOT: for a 2xx status, <see cref="SuccessWithData{T}"/> with the JSON body, or <see cref="SuccessNoData"/>
    /// when the body is empty or JSON <c>null</c>; otherwise <see cref="FailureWithStatusCode"/>. A body that isn't valid JSON
    /// for <typeparamref name="T"/> gives <see cref="FailureWithException"/>. The response is not disposed.
    /// </summary>
    /// <param name="response">The response to read.</param>
    /// <param name="jsonTypeInfo">The JSON metadata of <typeparamref name="T"/>, such as <c>AppJsonContext.Default.Module</c>.</param>
    /// <param name="cancellationToken">Cancels reading the body; a cancellation is thrown, not returned as a result.</param>
    public static Task<Result<T>> ToResultAsync<T>(this HttpResponseMessage response, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);
        return response.ToResultAsync(body => JsonSerializer.Deserialize(body, jsonTypeInfo), cancellationToken);
    }

    /// <summary>
    /// Awaits a request, such as <c>client.GetAsync(url)</c>, and gives the response as a <see cref="Result{T}"/>
    /// (see <see cref="ToResultAsync{T}(HttpResponseMessage, JsonSerializerOptions?, CancellationToken)"/>), and disposes it.
    /// A failed request gives <see cref="FailureWithException"/>, and the <see cref="HttpClient"/> timeout <see cref="TimedOut"/>.
    /// Uses reflection; under trimming or Native AOT use
    /// <see cref="ToResultAsync{T}(Task{HttpResponseMessage}, JsonTypeInfo{T}, CancellationToken)"/>.
    /// </summary>
    /// <param name="request">The request to await.</param>
    /// <param name="options">JSON options; the default is <see cref="JsonSerializerOptions.Web"/>.</param>
    /// <param name="cancellationToken">
    /// The token also given to the request; its cancellation is thrown, not returned as a result.
    /// </param>
    [RequiresUnreferencedCode(ReflectionJustification)]
    [RequiresDynamicCode(ReflectionJustification)]
    public static Task<Result<T>> ToResultAsync<T>(this Task<HttpResponseMessage> request, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default) =>
        request.ToResultAsync(body => JsonSerializer.Deserialize<T>(body, options ?? JsonSerializerOptions.Web), cancellationToken);

    /// <summary>
    /// Awaits a request, such as <c>client.GetAsync(url)</c>, and gives the response as a <see cref="Result{T}"/>
    /// (see <see cref="ToResultAsync{T}(HttpResponseMessage, JsonTypeInfo{T}, CancellationToken)"/>), and disposes it.
    /// A failed request gives <see cref="FailureWithException"/>, and the <see cref="HttpClient"/> timeout <see cref="TimedOut"/>.
    /// </summary>
    /// <param name="request">The request to await.</param>
    /// <param name="jsonTypeInfo">The JSON metadata of <typeparamref name="T"/>, such as <c>AppJsonContext.Default.Module</c>.</param>
    /// <param name="cancellationToken">
    /// The token also given to the request; its cancellation is thrown, not returned as a result.
    /// </param>
    public static Task<Result<T>> ToResultAsync<T>(this Task<HttpResponseMessage> request, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(jsonTypeInfo);
        return request.ToResultAsync(body => JsonSerializer.Deserialize(body, jsonTypeInfo), cancellationToken);
    }

    private static async Task<Result<T>> ToResultAsync<T>(this HttpResponseMessage response, Func<byte[], T?> deserialize, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(response);
        if (!response.StatusCode.IsSuccess) return new FailureWithStatusCode((int)response.StatusCode);
        try
        {
            var body = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            if (body.Length == 0) return new SuccessNoData();
            var data = deserialize(body);
            return data is null ? new SuccessNoData() : new SuccessWithData<T>(data);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return exception.AsResult<T>();
        }
    }

    private static async Task<Result<T>> ToResultAsync<T>(this Task<HttpResponseMessage> request, Func<byte[], T?> deserialize, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            using var response = await request.ConfigureAwait(false);
            return await response.ToResultAsync(deserialize, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return exception.AsResult<T>();
        }
    }
}
