using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Pet;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pet
{
    [Message(GameMessageOpcode.ServerPetCustomisationFailed)]
    public class ServerPetCustomisationFailed : IWritable
    {
        public PetCustomizeResult Reason { get; set; }
        public PetType Type { get; set; }
        public uint PetUnitId { get; set; }
        public ushort FlairSlotIndex { get; set; }
        public ushort PetFlairId { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Reason, 4u);
            writer.Write(Type, 2u);
            writer.Write(PetUnitId, 32u);
            writer.Write(FlairSlotIndex);
            writer.Write(PetFlairId, 14u);
        }
    }
}
