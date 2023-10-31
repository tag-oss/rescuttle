using System.ComponentModel.DataAnnotations;

namespace Rescuttle.Config;

public class ShutdownEndpointConfig : EndpointConfig
{
    /// <summary>
    /// Method to use against the endpoint
    /// </summary>
    public ShutdownEndpointHttpMethod Method { get; set; }

    public override HttpMethod GetHttpMethod()
    {
        switch (Method)
        {
            case ShutdownEndpointHttpMethod.Get:
                return HttpMethod.Get;
            case ShutdownEndpointHttpMethod.Post:
                return HttpMethod.Post;
            case ShutdownEndpointHttpMethod.Head:
                return HttpMethod.Head;
            case ShutdownEndpointHttpMethod.Options:
                return HttpMethod.Options;
            case ShutdownEndpointHttpMethod.Patch:
                return HttpMethod.Patch;
            case ShutdownEndpointHttpMethod.Put:
                return HttpMethod.Put;
            case ShutdownEndpointHttpMethod.Delete:
                return HttpMethod.Delete;
        }

        throw new InvalidOperationException("Cannot determine HttpClient HttpMethod to use, is config valid?");
    }
}