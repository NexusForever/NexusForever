# Run NexusForever with Docker

`./bootstrap.sh` builds the servers, starts MySQL and RabbitMQ, migrates the
seven databases, imports world data, and creates a game account. It also starts
phpMyAdmin, RabbitMQ Management, and the world server's web console.

## Setup

You need Docker with Compose v2.20 or newer and a WildStar build 16042 client
or compatible extracted assets. Use Linux containers on Docker Desktop. Builds
run inside Docker; you don't need .NET installed on the host.

Without a client path, the default directory layout is:

```text
NexusForeverDocker/
├── map/                   # *.nfmap, format 4
├── tbl/                   # *.tbl, including World.tbl
└── NexusForever/
    ├── bootstrap.sh
    ├── compose.yaml
    ├── .env               # created on first run
    └── data/
        ├── mysql/
        └── rabbitmq/
```

From the checkout, run:

```sh
./bootstrap.sh
```

On the first interactive run, bootstrap asks for the client folder, database
storage, map and table directories, and server IPv4 address. It saves these in
`.env`. Enter the client folder or its `Patch` subfolder to generate assets.
Leave the client path blank to use existing version 4 maps and tables.

To ask for paths again without starting containers:

```sh
./bootstrap.sh --configure-only
```

Use `--configure` to ask and then start, or edit `.env` directly for unattended
runs. Credentials and ports are also in `.env`. Set `REALM_HOST` to the IPv4
address your clients will connect to: `127.0.0.1` for the same computer, the server's LAN
address for local play, or its public address for internet play. Setup writes
this address and `WORLD_PORT` to realm 1 in `nexus_forever_auth.server`.

When adding a client path, the prompts suggest `assets/map` and `assets/tbl`
under your chosen data directory (`./data` by default). Otherwise, `MAP_PATH` and
`TBL_PATH` default to `../map` and `../tbl`. Relative paths start at the checkout;
absolute paths work too. Use forward slashes on Docker Desktop.

Bootstrap checks assets before startup. If they're missing or incompatible,
it uses `CLIENT_PATH` to extract tables and localisation files and generate maps
with this checkout's MapGenerator. In a terminal, it asks for paths if no client
is configured. Unattended runs need the paths in `.env` beforehand.

The client is mounted read-only, and generated files belong to your host user.
Servers mount assets read-only. SELinux mounts use shared labels for assets and
private labels for database files; the client directory isn't relabelled.
If extraction fails or is interrupted, bootstrap retries it on the next run.
Compatible assets are reused. To regenerate them explicitly without starting
the server:

```sh
./bootstrap.sh --extract-assets
```

Extraction stops this stack's servers before writing to the selected output
directories. Choose new directories to keep old assets. `--extract-assets`
leaves servers stopped; run `./bootstrap.sh` when ready to start them.

The script runs in Linux/macOS shells and WSL. On Windows, enable Docker Desktop's
WSL integration or use the Compose commands below. Images support x86-64 and
ARM64; other architectures may need emulation. The first build downloads .NET
images, NuGet packages, and world SQL. Allow time for asset extraction as well
as the build, imports, and cache generation.

## Logins and ports

| Interface | Address | Username | Password |
| --- | --- | --- | --- |
| phpMyAdmin | http://localhost:8080 | `root` | `nexusforever-root` |
| MySQL application account | `mysql:3306` inside Compose | `nexusforever` | `nexusforever` |
| RabbitMQ Management | http://localhost:15672 | `nexusforever` | `nexusforever` |
| World console | http://localhost:5000/console.html | No login | — |
| Game account | Your launcher/server address | `admin@example.com` | `nexusforever` |

These are development defaults from `.env`. The game account starts with the
`Administrator` role; set `GAME_ACCOUNT_ROLE` to `Player` or `GameMaster` before
first startup to change it. Setup leaves existing accounts alone. Create more
accounts from the world console with `account create email@example.com password Player`.

Admin interfaces bind to localhost. The world console has no authentication;
use an SSH tunnel for remote access:

```sh
ssh -L 8080:127.0.0.1:8080 -L 15672:127.0.0.1:15672 -L 5000:127.0.0.1:5000 user@server
```

