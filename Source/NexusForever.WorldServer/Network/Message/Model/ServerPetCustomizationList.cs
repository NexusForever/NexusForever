using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPetCustomizationList, MessageDirection.Server)]
    public class ServerPetCustomizationList : IWritable
    {
        public List<ushort> PetFlairUnlockBits { get; set; } = new List<ushort>();
        public List<PetCustomization> PetCustomizations { get; set; } = new List<PetCustomization>();

        public void Write(GamePacketWriter writer)
        {
            writer.BitBang(PetFlairUnlockBits.ToArray(), 64u);

            writer.Write(PetCustomizations.Count, 32u);

            foreach (var petCustomization in PetCustomizations)
                petCustomization.Write(writer);
        }
    }
}