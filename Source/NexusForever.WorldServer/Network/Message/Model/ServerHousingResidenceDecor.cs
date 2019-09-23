using System.Collections.Generic;
using System.Numerics;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Housing.Static;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerHousingResidenceDecor)]
    public class ServerHousingResidenceDecor : IWritable
    {
        public class Decor : IWritable
        {
            public ushort RealmId { get; set; }
            public ulong DecorId { get; set; }
            public ulong ResidenceId { get; set; }
            public DecorType DecorType { get; set; }
            public uint DecorData { get; set; }
            public uint HookBagIndex { get; set; }
            public uint HookIndex { get; set; }
            public uint PlotIndex { get; set; } = uint.MaxValue;
            public float Scale { get; set; }
            public Vector3 Position { get; set; } = new Vector3();
            public Quaternion Rotation { get; set; } = new Quaternion();
            public uint DecorInfoId { get; set; }
            public uint ActivePropUnitId { get; set; }
            public ulong ParentDecorId { get; set; }
            public ushort ColourShift { get; set; }
            
            public void Write(GamePacketWriter writer)
            {
                writer.Write(RealmId, 14u);
                writer.Write(ResidenceId);
                writer.Write(DecorId);
                writer.Write(DecorType, 32u);
                writer.Write(DecorData);
                writer.Write(HookBagIndex);
                writer.Write(HookIndex);
                writer.Write(PlotIndex);
                writer.Write(Scale);
                writer.Write(Position.X);
                writer.Write(Position.Y);
                writer.Write(Position.Z);
                writer.Write(Rotation.X);
                writer.Write(Rotation.Y);
                writer.Write(Rotation.Z);
                writer.Write(Rotation.W);
                writer.Write(DecorInfoId);
                writer.Write(ActivePropUnitId);
                writer.Write(ParentDecorId);
                writer.Write(ColourShift, 14u);
            }
        }

        public uint Operation { get; set; }
        public List<Decor> DecorData { get; set; } = new List<Decor>();
        
        public void Write(GamePacketWriter writer)
        {
            writer.Write(DecorData.Count);
            writer.Write(Operation);
            DecorData.ForEach(d => d.Write(writer));
        }
    }
}
