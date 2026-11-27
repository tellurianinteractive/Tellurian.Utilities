namespace Tellurian.Utilities.Tests;

[TestClass]
public sealed class ResultTests
{
    private static string Describe(Result<int> result) => result switch
    {
        SuccessWithData<int> success => $"Data {success.Data}",
        SuccessNoData => "No data",
        FailureWithException failure => failure.Exception.Message,
        FailureWithStatusCode failure => $"Status {failure.StatusCode}",
        TimedOut => "Timed out",
    };

    [TestMethod]
    public void Switch_EachCase_IsMatched()
    {
        Assert.AreEqual("Data 42", Describe(new SuccessWithData<int>(42)));
        Assert.AreEqual("No data", Describe(new SuccessNoData()));
        Assert.AreEqual("Oops", Describe(new FailureWithException(new InvalidOperationException("Oops"))));
        Assert.AreEqual("Status 404", Describe(new FailureWithStatusCode(404)));
        Assert.AreEqual("Timed out", Describe(new TimedOut()));
    }

    [TestMethod]
    public void IsSuccess_SuccessCases_IsTrue()
    {
        Result<int> withData = new SuccessWithData<int>(1);
        Result<int> noData = new SuccessNoData();
        Assert.IsTrue(withData.IsSuccess);
        Assert.IsTrue(noData.IsSuccess);
        Assert.IsFalse(withData.IsFailure);
    }

    [TestMethod]
    public void IsFailure_FailureCases_IsTrue()
    {
        Result<int> exception = new FailureWithException(new InvalidOperationException());
        Result<int> statusCode = new FailureWithStatusCode(500);
        Result<int> timedOut = new TimedOut();
        Assert.IsTrue(exception.IsFailure);
        Assert.IsTrue(statusCode.IsFailure);
        Assert.IsTrue(timedOut.IsFailure);
        Assert.IsFalse(timedOut.IsSuccess);
    }

    [TestMethod]
    public void Value_IsTheCase()
    {
        Result<int> result = new FailureWithStatusCode(404);
        Assert.AreEqual(new FailureWithStatusCode(404), result.Value);
    }

    [TestMethod]
    public void TryGetData_WithData_ReturnsData()
    {
        Result<string> result = new SuccessWithData<string>("Text");
        Assert.IsTrue(result.TryGetData(out var data));
        Assert.AreEqual("Text", data);
        Assert.AreEqual("Text", result.DataOrDefault);
    }

    [TestMethod]
    public void TryGetData_NoData_ReturnsFalse()
    {
        Result<string> result = new SuccessNoData();
        Assert.IsFalse(result.TryGetData(out var data));
        Assert.IsNull(data);
        Assert.IsNull(result.DataOrDefault);
    }

    [TestMethod]
    public void GetMessage_EachCase()
    {
        Assert.AreEqual("Succeeded.", ((Result<int>)new SuccessWithData<int>(1)).GetMessage());
        Assert.AreEqual("Succeeded.", ((Result<int>)new SuccessNoData()).GetMessage());
        Assert.AreEqual("Oops", ((Result<int>)new FailureWithException(new InvalidOperationException("Oops"))).GetMessage());
        Assert.AreEqual("Failed with status code 404.", ((Result<int>)new FailureWithStatusCode(404)).GetMessage());
        Assert.AreEqual("Timed out.", ((Result<int>)new TimedOut()).GetMessage());
    }

    [TestMethod]
    public void Equals_SameCaseAndData_IsTrue()
    {
        Result<int> first = new SuccessWithData<int>(1);
        Result<int> second = new SuccessWithData<int>(1);
        Assert.IsTrue(first == second);
        Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
    }

    [TestMethod]
    public void Equals_OtherDataOrCase_IsFalse()
    {
        Result<int> one = new SuccessWithData<int>(1);
        Result<int> two = new SuccessWithData<int>(2);
        Result<int> timedOut = new TimedOut();
        Assert.IsTrue(one != two);
        Assert.IsTrue(one != timedOut);
        Assert.IsFalse(one.Equals(default));
    }

    [TestMethod]
    public void AsResult_TimeoutException_IsTimedOut() =>
        Assert.IsTrue(new TimeoutException().AsResult<int>() is TimedOut);

    [TestMethod]
    public void AsResult_HttpClientTimeout_IsTimedOut() =>
        Assert.IsTrue(new TaskCanceledException("Timeout", new TimeoutException()).AsResult<int>() is TimedOut);

    [TestMethod]
    public void AsResult_Cancellation_IsFailureWithException() =>
        Assert.IsTrue(new TaskCanceledException().AsResult<int>() is FailureWithException);

    [TestMethod]
    public void AsResult_OtherException_KeepsException()
    {
        var exception = new InvalidOperationException("Oops");
        var result = exception.AsResult<int>();
        Assert.IsTrue(result is FailureWithException { Exception: var kept } && ReferenceEquals(exception, kept));
    }
}
