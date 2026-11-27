using System.Net;
using System.Text.Json;

namespace Tellurian.Utilities.Web;

public static class HttpExtensions
{
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
    /// The response is not disposed.
    /// </summary>
    /// <param name="response">The response to read.</param>
    /// <param name="options">JSON options; the default is <see cref="JsonSerializerOptions.Web"/>.</param>
    /// <param name="cancellationToken">Cancels reading the body; a cancellation is thrown, not returned as a result.</param>
    public static async Task<Result<T>> ToResultAsync<T>(this HttpResponseMessage response, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(response);
        if (!response.StatusCode.IsSuccess) return new FailureWithStatusCode((int)response.StatusCode);
        try
        {
            var body = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            if (body.Length == 0) return new SuccessNoData();
            var data = JsonSerializer.Deserialize<T>(body, options ?? JsonSerializerOptions.Web);
            return data is null ? new SuccessNoData() : new SuccessWithData<T>(data);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return exception.AsResult<T>();
        }
    }

    /// <summary>
    /// Awaits a request, such as <c>client.GetAsync(url)</c>, and gives the response as a <see cref="Result{T}"/>
    /// (see <see cref="ToResultAsync{T}(HttpResponseMessage, JsonSerializerOptions?, CancellationToken)"/>), and disposes it.
    /// A failed request gives <see cref="FailureWithException"/>, and the <see cref="HttpClient"/> timeout <see cref="TimedOut"/>.
    /// </summary>
    /// <param name="request">The request to await.</param>
    /// <param name="options">JSON options; the default is <see cref="JsonSerializerOptions.Web"/>.</param>
    /// <param name="cancellationToken">
    /// The token also given to the request; its cancellation is thrown, not returned as a result.
    /// </param>
    public static async Task<Result<T>> ToResultAsync<T>(this Task<HttpResponseMessage> request, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        try
        {
            using var response = await request.ConfigureAwait(false);
            return await response.ToResultAsync<T>(options, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (!cancellationToken.IsCancellationRequested)
        {
            return exception.AsResult<T>();
        }
    }
}
