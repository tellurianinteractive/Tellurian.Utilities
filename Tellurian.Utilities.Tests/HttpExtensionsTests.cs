using System.Net;
using System.Text;
using System.Text.Json;
using Tellurian.Utilities.Web;

namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class HttpExtensionsTests(TestContext testContext)
{
    private sealed record Module(int Id, string Name);

    private sealed class FakeHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            send(request, cancellationToken);
    }

    private static HttpClient Client(HttpStatusCode status, string? json = null) =>
        new(new FakeHandler((_, _) => Task.FromResult(new HttpResponseMessage(status)
        {
            Content = json is null ? new ByteArrayContent([]) : new StringContent(json, Encoding.UTF8, "application/json")
        })))
        { BaseAddress = new Uri("https://example.com/") };

    [TestMethod]
    public async Task ToResultAsync_OkWithJson_IsSuccessWithData()
    {
        using var client = Client(HttpStatusCode.OK, """{"id":1,"name":"Station"}""");
        var result = await client.GetAsync(new Uri("modules/1", UriKind.Relative), testContext.CancellationToken).ToResultAsync<Module>(cancellationToken: testContext.CancellationToken);
        Assert.AreEqual(new Module(1, "Station"), result.DataOrDefault);
    }

    [TestMethod]
    public async Task ToResultAsync_NoContent_IsSuccessNoData()
    {
        using var client = Client(HttpStatusCode.NoContent);
        var result = await client.DeleteAsync(new Uri("modules/1", UriKind.Relative), testContext.CancellationToken).ToResultAsync<Module>(cancellationToken: testContext.CancellationToken);
        Assert.IsTrue(result is SuccessNoData);
    }

    [TestMethod]
    public async Task ToResultAsync_JsonNull_IsSuccessNoData()
    {
        using var client = Client(HttpStatusCode.OK, "null");
        var result = await client.GetAsync(new Uri("modules/1", UriKind.Relative), testContext.CancellationToken).ToResultAsync<Module>(cancellationToken: testContext.CancellationToken);
        Assert.IsTrue(result is SuccessNoData);
    }

    [TestMethod]
    public async Task ToResultAsync_NotFound_IsFailureWithStatusCode()
    {
        using var client = Client(HttpStatusCode.NotFound, """{"title":"Not found"}""");
        var result = await client.GetAsync(new Uri("modules/9", UriKind.Relative), testContext.CancellationToken).ToResultAsync<Module>(cancellationToken: testContext.CancellationToken);
        Assert.IsTrue(result is FailureWithStatusCode { StatusCode: 404 });
    }

    [TestMethod]
    public async Task ToResultAsync_InvalidJson_IsFailureWithException()
    {
        using var client = Client(HttpStatusCode.OK, "<html></html>");
        var result = await client.GetAsync(new Uri("modules/1", UriKind.Relative), testContext.CancellationToken).ToResultAsync<Module>(cancellationToken: testContext.CancellationToken);
        Assert.IsTrue(result is FailureWithException { Exception: JsonException });
    }

    [TestMethod]
    public async Task ToResultAsync_RequestFails_IsFailureWithException()
    {
        using var client = new HttpClient(new FakeHandler((_, _) => throw new HttpRequestException("No connection")))
        { BaseAddress = new Uri("https://example.com/") };
        var result = await client.GetAsync(new Uri("modules/1", UriKind.Relative), testContext.CancellationToken).ToResultAsync<Module>(cancellationToken: testContext.CancellationToken);
        Assert.IsTrue(result is FailureWithException { Exception: HttpRequestException });
    }

    [TestMethod]
    public async Task ToResultAsync_ClientTimeout_IsTimedOut()
    {
        using var client = new HttpClient(new FakeHandler(async (_, token) =>
        {
            await Task.Delay(Timeout.Infinite, token);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }))
        { BaseAddress = new Uri("https://example.com/"), Timeout = TimeSpan.FromMilliseconds(50) };
        var result = await client.GetAsync(new Uri("modules/1", UriKind.Relative), testContext.CancellationToken).ToResultAsync<Module>(cancellationToken: testContext.CancellationToken);
        Assert.IsTrue(result is TimedOut);
    }

    [TestMethod]
    public async Task ToResultAsync_Cancelled_Throws()
    {
        using var cancellation = new CancellationTokenSource();
        using var client = new HttpClient(new FakeHandler(async (_, token) =>
        {
            await Task.Delay(Timeout.Infinite, token);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }))
        { BaseAddress = new Uri("https://example.com/") };
        var request = client.GetAsync(new Uri("modules/1", UriKind.Relative), cancellation.Token);
        await cancellation.CancelAsync();
        await Assert.ThrowsAsync<TaskCanceledException>(() => request.ToResultAsync<Module>(cancellationToken: cancellation.Token));
    }

    [TestMethod]
    public async Task ToResultAsync_WithJsonTypeInfo_IsSuccessWithData()
    {
        using var client = Client(HttpStatusCode.OK, """{"id":2,"name":"Yard"}""");
        var result = await client.GetAsync(new Uri("modules/2", UriKind.Relative), testContext.CancellationToken)
            .ToResultAsync(TestJsonContext.Default.ModuleData, testContext.CancellationToken);
        Assert.AreEqual(new ModuleData(2, "Yard"), result.DataOrDefault);
    }

    [TestMethod]
    public async Task ToResultAsync_ResponseWithJsonTypeInfo_NotFound_IsFailureWithStatusCode()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NotFound);
        var result = await response.ToResultAsync(TestJsonContext.Default.ModuleData, testContext.CancellationToken);
        Assert.IsTrue(result is FailureWithStatusCode { StatusCode: 404 });
    }
}