Game TCP ports 23115, 6600, and 24000 are published. Open them in your firewall
and forward them on your router as needed. MySQL, AMQP, and the account/character
APIs stay inside the Compose network. For the client and launcher, follow the
[client connection guide](https://www.emulator.ws/installation/client-connection-guide)
using build 16042 and your `REALM_HOST` address.

## Changing passwords

After first startup, editing `.env` alone won't change stored MySQL or RabbitMQ
passwords. Change them in the web UIs and `.env`, then rerun `./bootstrap.sh`.
Services may lose access while you do this, so plan for downtime.

1. In phpMyAdmin, sign in as `root`, open **User accounts**, and change the
   password for `nexusforever@%`. Set the same `MYSQL_PASSWORD` in `.env`.
2. For root, change both `root@%` and `root@localhost`, then update
   `MYSQL_ROOT_PASSWORD`. Setup uses the former; the health check uses the latter.
3. In RabbitMQ Management, open **Admin → Users → nexusforever**, change the
   password, and update `RABBITMQ_PASSWORD`. Keep its administrator tag and `/`
   permissions; worker health checks use the management API.
4. Run `./bootstrap.sh`.

Single-quote `.env` passwords containing `$` or `#`, for example
`MYSQL_PASSWORD='a$long#password'`. The MySQL application username is fixed at
`nexusforever`. To change `RABBITMQ_USER`, first create that user in RabbitMQ
Management with administrator access and permissions on `/`.

## Backups

`DATA_PATH` defaults to `./data`. Its `mysql/` and `rabbitmq/` directories hold
the databases and broker state. Use a local filesystem with Linux permissions;
on Windows, keep it in WSL's Linux filesystem. Containers set directory ownership.

Stopping, removing, or rebuilding containers preserves this data, including
`docker compose down --volumes`. Deleting these directories or changing
`DATA_PATH` gives the stack a different database installation. `.env` and the
default `data/` directory are ignored by Git; keep custom data paths outside
tracked files.

For a filesystem backup, stop the stack and copy `.env` and the data directory
with ownership preserved:

```sh
docker compose down
sudo tar -czpf nexusforever-backup.tar.gz .env data
docker compose up -d --wait --wait-timeout 900
```

Adjust the paths if you changed `DATA_PATH`, and move the archive outside the
checkout. Restore with the stack stopped, preserving ownership and using
compatible MySQL/RabbitMQ versions. You can also export databases in phpMyAdmin
and broker definitions in RabbitMQ Management. Definitions don't include queued
messages.

Application logs and table caches are lost when app containers are recreated.
Caches rebuild on startup; read current logs with `docker compose logs`.

## Running and updating

```sh
docker compose ps -a
docker compose logs -f --tail=100 world
docker compose logs --tail=100 setup
docker compose restart world
docker compose down
./bootstrap.sh
```

Bootstrap builds and checks assets, starts infrastructure, stops the servers,
then runs database setup before restarting them. It waits for server listeners
and worker queue consumers. If it fails, check the logs, fix the cause, and rerun.

Setup applies the repository's EF migrations and imports world SQL in path order.
Each import and its SHA-256 record share a transaction. Unchanged files are
skipped; a failed import rolls back and stops setup. Changed dumps can replace
zone content, so back up before updating the source or `WORLD_DATABASE_REF`.
Schema and image downgrades aren't automated.

World data is pinned to `79d836576b9da8d7f7321ec3463d3673d496f090` (2024-10-19)
from [NexusForever.WorldDatabase](https://github.com/NexusForever/NexusForever.WorldDatabase).
Later revisions need `entity_property`, `entity_script`, and creature template
tables missing from this `game_rework` checkout. The pin excludes later content,
including the newer tutorial dump. Update it only with matching schema changes.

Image defaults are .NET `10.0`, MySQL `8.4`, RabbitMQ `4.3-management`, and
phpMyAdmin `5.2-apache`. To refresh them:

```sh
docker compose build --pull setup
docker compose pull mysql rabbitmq phpmyadmin
./bootstrap.sh
```

Use full image tags or digests in `.env` if you need fixed versions.

### Running Compose directly

Copy `.env.example` to `.env` and edit it, then run these commands in order,
checking that each succeeds. They also work in PowerShell with Docker Desktop
using Linux containers.

If you need to extract assets first, set `CLIENT_PATH`, `MAP_PATH`, and `TBL_PATH`
in `.env`, create the two output directories, then run:

```sh
docker compose build setup
docker compose stop auth sts world account-api character-api group chat friendship character
docker compose run --rm --no-deps asset-extract
```

On Linux, set `ASSET_UID` and `ASSET_GID` to your `id -u` and `id -g` values
if they differ from 1000. Bootstrap sets these automatically.

```sh
docker compose build setup
docker compose up --no-deps --force-recreate --exit-code-from asset-check asset-check
docker compose up -d --wait --wait-timeout 300 mysql rabbitmq phpmyadmin
docker compose stop auth sts world account-api character-api group chat friendship character
docker compose up --no-deps --force-recreate --exit-code-from setup setup
docker compose up -d --wait --wait-timeout 900
```

`docker compose up -d --build` can initialize a fresh installation. For updates,
use bootstrap or the sequence above so servers are stopped during migrations.

For a `veth` / `operation not supported` error, check that the running kernel
matches its installed modules; a reboot may be needed after a kernel update.
Other startup failures commonly come from missing or incompatible assets, port
conflicts, or `.env` passwords that don't match the stored database users.
