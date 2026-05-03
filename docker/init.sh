#!/usr/bin/env bash
# One-shot setup script: generates docker/config/*.json from the *.example.json
# files in Source/ (via jq), clones/updates the world database, and imports
# all world SQL files into the running MariaDB container.
#
# Usage:
#   ./docker/init.sh                    # skip files/steps already done
#   ./docker/init.sh --force            # overwrite configs, git pull, re-import SQL
#   ./docker/init.sh --no-import        # skip the SQL import step
#
# Customisable variables (override via environment):
#   DB_HOST        MariaDB hostname/IP       (default: 172.26.240.10)
#   DB_PORT        MariaDB port              (default: 3306)
#   DB_USER        MariaDB user              (default: nexusforever)
#   DB_PASS        MariaDB password          (default: nexusforever)
#   RABBIT_HOST    RabbitMQ hostname         (default: rabbitmq)
#   RABBIT_PORT    RabbitMQ AMQP port        (default: 5672)
#   RABBIT_USER    RabbitMQ user             (default: nexusforever)
#   RABBIT_PASS    RabbitMQ password         (default: nexusforever)
#   CHAR_API_HOST  Character API base URL    (default: http://0.0.0.0:4000)
#   REALM_ID       Realm ID                  (default: 1)
#   WORLD_DB_REPO  World database git URL    (default: NexusForever/NexusForever.WorldDatabase)

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOURCE_DIR="$(dirname "$SCRIPT_DIR")/Source"
CONFIG_DIR="$SCRIPT_DIR/config"

FORCE=false
NO_IMPORT=false
for arg in "$@"; do
  [[ "$arg" == "--force" ]]     && FORCE=true
  [[ "$arg" == "--no-import" ]] && NO_IMPORT=true
done

export DB_HOST="${DB_HOST:-172.26.240.10}"
export DB_PORT="${DB_PORT:-3306}"
export DB_USER="${DB_USER:-nexusforever}"
export DB_PASS="${DB_PASS:-nexusforever}"
export RABBIT_HOST="${RABBIT_HOST:-rabbitmq}"
export RABBIT_PORT="${RABBIT_PORT:-5672}"
export RABBIT_USER="${RABBIT_USER:-nexusforever}"
export RABBIT_PASS="${RABBIT_PASS:-nexusforever}"
export CHAR_API_HOST="${CHAR_API_HOST:-http://0.0.0.0:4000}"
export REALM_ID="${REALM_ID:-1}"

mkdir -p "$CONFIG_DIR"
# Ensure game-data subdirs exist as directories (remove any blocking placeholder files)
for _d in "$SCRIPT_DIR/config" "$SCRIPT_DIR/game-data" "$SCRIPT_DIR/game-data/cache" "$SCRIPT_DIR/game-data/map" "$SCRIPT_DIR/game-data/tbl"; do
  if [[ -e "$_d" ]] || [[ -L "$_d" ]]; then
    if ! [[ -d "$_d" ]]; then
      echo "  WARN: removing non-directory at $_d ($(ls -ld "$_d" 2>&1 | head -c 120))"
      rm -rf "$_d"
    fi
  fi
  mkdir -p "$_d"
done
unset _d

