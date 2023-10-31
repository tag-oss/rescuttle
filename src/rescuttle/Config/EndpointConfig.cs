using System.ComponentModel.DataAnnotations;

namespace Rescuttle.Config;

public abstract class EndpointConfig
{
    public string Name { get; set; } = null!;

    /// <summary>
    /// Full Uri to the endpoint
    /// </summary>
    public Uri Uri { get; set; } = null!;

    /// <summary>
    /// What action to take when a non-200 response code is received from the Uri
    /// </summary>
    public FailureAction OnFailure { get; set; }
    
    /// <summary>
    /// Maximum number of attempts to retry the endpoint (if a retry action is specified for OnFailure)
    /// </summary>
    public int MaxRetries { get; set; } = 5;
    
    /// <summary>
    /// How long to wait between retries in milliseconds
    /// </summary>
    public int RetryWaitMs { get; set; } = 1000;

    /// <summary>
    /// The source or owner of this endpoint (end user, created by a mode, etc.)
    /// </summary>
    public EndpointConfigSource Source { get; set; } = EndpointConfigSource.UserInput;

    public abstract HttpMethod GetHttpMethod();
}