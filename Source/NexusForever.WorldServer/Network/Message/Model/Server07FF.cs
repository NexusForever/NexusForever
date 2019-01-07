using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.Server07FF, MessageDirection.Server)]
    public class Server07FF : IWritable
    {
        public class InitialPosition : IWritable
        {   
            public uint   CasterId { get; set; } = 0;
            public byte   Unknown4 { get; set; } = 0;
            public Position Position { get; set; } = new Position();
            public float  Yaw { get; set; } = 0;
            public uint   Unknown21 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(CasterId);
                writer.Write(Unknown4);
                Position.Write(writer);
                writer.Write(Yaw);
                writer.Write(Unknown21);
            }
        }

        public class TelegraphPosition : IWritable
        {   
            public ushort Unknown0 { get; set; } = 0;
            public uint   CasterId { get; set; } = 0;
            public byte   Unknown6 { get; set; } = 0;
            public Position Position { get; set; } = new Position();
            public float  Yaw { get; set; } = 0;
            public uint   Unknown23 { get; set; } = 0;

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Unknown0);
                writer.Write(CasterId);
                writer.Write(Unknown6);
                Position.Write(writer);
                writer.Write(Yaw);
                writer.Write(Unknown23);
            }
        }

        public uint CastingId { get; set; }
        public uint Spell4Id { get; set; }
        public uint RootSpell4Id { get; set; }
        public uint ParentSpell4Id { get; set; } = 0;
        public uint CasterId { get; set; }
        public ushort Unknown20 { get; set; } = 0;
        public uint Guid2 { get; set; } // target?
        public Position FieldPosition { get; set; } = new Position();
        public float Yaw { get; set; } = 0;
        public bool UserInitiatedSpellCast  { get; set; } = false;
        public bool UseCreatureOverrides { get; set; } = false;

        public List<InitialPosition> initialPosition { get; set; } = new List<InitialPosition>();
        public List<TelegraphPosition> telegraphPosition { get; set; } = new List<TelegraphPosition>();

        public void Write(GamePacketWriter writer)
        {
            writer.Write(CastingId);
            writer.Write(Spell4Id, 18u);
            writer.Write(RootSpell4Id, 18u);
            writer.Write(ParentSpell4Id, 18u);
            writer.Write(CasterId);
            writer.Write(Unknown20);
            writer.Write(Guid2);
            FieldPosition.Write(writer);
            writer.Write(Yaw);

            writer.Write(initialPosition.Count, 8u);
            initialPosition.ForEach(i => i.Write(writer));

            writer.Write(telegraphPosition.Count, 8u);
            telegraphPosition.ForEach(t => t.Write(writer));

            writer.Write(UserInitiatedSpellCast);
            writer.Write(UseCreatureOverrides);
        }
    }
}
