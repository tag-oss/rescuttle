namespace Rescuttle.Config;

public class StartupEndpointConfig : EndpointConfig
{
    /// <summary>
    /// Method to use against the endpoint
    /// </summary>
    public StartupEndpointHttpMethod Method { get; set; }
    
    public override HttpMethod GetHttpMethod()
    {
        switch (Method)
        {
            case StartupEndpointHttpMethod.Get:
                return HttpMethod.Get;
            case StartupEndpointHttpMethod.Post:
                return HttpMethod.Post;
            case StartupEndpointHttpMethod.Head:
                return HttpMethod.Head;
            case StartupEndpointHttpMethod.Options:
                return HttpMethod.Options;
            case StartupEndpointHttpMethod.Patch:
                return HttpMethod.Patch;
            case StartupEndpointHttpMethod.Put:
                return HttpMethod.Put;
            case StartupEndpointHttpMethod.Delete:
                return HttpMethod.Delete;
        }

        throw new InvalidOperationException("Cannot determine HttpClient HttpMethod to use, is config valid?");
    }
}