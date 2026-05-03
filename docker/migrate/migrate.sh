#!/usr/bin/env bash
# Runs EF migrations against MariaDB (first-time setup). Matches:
# https://www.emulator.ws/installation/server-guide/server-installation/server-install-guide-linux.md
#
# Upstream doc paths say NexusForever.Server.WorldServer — this repository uses NexusForever.WorldServer.
#
# EF is run from /migrate-config (mounted from docker/config) so that design-time factories
# resolve AddJsonFile("<Name>.json") against that directory — no files are copied into the source tree.
set -euo pipefail

export PATH="${PATH}:/root/.dotnet/tools"
dotnet tool install dotnet-ef --global --verbosity quiet || dotnet tool update dotnet-ef --global --verbosity quiet

ROOT="/src/Source"

dotnet restore "${ROOT}/NexusForever.slnx"

# Run EF from /migrate-config so that design-time factories find the JSON files
# via AddJsonFile("<Name>.json") resolving against the current working directory.
# No files are copied into the source tree.
cd /migrate-config

dotnet ef database update --context AuthContext      --project "${ROOT}/NexusForever.WorldServer"
dotnet ef database update --context CharacterContext --project "${ROOT}/NexusForever.WorldServer"
dotnet ef database update --context WorldContext     --project "${ROOT}/NexusForever.WorldServer"

dotnet ef database update --project "${ROOT}/NexusForever.Server.ChatServer"

dotnet ef database update --project "${ROOT}/NexusForever.Server.GroupServer"

echo "Database migrations finished."
