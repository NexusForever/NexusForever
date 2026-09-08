#!/bin/sh
set -eu
cd "$(CDPATH='' cd -- "$(dirname -- "$0")" && pwd)"

case "${1:-}" in
    --help|-h)
        echo "Usage: ./bootstrap.sh [--configure | --configure-only | --extract-assets]"
        echo "Builds, prepares assets and databases, and starts the server."
        echo "--configure       Ask for client, asset, data and realm paths/settings again."
        echo "--configure-only  Save settings without building or starting containers."
        echo "--extract-assets  Regenerate maps and tables, then exit."
        exit 0 ;;
    ""|--configure|--configure-only|--extract-assets) ;;
    *) echo "Unknown option: $1 (see --help)" >&2; exit 2 ;;
esac

if ! command -v docker >/dev/null 2>&1 || ! docker compose version >/dev/null 2>&1; then
    echo "Install Docker with Compose v2.20+ (Linux containers), then rerun this script." >&2
    exit 1
fi

setting() {
    docker compose config --environment | sed -n "s/^$1=//p"
}

save_setting() {
    config_tmp=$(mktemp .env.XXXXXX)
    CONFIG_KEY=$1 CONFIG_VALUE=$2 awk '
        BEGIN {
            key = ENVIRON["CONFIG_KEY"]
            value = ENVIRON["CONFIG_VALUE"]
            gsub(/\047/, "\\\047", value)
            value = "\047" value "\047"
        }
        $0 ~ "^" key "=" { print key "=" value; found = 1; next }
        { print }
        END { if (!found) print key "=" value }
    ' .env > "$config_tmp"
    mv "$config_tmp" .env
    unset "$1"
}

prompt() {
    printf '%s [%s]: ' "$2" "$3"
    IFS= read -r answer || { echo "Input ended; rerun --configure in a terminal." >&2; exit 1; }
    save_setting "$1" "${answer:-$3}"
}

configure() {
    old_client=$(setting CLIENT_PATH)
    prompt CLIENT_PATH 'WildStar client or Patch folder (optional; - to use existing assets)' "$old_client"
    if [ "$(setting CLIENT_PATH)" = - ]; then
        save_setting CLIENT_PATH ''
    fi
    prompt DATA_PATH 'Database storage directory' "$(setting DATA_PATH)"
    map_default=$(setting MAP_PATH)
    tbl_default=$(setting TBL_PATH)
    if [ -z "$old_client" ] && [ -n "$(setting CLIENT_PATH)" ]; then
        map_default=$(setting DATA_PATH)/assets/map
        tbl_default=$(setting DATA_PATH)/assets/tbl
    fi
    prompt MAP_PATH 'Map directory' "$map_default"
    prompt TBL_PATH 'Table directory' "$tbl_default"
    prompt REALM_HOST 'Server IPv4 address clients will connect to' "$(setting REALM_HOST)"
}

new_config=false
if [ ! -f .env ]; then
    (umask 077; cp .env.example .env)
    echo "Created .env with local development defaults."
    new_config=true
fi
if [ "${1:-}" = --configure ] || [ "${1:-}" = --configure-only ] || { "$new_config" && [ -t 0 ]; }; then
    configure
fi
docker compose config --quiet
if [ "${1:-}" = --configure-only ]; then
    echo "Configuration saved. Edit .env for credentials and ports, then run ./bootstrap.sh."
    exit 0
fi
if ! docker info >/dev/null 2>&1; then
    echo "Cannot access Docker. Start Docker and ensure your user can access its daemon." >&2
    exit 1
fi

extract_assets() {
    client_path=$(setting CLIENT_PATH)
    if [ -z "$client_path" ]; then
        if [ ! -t 0 ]; then
            echo "Set CLIENT_PATH, MAP_PATH and TBL_PATH in .env, or run ./bootstrap.sh --configure." >&2
            exit 1
        fi
        configure
        client_path=$(setting CLIENT_PATH)
    fi
    if [ -z "$client_path" ]; then
        echo "Compatible maps and tables are required. Set CLIENT_PATH to generate them." >&2
        exit 1
    fi
    patch_path=$client_path
    if [ -d "$patch_path/Patch" ]; then
        patch_path=$patch_path/Patch
    fi
    if [ ! -f "$patch_path/ClientData.index" ] || [ ! -f "$patch_path/ClientData.archive" ]; then
        echo "No client archives found in $patch_path. Check CLIENT_PATH in .env." >&2
        exit 1
    fi
    map_path=$(setting MAP_PATH)
    tbl_path=$(setting TBL_PATH)
    mkdir -p -- "$map_path" "$tbl_path"
    ASSET_UID=$(id -u)
    ASSET_GID=$(id -g)
    export ASSET_UID ASSET_GID
    docker compose stop auth sts world account-api character-api group chat friendship character
    docker compose run --rm --no-deps asset-extract
}

docker compose build setup
if [ "${1:-}" = --extract-assets ]; then
    extract_assets
    docker compose up --no-deps --force-recreate --exit-code-from asset-check asset-check
    echo "Assets ready. Run ./bootstrap.sh to start the server."
    exit 0
fi
if [ -f "$(setting MAP_PATH)/.extracting" ] || [ -f "$(setting TBL_PATH)/.extracting" ] ||
    ! docker compose up --no-deps --force-recreate --exit-code-from asset-check asset-check; then
    extract_assets
    docker compose up --no-deps --force-recreate --exit-code-from asset-check asset-check
fi
docker compose up -d --wait --wait-timeout 300 mysql rabbitmq phpmyadmin
# Stop servers before updating the databases.
docker compose stop auth sts world account-api character-api group chat friendship character
docker compose up --no-deps --force-recreate --exit-code-from setup setup
if ! docker compose up -d --wait --wait-timeout 900; then
    docker compose ps -a
    echo "Startup failed. Inspect: docker compose logs --tail=100 setup world" >&2
    exit 1
fi
docker compose ps
cat <<'EOF'

NexusForever is running. Default local addresses:
  World console: http://localhost:5000/console.html
  phpMyAdmin:    http://localhost:8080  (root / nexusforever-root)
  RabbitMQ:      http://localhost:15672  (nexusforever / nexusforever)
  Game account:  admin@example.com / nexusforever
If you changed .env, use your configured ports and credentials instead.
See docker/README.md for client connection, password changes and backups.
EOF
