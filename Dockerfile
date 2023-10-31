# Easiest to run locally using: 
#   docker buildx build --build-arg=VERSION=1.0.0.0 --platform=linux/amd64 --progress=plain --no-cache .
FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
COPY . .
ARG VERSION
ARG TARGET
# Tests
RUN dotnet test
# Set by docker build x
ARG TARGETPLATFORM
COPY get-dotnet-rid.sh .
# Use script to determine current architecture from Docker build multi-arch builds
# then map that to the correct single binary dotnet runtime id (RID) so we can do a single binary build
# if TARGET arg is set, get-dotnet-rid.sh returns TARGET so we can override for special cases like musl-c
RUN echo "Will use dotnet RID: $(./get-dotnet-rid.sh), for Docker platform: $TARGETPLATFORM"
RUN dotnet publish src/rescuttle -r "$(./get-dotnet-rid.sh)" -p:PublishSingleFile=true --self-contained true -c Release -o /out -p:Version=$VERSION

# Use scratch image for smallest possible image + our binary
FROM scratch
WORKDIR /app
ENV PATH="/app:$PATH"
COPY --from=build /out/rescuttle rescuttle

