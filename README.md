# Rescuttle

Rescuttle provides pre-flight and post-flight webhooks for applications running in containers.  The main motivation was to ensure containers with Istio sidecars wait until the sidecar is ready to start and stop the Istio sidecar when the container exits.  This is a common issue with Kubernetes Job, where the Job continues on forever because the Istio sidecar does not exit.

Rescuttle ships with Istio support baked in, but you can extend it to work with any HTTP endpoint for your own use-cases as well.

See [Changelog](CHANGELOG.md) for change history.  See [Docker Hub](https://hub.docker.com/repository/docker/tagoss/rescuttle/tags?page=1&ordering=last_updated) for a list of image tags and releases.

## Quick Start using Istio Mode

Copy Rescuttle into your Dockerfile, place it in your entrypoint in front of your original entrypoint:

```
FROM my-base-image

# Add this line to copy the Rescuttle binary into your image
COPY --from=tagoss/rescuttle:latest /app/rescuttle .

# Enable Istio Mode
ENV Rescuttle__Istio__Enabled="true"

# Insert rescuttle into the first argument of your entrypoint
ENTRYPOINT ["rescuttle", "echo", "hello", "world"]
```

**Note:** We have multi-architecture builds for Linux x64 and arm64.  For Alpine you may want to use the `muslc` tag for the musl-c compiled binary. See a full list: [Docker Hub page](https://hub.docker.com/repository/docker/tagoss/rescuttle/tags).

That's it, if you build and push this container to a Kubernetes cluster with Istio sidecars it will now:

1. Poll Istio's health endpoints until Istio is ready
2. Start your container's workload, `echo hello world` in this example
3. Capture your workload's exit code when it completed
4. Tell the Istio sidecar to exit using it's `/quitquitquit` endpoint
5. Rescuttle will now exit, passing the same exit code your workload returned

## Advanced Configurations

### Basics

At it's core Rescuttle simply:

1. Does some HTTP calls on startup (with polling, error handling, backoff, etc.)
2. Runs another application
3. Does some HTTP calls on exit  (with polling, error handling, backoff, etc.)
4. Exits (passing the exit code of the application run in step #2)

Rescuttle supports configuration options to allow you to use the preexisting Istio mode, your own HTTP endpoints, or a combination of both.

### JSON File

If you place a `rescuttle.json` file in your working directly Rescuttle will read it, parse it and change it's behavior accordingly.  For example:

```json5
{
    // Enable or disable Istio mode
    "Istio": {
      "Enabled": true
    },
    // Add your own Startup endpoints (in addition to Istio ones, if enabled)
    "StartupEndpoints": [
      {
        "Name": "A Custom Startup Poller", // A name for your endpoint, used in logging
        "URI": "https://httpbin.org/get",
        "Method": "get", // What method to use
        "MaxRetries": 5, // How many retries before giving up, 0 to disable retries
        "RetryWaitMs": 1000, // Milliseconds between retries
        "OnFailure": "Fail" // Fail/Ignore - if Fail Rescuttle will exit after an endpoint fails (after the last retry)
      }
    ],
    // Add your own Shutdown endpoints (in addition to Istio ones, if enabled)
    "ShutdownEndpoints": [
      {
        "Name": "A Custom Shutdown Webhook", // A name for your endpoint, used in logging
        "URI": "https://httpbin.org/post",
        "Method": "post", // What method to use
        "MaxRetries": 0, // How many retries before giving up, 0 to disable retries
        "RetryWaitMs": 1000, // Milliseconds between retries
        "OnFailure": "Ignore" // Fail/Ignore - if Fail Rescuttle will exit after an endpoint fails (after the last retry)
      }
    ]
}
```

Spec:

* **Istio.Enabled**: Enabled or Disables Istio mode, which injects Istio startup and shutdown endpoints into the config ([IstioModeConfig class](src/rescuttle/Config/IstioModeConfig.cs))
* **StartupEndpoints**: Endpoints that should be polled _before_ starting the container's workload (can be an empty array) ([StartupEndpointConfig class](src/rescuttle/Config/StartupEndpointConfig.cs))
* **ShutdownEndpoints**: Endpoints that should be hit _after_ the container's workload exits (can be an empty array) ([ShutdownEndpointConfig class](src/rescuttle/Config/ShutdownEndpointConfig.cs))

Key Points:

* You do not need to specify `ShutdownEndpoints` and `StartupEndpoints`
* If a mode is used, those endpoints are added automatically
* You can mix/match custom endpoints and modes (e.g. You can hit httpbin.org and enable Istio mode)

### Env Vars

All of the options available using the above JSON file options are also available via env vars.  Each JSON key should be set with two underscores separating nested objects.  Array indexes are also set in this way.  

Simple Example: `Rescuttle.Istio.Enabled: true` becomes `RESCUTTLE__Istio__Enabled=true`.

A more complicated example, taking the above `StartupEndpoints` in JSON you would do:

```properties
Rescuttle__StartupEndpoints__0__Name=A Custom Startup Poller
Rescuttle__StartupEndpoints__0__Uri=https://httpbin.org/get
Rescuttle__StartupEndpoints__0__Method=get
Rescuttle__StartupEndpoints__0__RetryWaitMs=1000
```

If you needed to define a second endpoint, change the 0 to a 1 and so on:

```properties
Rescuttle__StartupEndpoints__1__Name=A Custom Startup Poller, again
Rescuttle__StartupEndpoints__1__Uri=https://httpbin.org/get
Rescuttle__StartupEndpoints__1__Method=get
Rescuttle__StartupEndpoints__1__RetryWaitMs=1000
```

### When to use JSON vs Env vars

It is completely up to you!  Our recommendation would be to use Env vars if you only use preexisting modes, such as the Istio mode.  If you define custom endpoints, it would probably be easier to read in JSON format but it isn't a requirement.  Do whatever works best for you.

### Advanced Istio Mode Options

You may want to tweak Istio mode's defaults in certain situations.  You can disable Istio mode and specify your own configuration, or use the below options (json and env vars both supported):

```json5
{
    "Istio": {
      // Enable or disable istio mode entirely
      "Enabled": "true",
      // Enable or disable Startup probes
      "StartupEnabled": true,
      // How to handle failures on startup probes (fail throws an exception and kills the container)
      "OnStartupFailure": "fail",
      // Startup max retries
      "StartupMaxRetries": 0,
      // Enable or disable Shutdown endpoints on exit
      "ShutdownEnabled": true,
      // Max retries for Shutdown endpoints
      "ShutdownMaxRetries": 500,
      // How to handle failures on Istio's /quitquitquit endpoint.  Ignore means retries (if configured)
      // will take place, but if they all fail Rescuttle will move on silently and not exit a non-zero exit code
      "OnShutdownFailure": "ignore"
    }
}
```

You can see defaults in this class: [IstioModeConfig.cs](src/rescuttle/Config/IstioModeConfig.cs).  You can convert any of the above
JSON to env var config if needed, for example: `RESCUTTLE__ISTIO__ENABLED=true` or `RESCUTTLE__ISTIO__SHUTDOWNMAXRETRIES=9999`.

# Rescuttle vs. Scuttle

Rescuttle is maintained by some of the same team that created [scuttle](https://github.com/redboxllc/scuttle), a Go app for the same purpose.  That repository is no longer maintained.  This repository is a ground-up rewrite in C#/.NET as this is what our team is more familiar with (Scuttle is in Golang).  Similar to the Golang predecessor, Rescuttle is distributed as a single executable binary.  Rescuttle was rewritten to be more flexible and have more clear configuration options, but the general idea is still the same - so if you have used Scuttle in the past, give Rescuttle a try!