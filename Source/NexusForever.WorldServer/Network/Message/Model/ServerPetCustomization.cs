using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPetCustomization, MessageDirection.Server)]
    public class ServerPetCustomization : IWritable
    {
        public PetCustomization petCustomization { get; set; }

        public void Write(GamePacketWriter writer)
        {
            petCustomization.Write(writer);
        }
    }
}
