using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Pet
{
    [Message(GameMessageOpcode.ServerPetCustomisationList)]
    public class ServerPetCustomisationList : IWritable
    {
        public NetworkBitArray UnlockedFlair { get; set; } = new(512, NetworkBitArray.BitOrder.MostSignificantBit);
        public List<PetCustomisation> PetCustomisations { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.WriteBytes(UnlockedFlair.GetBuffer());

            writer.Write(PetCustomisations.Count, 32u);
            foreach (var petCustomisation in PetCustomisations)
                petCustomisation.Write(writer);
        }
    }
}
