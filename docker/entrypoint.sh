#!/bin/sh
set -eu
service=${1:?Specify a NexusForever service}
shift
if [ "$service" = Bootstrap ] && [ "${1:-}" = check-assets ] &&
    { [ -f /assets/map/.extracting ] || [ -f /assets/tbl/.extracting ]; }; then
    echo "Asset extraction is incomplete. Run ./bootstrap.sh --extract-assets." >&2
    exit 1
fi
case "$service" in
    Bootstrap) assembly=Bootstrap ;;
    AuthServer|StsServer|WorldServer|API.Account|API.Character|Server.Character|Server.ChatServer|Server.Friendship|Server.GroupServer)
        assembly=NexusForever.$service ;;
    *) echo "Unknown service: $service" >&2; exit 2 ;;
esac
cd "/app/$service"
if [ "$service" != Bootstrap ]; then
    dotnet /app/Bootstrap/Bootstrap.dll configure
fi
exec dotnet "$assembly.dll" "$@"