# write_from_example <source_example> <dest_json> [extra jq filter expressions...]
write_from_example() {
  local src="$1"
  local dst="$2"
  shift 2
  local extra=("$@")

  if [[ ! -f "$src" ]]; then
    echo "  MISSING example: $src" >&2
    return 1
  fi

  if [[ -f "$dst" && "$FORCE" == false ]]; then
    echo "  SKIP   $(basename "$dst") (already exists; use --force to overwrite)"
    return
  fi

  # Base jq filter applied to every file:
  #   DB host/port and credentials in connection strings
  #   RabbitMQ AMQP URL
  #   Character API URL
  local jq_filter='walk(
    if type == "string" then
      gsub("server=127\\.0\\.0\\.1;port=3306";
           "server=" + env.DB_HOST + ";port=" + env.DB_PORT) |
      gsub("user=nexusforever;password=nexusforever";
           "user=" + env.DB_USER + ";password=" + env.DB_PASS) |
      gsub("amqp://nexusforever:nexusforever@localhost:5672";
           "amqp://" + env.RABBIT_USER + ":" + env.RABBIT_PASS +
           "@" + env.RABBIT_HOST + ":" + env.RABBIT_PORT) |
      gsub("http://localhost:4000"; env.CHAR_API_HOST)
    else . end
  )'

  for expr in "${extra[@]+"${extra[@]}"}"; do
    jq_filter+=" | $expr"
  done

  jq "$jq_filter" "$src" > "$dst"
  echo "  WRITE  $(basename "$dst")"
}

echo "Generating docker/config files from Source example JSONs..."
echo "  DB_HOST=${DB_HOST}  RABBIT_HOST=${RABBIT_HOST}  CHAR_API_HOST=${CHAR_API_HOST}"
echo ""

write_from_example \
  "$SOURCE_DIR/NexusForever.AuthServer/AuthServer.example.json" \
  "$CONFIG_DIR/AuthServer.json"

write_from_example \
  "$SOURCE_DIR/NexusForever.StsServer/StsServer.example.json" \
  "$CONFIG_DIR/StsServer.json"

write_from_example \
  "$SOURCE_DIR/NexusForever.WorldServer/WorldServer.example.json" \
  "$CONFIG_DIR/WorldServer.json" \
  '.Network.Internal.InputQueue = "WorldServer_" + env.REALM_ID' \
  '.Realm.RealmId = (env.REALM_ID | tonumber)'

write_from_example \
  "$SOURCE_DIR/NexusForever.Server.ChatServer/ChatServer.example.json" \
  "$CONFIG_DIR/ChatServer.json"

write_from_example \
  "$SOURCE_DIR/NexusForever.Server.GroupServer/GroupServer.example.json" \
  "$CONFIG_DIR/GroupServer.json"

# CharacterAPI: also fix the bind URL (CHAR_API_HOST → 0.0.0.0:4000 for the listener)
write_from_example \
  "$SOURCE_DIR/NexusForever.API.Character/CharacterAPI.example.json" \
  "$CONFIG_DIR/CharacterAPI.json" \
  '.urls = "http://0.0.0.0:4000"' \
  '.Database.Character[0].RealmId = (env.REALM_ID | tonumber)'

echo ""
echo "Done. Config files are in $CONFIG_DIR"

# ---------------------------------------------------------------------------
# World database
# ---------------------------------------------------------------------------
WORLD_DB_DIR="$SCRIPT_DIR/world-database"
WORLD_DB_REPO="${WORLD_DB_REPO:-https://github.com/NexusForever/NexusForever.WorldDatabase.git}"
# Pin to a commit whose tables are all present in the EF migrations.
# Newer commits reference entity_property / entity_script / entity_template which
# don't have migrations yet. Override via WORLD_DB_REVISION=HEAD to use latest.
WORLD_DB_REVISION="${WORLD_DB_REVISION:-342cb2e}"

echo ""
if [[ -d "$WORLD_DB_DIR/.git" ]]; then
  if [[ "$FORCE" == true ]]; then
    echo "Updating world database (git pull)..."
    git -C "$WORLD_DB_DIR" fetch origin
    git -C "$WORLD_DB_DIR" checkout "$WORLD_DB_REVISION"
  else
    echo "SKIP   world-database (already cloned; use --force to update)"
  fi
else
  echo "Cloning world database from $WORLD_DB_REPO ..."
  git clone "$WORLD_DB_REPO" "$WORLD_DB_DIR"
  git -C "$WORLD_DB_DIR" checkout "$WORLD_DB_REVISION"
fi
echo "Done. World database is in $WORLD_DB_DIR (revision: $WORLD_DB_REVISION)"
