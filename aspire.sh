#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")/Source"
command -v dotnet >/dev/null || { echo "Install the .NET 10 SDK first." >&2; exit 1; }
command -v docker >/dev/null || { echo "Install Docker first." >&2; exit 1; }
docker info >/dev/null 2>&1 || { echo "Docker is unavailable. Start Docker and check access with docker info." >&2; exit 1; }

if [[ -z ${DOTNET_ROOT:-} ]]; then
    runtime_path=$(dotnet --list-runtimes | awk '$1 == "Microsoft.NETCore.App" { sub(/^.*\[/, ""); sub(/\].*$/, ""); print; exit }')
    [[ -n "$runtime_path" ]] || { echo "Install the .NET 10 SDK first." >&2; exit 1; }
    export DOTNET_ROOT="${runtime_path%/shared/Microsoft.NETCore.App}"
fi

dotnet tool restore
apphost=NexusForever.Aspire.AppHost/NexusForever.Aspire.AppHost.csproj
if [[ ${1:-} == --setup ]]; then
    exec dotnet run --project "$apphost" -- --setup
fi
if [[ ! -f NexusForever.Aspire.AppHost/appsettings.Local.json ]]; then
    dotnet run --project "$apphost" -- --setup
fi
exec dotnet aspire run "$@"
