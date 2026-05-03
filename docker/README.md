# docker/ — Support Files Reference

This folder contains scripts, bootstrap SQL, server configuration, and runtime game data consumed by the Docker Compose stack. Several subdirectories and files are **generated at runtime** by the scripts and are excluded from version control via `.gitignore`.

---

## Directory Layout

```
docker/
├── init.sh                  # One-shot setup script (config generation + world DB clone)
├── import.sh                # World SQL import script
├── config/                  # !! GENERATED — gitignored !!
│   ├── AuthServer.json
│   ├── CharacterAPI.json
│   ├── ChatServer.json
│   ├── GroupServer.json
│   ├── StsServer.json
│   └── WorldServer.json
├── game-data/               # !! GENERATED — gitignored !!
│   ├── tbl/                 # Extracted game tables (*.tbl, *.bin)
│   ├── map/                 # Extracted map files (*.nfmap)
│   └── cache/               # Runtime cache written by WorldServer / ChatServer
├── init-db/
│   └── 01-databases.sql     # MariaDB bootstrap SQL (run once on container first start)
├── migrate/
│   └── migrate.sh           # EF Core migration runner
└── world-database/          # !! GENERATED — gitignored !!
                             # Cloned NexusForever.WorldDatabase SQL repo
```

---

## Scripts

### `init.sh`

**Run by:** the `init` Compose service (one-shot, exits after completion).  
**Can also be run directly** on the host for local (non-Docker) setups.

**What it does:**

1. **Config generation** — reads each `*.example.json` from the `Source/` projects and writes a corresponding `docker/config/*.json` using `jq`. Substitutions applied to every file:
   - MariaDB host/port and credentials in connection strings
   - RabbitMQ AMQP URL
   - Character API base URL

   Extra per-file transforms:
   - `WorldServer.json` — sets `Network.Internal.InputQueue` and `Realm.RealmId` from `$REALM_ID`
   - `CharacterAPI.json` — sets `.urls` to `http://0.0.0.0:4000` so Kestrel binds on all interfaces, and sets `Database.Character[0].RealmId`

