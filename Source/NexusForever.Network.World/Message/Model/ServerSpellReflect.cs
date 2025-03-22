using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellReflect)]
    public class ServerSpellReflect : IWritable
    {
        public uint SpellUniqueId { get; set; }
        public uint Spell4EffectId { get; set; } = 0;
        public uint CasterUnitId { get; set; } = 0;
        public uint TargetUnitId { get; set; } = 0;

        public SpellEffectResult SpellEffectResult { get; set; } = new();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(SpellUniqueId);
            writer.Write(Spell4EffectId, 19);
            writer.Write(CasterUnitId);
            writer.Write(TargetUnitId);
            SpellEffectResult.Write(writer);
        }
    }
}
