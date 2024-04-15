## NexusForever
[![Discord](https://img.shields.io/discord/499473932131500034.svg?style=flat&logo=discord)](https://discord.gg/8wT3GEQ)

### Information
A server emulator for WildStar written in C# that supports build 16042.

### Getting Started
[Server Setup Guide](https://www.emulator.ws/installation/server-install-guide-windows)

### Requirements
 * Visual Studio 2022 (.NET 8 and C# 12 support required)
 * MySQL Server (or equivalent, eg: MariaDB)
 * WildStar 16042 client

### Branches
NexusForever has multiple branches:
* **[Master](https://github.com/NexusForever/NexusForever/tree/master)**  
Latest stable release, develop is merged into master once enough content has accumulated in develop.  
Compiled binary releases are based on this branch.
* **[Game Rework](https://github.com/NexusForever/NexusForever/tree/game_rework)**  
Current active development branch, major refactors and updates to the project are underway in this branch.  
All PR's should be targeted to this branch.  
This branch will eventually be merged back into develop.  
* **[Develop](https://github.com/NexusForever/NexusForever/tree/develop)**  
~~Active development branch with the latest features but may be unstable.  
Any new pull requests must be targed towards this branch.~~

### Links
 * [Website](https://emulator.ws)
 * [Discord](https://discord.gg/8wT3GEQ)
 * [World Database](https://github.com/NexusForever/NexusForever.WorldDatabase)

## Build Status
### Windows
Automated builds that will run on Windows or Windows Server.

Master:  
![Master](https://dev.azure.com/NexusForever/NexusForever/_apis/build/status/NexusForever%20Master%20Windows)  
Game Rework:  
![Game Rework](https://dev.azure.com/NexusForever/NexusForever/_apis/build/status/NexusForever%20Develop%20Windows?branchName=game_rework)  
Development:  
![Development](https://dev.azure.com/NexusForever/NexusForever/_apis/build/status/NexusForever%20Develop%20Windows?branchName=develop)
### Linux
Automated builds that will run on various Linux distributions.  
See the [.NET runtime identifer documentation](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#linux-rids)  for more information on exact distributions.

Master:  
![Master](https://dev.azure.com/NexusForever/NexusForever/_apis/build/status/NexusForever%20Master%20Linux)  
Game Rework:  
![Game Rework](https://dev.azure.com/NexusForever/NexusForever/_apis/build/status/NexusForever%20Develop%20Linux?branchName=game_rework)  
Development:  
![Development](https://dev.azure.com/NexusForever/NexusForever/_apis/build/status/NexusForever%20Develop%20Linux?branchName=develop)
