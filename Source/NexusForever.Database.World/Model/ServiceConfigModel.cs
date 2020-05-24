using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.Database.World.Model
{
    public class ServerConfigModel
    {
        public uint Id { get; set; }
        public byte Active { get; set; }
        public ICollection<ServerConfigCharacterCreationLocationModel> CharacterCreationLocations { get; set; } = new HashSet<ServerConfigCharacterCreationLocationModel>();
    }
}
