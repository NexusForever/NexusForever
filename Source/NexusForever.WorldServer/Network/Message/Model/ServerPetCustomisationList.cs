using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerPetCustomizationList)]
    public class ServerPetCustomisationList : IWritable
    {
        public NetworkBitArray UnlockedFlair { get; set; } = new NetworkBitArray(512);
        public List<PetCustomisation> PetCustomisations { get; set; } = new List<PetCustomisation>();

        public void Write(GamePacketWriter writer)
        {
            writer.WriteBytes(UnlockedFlair.GetBuffer());

            writer.Write(PetCustomisations.Count, 32u);
            foreach(var petCustomization in PetCustomisations)
                petCustomization.Write(writer);
        }
    }
}
