# syntax=docker/dockerfile:1
ARG DOTNET_VERSION=10.0
FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build
WORKDIR /src
COPY Source/ Source/
COPY docker/Bootstrap/ docker/Bootstrap/
COPY docker/publish.sh /usr/local/bin/publish
RUN --mount=type=cache,target=/root/.nuget/packages sh /usr/local/bin/publish

FROM build AS world-data
ARG WORLD_DATABASE_REF=79d836576b9da8d7f7321ec3463d3673d496f090
RUN git init /world-database \
    && git -C /world-database remote add origin https://github.com/NexusForever/NexusForever.WorldDatabase.git \
    && git -C /world-database fetch --depth 1 origin "$WORLD_DATABASE_REF" \
    && git -C /world-database checkout --detach FETCH_HEAD \
    && git -C /world-database rev-parse HEAD > /world-database/REVISION \
    && rm -rf /world-database/.git

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION} AS runtime
# hadolint ignore=DL3008
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build --chown=app:app /out/ /app/
COPY --from=world-data /world-database/ /world-database/
COPY --chmod=755 docker/entrypoint.sh /usr/local/bin/nexusforever
COPY --chmod=755 docker/extract-assets.sh /usr/local/bin/extract-assets
# hadolint ignore=DL3066
USER $APP_UID
ENTRYPOINT ["nexusforever"]
