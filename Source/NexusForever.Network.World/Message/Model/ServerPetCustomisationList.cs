using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerPetCustomizationList)]
    public class ServerPetCustomisationList : IWritable
    {
        public NetworkBitArray UnlockedFlair { get; set; } = new(512, NetworkBitArray.BitOrder.MostSignificantBit);
        public List<PetCustomisation> PetCustomisations { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.WriteBytes(UnlockedFlair.GetBuffer());

            writer.Write(PetCustomisations.Count, 32u);
            foreach(var petCustomization in PetCustomisations)
                petCustomization.Write(writer);
        }
    }
}
