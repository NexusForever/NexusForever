using NexusForever.Database.World.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace NexusForever.WorldServer.Game.ServerConfig
{
    public class ServerConfig
    {
        public uint Id { get; set; }
        public byte Active { get; set; }
        public ImmutableList<CharacterCreationLocation> CharacterCreationLocations { get; }

        public ServerConfig(ServerConfigModel model)
        {
            Id = model.Id;
            Active = model.Active;
            var creationLocationsBuilder = ImmutableList.CreateBuilder<CharacterCreationLocation>();
            foreach (var creationModel in model.CharacterCreationLocations)
            {
                creationLocationsBuilder.Add(new CharacterCreationLocation(creationModel));
            }
            CharacterCreationLocations = creationLocationsBuilder.ToImmutable();
        }
    }
}
