# syntax=docker/dockerfile:1

# Build steps follow the official NexusForever server guide (Linux / .NET 10 SDK):
# https://www.emulator.ws/installation/server-guide
# https://www.emulator.ws/installation/server-guide/server-building/linux.md
#
# Final stage: use aspnet for NexusForever.WorldServer and NexusForever.API.Character;
# use runtime for smaller images for Auth, STS, Chat, or Group servers.
ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT=NexusForever.AuthServer
WORKDIR /src/Source

COPY Source/ .

# Official guide: restore and build the solution from NexusForever.slnx in Release.
RUN dotnet restore NexusForever.slnx

RUN dotnet build NexusForever.slnx --configuration Release --no-restore --verbosity minimal

RUN dotnet publish "${PROJECT}/${PROJECT}.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-build \
    --verbosity minimal

FROM ${RUNTIME_IMAGE} AS final
ARG PROJECT=NexusForever.AuthServer

WORKDIR /app
COPY --from=build --chown=app:app /app/publish .

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV ASSEMBLY=${PROJECT}.dll

USER app

ENTRYPOINT ["sh", "-c", "exec dotnet /app/$ASSEMBLY"]
