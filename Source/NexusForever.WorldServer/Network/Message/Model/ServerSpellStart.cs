using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Network.Message.Model.Shared;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerSpellStart)]
    public class ServerSpellStart : IWritable
    {
        public uint CastingId { get; set; }
        public uint Spell4Id { get; set; }
        public uint RootSpell4Id { get; set; }
        public uint ParentSpell4Id { get; set; }
        public uint CasterId { get; set; }
        public ushort Unknown20 { get; set; }
        public uint PrimaryTargetId { get; set; }
        public Position FieldPosition { get; set; } = new Position();
        public float Yaw { get; set; }
        public bool UserInitiatedSpellCast  { get; set; }
        public bool UseCreatureOverrides { get; set; }

        public List<InitialPosition> InitialPositionData { get; set; } = new List<InitialPosition>();
        public List<TelegraphPosition> TelegraphPositionData { get; set; } = new List<TelegraphPosition>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Spell4Id, 18u);
            writer.Write(RootSpell4Id, 18u);
            writer.Write(ParentSpell4Id, 18u);
            writer.Write(CasterId);
            writer.Write(Unknown20);
            writer.Write(PrimaryTargetId);
            FieldPosition.Write(writer);
            writer.Write(Yaw);

            writer.Write(InitialPositionData.Count, 8u);
            InitialPositionData.ForEach(i => i.Write(writer));

            writer.Write(TelegraphPositionData.Count, 8u);
            TelegraphPositionData.ForEach(t => t.Write(writer));

            writer.Write(UserInitiatedSpellCast);
            writer.Write(UseCreatureOverrides);
        }
    }
}
