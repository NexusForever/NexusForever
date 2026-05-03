# Docker Overview

This document describes all Dockerfiles, the Compose file, and the `docker/` support folder.

---

## Dockerfiles

### `Dockerfile`

**Purpose:** Generic multi-project server image builder. Used for every game server service in Compose.

Accepts two build arguments:

| Argument | Default | Description |
|---|---|---|
| `PROJECT` | `NexusForever.AuthServer` | The `Source/` project to publish |
| `RUNTIME_IMAGE` | `mcr.microsoft.com/dotnet/aspnet:10.0` | Base runtime image for the final stage |

**Build stages:**
1. `build` — restores and builds the full `NexusForever.slnx` solution in Release, then publishes the specified `PROJECT` to `/app/publish`.
2. `final` — copies the published output into the chosen `RUNTIME_IMAGE`. The entry-point dynamically resolves `$ASSEMBLY` so any project name works.

**Used by Compose services:** `auth`, `sts`, `character-api`, `world`, `chat`, `group`.

Use `RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0` for projects that expose HTTP (WorldServer, CharacterAPI) and `mcr.microsoft.com/dotnet/runtime:10.0` for all others to keep images smaller.

---

### `Dockerfile.Artifacts`

**Purpose:** Builds all five server release artifacts in a single image and optionally exports them to the host.

**Artifacts produced:**

| Folder | Target RID | Deployment |
|---|---|---|
| `AuthServer/` | `linux-x64` | Framework-dependent |
| `StsServer/` | `linux-x64` | Framework-dependent |
| `WorldServer/` | `linux-x64` | Framework-dependent |
| `MapGenerator/` | `linux-x64` | Framework-dependent |
| `ClientConnector/` | `win-x64` | Framework-dependent |

**Build stages:**
1. `build` — restores the solution and publishes each project separately.
2. `final` — Alpine image with all artifacts under `/artifacts/`. Used by the `build-artifacts` Compose service, which copies them to `./dist/` via a bind-mount.
3. `export` — scratch target for standalone use; `--output` writes folders directly to the host without a running container.

**Usage:**
```bash
# Via Compose (copies to ./dist/)
docker compose --profile tools run --rm build-artifacts

# Standalone (direct export)
docker build -f Dockerfile.Artifacts --target export --output ./dist/artifacts .
```

---

### `Dockerfile.Launcher`

**Purpose:** Cross-compiles the [NexusForever.Launcher](https://github.com/NexusForever/NexusForever.Launcher) WPF application into a self-contained `win-x64` single-file executable from a Linux SDK container (no Windows Docker daemon required).

**Build stages:**
1. `build` — installs `git`, clones the Launcher repository at `HEAD --depth 1`, and publishes a self-contained single-file `win-x64` binary to `/artifacts/`.
2. `final` — copies `/artifacts/` so the Compose `launcher` service can copy them to the host via a bind-mount volume.

**Usage:**
```bash
# Via Compose (copies to ./dist/Launcher/)
docker compose --profile tools run --rm launcher

# Standalone
docker build -f Dockerfile.Launcher --output ./dist/Launcher .
```

The output is the player-side launcher — it runs on the player's Windows machine and connects to a live NexusForever server.

---

## `docker-compose.yml`

The Compose file orchestrates the full NexusForever server stack. It follows the sequence from the [official server guide](https://www.emulator.ws/installation/server-guide).

**Network:** All services share a bridge network `nf_net` (`172.26.240.0/24`). MariaDB has a fixed IP (`172.26.240.10`) to avoid Docker DNS and IPv6 resolution issues.

**Optional `.env` file** (copy from `.env.example`): Overrides port bindings, credentials, and the `WILDSTAR_DIR` path used by extraction tooling.

### Service groups

#### Infrastructure (always up)

| Service | Image | Purpose |
|---|---|---|
| `mariadb` | `mariadb:11.4` | Primary database. Bootstrapped by `docker/init-db/01-databases.sql` on first run. Data persisted in `mariadb_data` named volume. |
| `rabbitmq` | `rabbitmq:3.13-management` | Message broker between WorldServer, ChatServer, and GroupServer. Management UI on port `15672`. |

#### One-shot setup (run once, then exit)

| Service | Depends on | Purpose |
|---|---|---|
| `init` | `mariadb` healthy | Generates `docker/config/*.json` from example files and clones/pins the world database repo. Runs `docker/init.sh`. |
| `migrate` | `init` complete | Runs EF Core database migrations via `docker/migrate/migrate.sh` using the .NET SDK image. |
| `world-import` | `migrate` complete | Imports world database SQL files into `nexus_forever_world` via `docker/import.sh`. |
| `check-assets` | `world-import` complete | Verifies that `docker/game-data/tbl/` and `docker/game-data/map/` contain extracted game files before allowing game servers to start. |

#### Game servers (default profile)

| Service | Port(s) | Purpose |
|---|---|---|
| `character-api` | `4000` | HTTP REST API for character data (`NexusForever.API.Character`). |
| `auth` | `23115` | Authentication server. Also runs an internal `socat` relay on `24000` → `world:24000` for client compatibility. |
| `sts` | `6600` | STS (Secure Token Service) server. |
| `group` | — | Group server; communicates via RabbitMQ. |

#### Game servers (`--profile game`)

| Service | Port(s) | Purpose |
|---|---|---|
| `world` | `24000` (game), `5000` (HTTP) | World game server. Requires extracted `tbl/` and `map/` assets. HTTP port is used for the web account-management console. |
| `chat` | — | Chat server. Requires extracted `tbl/` assets. |

Start the game-profile services with:
```bash
docker compose --profile game up -d
```

#### Tools (`--profile tools`)

| Service | Purpose |
|---|---|
| `extractor` | Runs `NexusForever.MapGenerator` against a WildStar 16042 `Patch` directory to extract `.tbl`/`.bin` game tables and `.nfmap` map files into `docker/game-data/`. Requires `WILDSTAR_DIR` to be set. |
| `build-artifacts` | Builds all five server artifacts via `Dockerfile.Artifacts` and copies them to `./dist/`. |
| `launcher` | Builds the player-side launcher via `Dockerfile.Launcher` and copies it to `./dist/Launcher/`. |

---

## `docker/` Folder

Supporting files consumed by Compose services and setup scripts. See [docker/README.md](docker/README.md) for full details.

| Path | Description |
|---|---|
| `init.sh` | One-shot setup: generates configs, clones world DB |
| `import.sh` | Imports world SQL files into MariaDB |
| `migrate/migrate.sh` | Runs EF Core migrations |
| `init-db/01-databases.sql` | MariaDB bootstrap SQL (databases + user) |
| `config/` | Generated server JSON configs (gitignored) |
| `game-data/` | Extracted game assets — `tbl/`, `map/`, `cache/` (gitignored) |
| `world-database/` | Cloned world database SQL repo (gitignored) |
