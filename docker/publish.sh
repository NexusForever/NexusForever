#!/bin/sh
set -eu
for project in AuthServer StsServer WorldServer API.Account API.Character Server.Character Server.ChatServer Server.Friendship Server.GroupServer; do
    dotnet publish "/src/Source/NexusForever.$project/NexusForever.$project.csproj" \
        -c Release -o "/out/$project" -p:UseAppHost=false
    for example in "/out/$project/"*.example.json; do
        cp "$example" "${example%.example.json}.json"
    done
done

# WorldServer loads these at runtime; publish doesn't include them.
for project in /src/Source/NexusForever.Script.*/*.csproj; do
    dotnet build "$project" -c Release -p:SolutionDir=/src/Source/ -o /tmp/scripts
done
cp /tmp/scripts/NexusForever.Script.*.dll /out/WorldServer/
dotnet publish /src/docker/Bootstrap/Bootstrap.csproj -c Release -o /out/Bootstrap -p:UseAppHost=false
dotnet publish /src/Source/NexusForever.MapGenerator/NexusForever.MapGenerator.csproj -c Release -o /out/MapGenerator -p:UseAppHost=false
