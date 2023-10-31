using Rescuttle.Config;

namespace Rescuttle;
using Rescuttle.Config;

internal enum PollResult
{
    Success,
    FailWithIgnore,
    FailWithFail
}

internal class PollController : IDisposable
{
    private readonly EndpointConfig _endpoint;
    private readonly HttpClient _client;

    /// <summary>
    /// Polls all endpoints in the given endpoint array
    /// </summary>
    /// <param name="endpoints"></param>
    /// <returns>Returns false if the program should exit with a non-zero exit code, true if it should continue</returns>
    public static async Task<bool> PollAllAsync(IEnumerable<EndpointConfig> endpoints)
    {
        foreach (var pollEndpoint in endpoints)
        {
            var controller = new PollController(pollEndpoint);
            var poll = await controller.PollAsync();
            var message = $"Endpoint '{pollEndpoint.Name} with URL '{pollEndpoint.Uri}'";
            switch (poll.Result)
            {
                case PollResult.Success:
                    Logger.Log($"{message} completed successfully");
                    continue;
                case PollResult.FailWithFail:
                    Logger.Log($"{message} has failed and Rescuttle will exit");
                    return false; // Fail rescuttle
                case PollResult.FailWithIgnore:
                    Logger.Log($"{message} has failed but will be ignored");
                    continue;
            }
        }

        return true; // Continue
    }
    
    public PollController(EndpointConfig endpoint)
    {
        _endpoint = endpoint;
        _client = new HttpClient();
    }

    private async Task<(bool Success, HttpResponseMessage? Response, Exception? Exception)> SendRequestAsync(HttpRequestMessage request)
    {
        try
        {
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return (true, response, null);
        }
        catch (Exception e)
        {
            return (false, null, e);
        }
    }

    internal async Task<(PollResult Result, Exception? Exception)> PollAsync()
    {
        var requestCount = 0;
        Exception? lastError = null;
        while(requestCount <= _endpoint.MaxRetries)
        {
            var request = GetRequest();
            if (requestCount > 0)
                // This is a retry, wait the necessary time before sending another request
                await Task.Delay(TimeSpan.FromMilliseconds(_endpoint.RetryWaitMs));

            var result = await SendRequestAsync(request);

            Logger.Log(result.Exception == null
                ? $"Poll result {(result.Success ? "success" : "fail")}, status code {result.Response?.StatusCode}, while polling {request.RequestUri}"
                : $"Received exception while polling {request.RequestUri}: {result.Exception.ToString().ReplaceLineEndings(string.Empty)}");

            if (result.Success)
                return (PollResult.Success, null);

            lastError = result.Exception;
            requestCount++;
        }

        // All tries and retries have failed
        switch (_endpoint.OnFailure)
        {
            case FailureAction.Fail:
                return (PollResult.FailWithFail, lastError);
            default:
            case FailureAction.Ignore:
                return (PollResult.FailWithIgnore, lastError);
        }
    }

    private HttpRequestMessage GetRequest()
    {
        return new HttpRequestMessage
        {
            RequestUri = _endpoint.Uri,
            Method = _endpoint.GetHttpMethod()
        };
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}