2. **World database** — clones [NexusForever.WorldDatabase](https://github.com/NexusForever/NexusForever.WorldDatabase) into `docker/world-database/` (if not already present) and checks out the pinned revision defined by `$WORLD_DB_REVISION` (default: `342cb2e`). This pin targets the last commit whose tables are fully covered by the EF migrations. Set `WORLD_DB_REVISION=HEAD` in `.env` to use the latest commit.

**Flags:**

| Flag | Effect |
|---|---|
| `--force` | Overwrite existing config files and `git pull` the world DB |
| `--no-import` | (legacy — import is now a separate service) |

**Environment variables (all have defaults):**

| Variable | Default | Description |
|---|---|---|
| `DB_HOST` | `172.26.240.10` | MariaDB host |
| `DB_PORT` | `3306` | MariaDB port |
| `DB_USER` | `nexusforever` | MariaDB user |
| `DB_PASS` | `nexusforever` | MariaDB password |
| `RABBIT_HOST` | `rabbitmq` | RabbitMQ hostname |
| `RABBIT_PORT` | `5672` | RabbitMQ AMQP port |
| `RABBIT_USER` | `nexusforever` | RabbitMQ user |
| `RABBIT_PASS` | `nexusforever` | RabbitMQ password |
| `CHAR_API_HOST` | `http://0.0.0.0:4000` | Character API base URL written into configs |
| `REALM_ID` | `1` | Realm ID written into WorldServer and CharacterAPI configs |
| `WORLD_DB_REPO` | `https://github.com/NexusForever/NexusForever.WorldDatabase.git` | World database git remote |
| `WORLD_DB_REVISION` | `342cb2e` | Git revision to check out after clone |

---

### `import.sh`

**Run by:** the `world-import` Compose service (one-shot, exits after completion).

**What it does:**

Finds all `*.sql` files under `docker/world-database/` (sorted, `.git/` excluded) and imports each one into the `nexus_forever_world` database using `mariadb --force`. Non-blank output lines from MariaDB (warnings or errors) are printed to stderr alongside a per-file warning. A final summary reports how many files had errors.

**Environment variables:**

| Variable | Default | Description |
|---|---|---|
| `DB_HOST` | (required) | MariaDB host |
| `DB_PORT` | (required) | MariaDB port |
| `DB_USER` | (required) | MariaDB user |
| `DB_PASS` | (required) | MariaDB password |
| `NO_IMPORT` | `false` | Set to `true` to skip the import entirely |

---

### `migrate/migrate.sh`

**Run by:** the `migrate` Compose service using the official `mcr.microsoft.com/dotnet/sdk:10.0` image.

**What it does:**

1. Installs (or updates) `dotnet-ef` as a global tool.
2. Restores the `NexusForever.slnx` solution.
3. Runs `dotnet ef database update` for every EF Core `DbContext`:
   - `AuthContext` — `nexus_forever_auth`
   - `CharacterContext` — `nexus_forever_character`
   - `WorldContext` — `nexus_forever_world`
   - ChatServer context — `nexus_forever_chat`
   - GroupServer context — `nexus_forever_group`

EF is run from `/migrate-config` (which maps to `docker/config/`) so that design-time factories resolve `AddJsonFile("<Name>.json")` against the correct directory without modifying the source tree.

> **Note:** The upstream guide references `NexusForever.Server.WorldServer`; this repository uses `NexusForever.WorldServer`.

---

## `init-db/01-databases.sql`

**Run by:** MariaDB's `docker-entrypoint-initdb.d` mechanism — executed **once** the first time the `mariadb_data` volume is created.

Creates all five databases and the `nexusforever` MariaDB user with full privileges:

```sql
CREATE DATABASE IF NOT EXISTS nexus_forever_auth;
CREATE DATABASE IF NOT EXISTS nexus_forever_character;
CREATE DATABASE IF NOT EXISTS nexus_forever_world;
CREATE DATABASE IF NOT EXISTS nexus_forever_chat;
CREATE DATABASE IF NOT EXISTS nexus_forever_group;

CREATE USER IF NOT EXISTS 'nexusforever'@'%' IDENTIFIED BY 'nexusforever';
GRANT ALL PRIVILEGES ON *.* TO 'nexusforever'@'%';
```

To reset the databases, remove the `mariadb_data` Docker volume:
```bash
docker compose down -v
```

---

## Generated / Gitignored Paths

The following paths do not exist in the repository. They are created by the setup scripts during the first run.

### `config/` — Server configuration files

Generated by `init.sh` from the `*.example.json` files in each `Source/` project. Mount-injected into running containers as read-only volumes.

| File | Server |
|---|---|
| `AuthServer.json` | `NexusForever.AuthServer` |
| `StsServer.json` | `NexusForever.StsServer` |
| `WorldServer.json` | `NexusForever.WorldServer` |
| `CharacterAPI.json` | `NexusForever.API.Character` |
| `ChatServer.json` | `NexusForever.Server.ChatServer` |
| `GroupServer.json` | `NexusForever.Server.GroupServer` |

To regenerate (e.g. after changing credentials or realm ID), re-run the `init` service:
```bash
docker compose run --rm init --force
```

---

### `game-data/` — Extracted game assets

Populated by the `extractor` Compose service (profile `tools`) or manually.

#### `game-data/tbl/`

Contains game table files extracted from the WildStar 16042 client:
- `*.tbl` — binary game tables
- `*.bin` — supplemental binary data

Required by: `WorldServer`, `ChatServer`.

#### `game-data/map/`

Contains pre-generated map data files:
- `*.nfmap` — NexusForever map format generated by `NexusForever.MapGenerator`

Required by: `WorldServer`.

#### `game-data/cache/`

Runtime write cache created and managed by `WorldServer` and `ChatServer`. Created automatically as an empty directory by `init.sh`. Persists across container restarts via a bind-mount.

**To populate `tbl/` and `map/`**, run the extractor with the WildStar client path:
```bash
WILDSTAR_DIR=/path/to/WildStar docker compose --profile tools run --rm extractor
```

---

### `world-database/`

A clone of the [NexusForever.WorldDatabase](https://github.com/NexusForever/NexusForever.WorldDatabase) repository, checked out at the pinned revision specified by `WORLD_DB_REVISION` in the Compose environment (default: `342cb2e`).

Populated by `init.sh`. Contains the SQL files that `import.sh` imports into `nexus_forever_world`.

Structure mirrors the upstream repository layout:
```
world-database/
├── Alizar/
├── Instance/
├── Isigrol/
├── Olyssia/
└── ...
```

To update to a newer revision, set `WORLD_DB_REVISION=HEAD` (or a specific commit hash) in `.env` and re-run:
```bash
docker compose run --rm init --force
docker compose run --rm world-import
```
