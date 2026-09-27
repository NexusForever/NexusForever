Aspire runs the servers locally and uses Docker for MySQL 8.4, RabbitMQ, and optional phpMyAdmin.

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0), Docker, Git, and a WildStar 16042 client or compatible generated assets.

Start Docker. On Linux/macOS, run from the repository root:

```sh
dotnet dev-certs https --trust
./aspire.sh
```

On Windows, run from `Source`:

```powershell
dotnet tool restore
dotnet dev-certs https --trust
dotnet run --project NexusForever.Aspire.AppHost -- --setup
dotnet aspire run
```

For Visual Studio 2026, run the setup command above once, then set `NexusForever.Aspire.AppHost` as the startup project.

The script installs the pinned Aspire CLI and prompts for paths, account, realm, and ports. Use `./aspire.sh --setup` to reconfigure. Settings live under `NexusForever` in ignored `appsettings.Local.json`; passwords use .NET user secrets. AppHost command-line options override saved settings and environment variables.

Choose the client installation or `Patch` directory and an asset output directory. Setup generates maps and tables in a temporary directory, validates them, then replaces old files and removes languages absent from the client. Interrupted extraction retries on the next start. Later starts check file headers and the map inventory (`.maps.json`) for missing or truncated maps. Assets without an inventory need one regeneration with the client; include `.maps.json` when copying `map/` and `tbl/` elsewhere.

Setup downloads pinned world SQL if its directory is missing. Custom SQL must contain data changes only; MySQL schema changes cannot be rolled back. Relative paths start at the AppHost directory.

| Setting | Default |
| --- | --- |
| `RealmHost`, `RealmName` | `127.0.0.1`, `NexusForever` |
| `AuthPort`, `StsPort`, `WorldPort` | `23115`, `6600`, `24000` |
| `AccountRole` | `1` Player; also `2` GameMaster or `3` Administrator |
| `PhpMyAdmin` | `true`; set `false` to disable |

Use a reachable IPv4 `RealmHost` for remote clients. Stop conflicting servers or change the game ports. HTTP ports are automatic.

The startup link opens the dashboard at `https://localhost:17021`. It links to the world console, RabbitMQ management, and phpMyAdmin, all bound locally. RabbitMQ credentials are in its resource details. For certificate issues, run `dotnet aspire doctor` from `Source`.

Startup checks assets, migrates all seven databases, imports world SQL, and creates the initial account. Existing accounts are kept; unchanged SQL is skipped. Failed setup blocks the servers; check the `assets` or `database-migrations` logs. Aspire uses example configs when runtime files are absent; standalone servers require runtime configs. Environment variables override either file.

Stop with Ctrl+C. Use `./aspire.sh --detach` to run in the background and `dotnet aspire stop` from `Source` to stop it.

Data persists in `nexusforever-aspire-mysql84-data` and `nexusforever-aspire-rabbitmq-data`. Override names with `MySqlVolume` and `RabbitMqVolume`. Keep the Aspire secrets with these volumes. Existing Compose data is not imported.
