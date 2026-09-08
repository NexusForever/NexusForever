#!/bin/sh
set -eu
patch=/client
if [ -d "$patch/Patch" ]; then
    patch=$patch/Patch
fi
if [ ! -f "$patch/ClientData.index" ] || [ ! -f "$patch/ClientData.archive" ]; then
    echo "CLIENT_PATH must contain Patch/ClientData.index and ClientData.archive, or point to Patch itself." >&2
    exit 1
fi
touch /assets/map/.extracting /assets/tbl/.extracting
echo "Extracting tables and generating maps from $patch..."
dotnet /app/MapGenerator/NexusForever.MapGenerator.dll -i "$patch" -o /assets -e -g
dotnet /app/Bootstrap/Bootstrap.dll check-assets
rm /assets/map/.extracting /assets/tbl/.extracting
