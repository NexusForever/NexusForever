using System.Numerics;
using NexusForever.Game.Static.Housing;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model.Shared;

namespace NexusForever.Network.World.Message.Model.Housing
{
    // This message is not for roofs, entryways, or doors. Client's handler for this message
    // ignores decor items of these types.
    [Message(GameMessageOpcode.ServerHousingResidenceDecor)]
    public class ServerHousingResidenceDecor : IWritable
    {
        public class Decor : IWritable
        {
            public Identity ResidenceIdentity { get; set; }
            public ulong DecorId { get; set; }
            public DecorType DecorType { get; set; }
            public uint MannequinDecorId { get; set; } // Separate guid for each piece of mannequin items
            public uint Flags { get; set; } // Used as an override for housingWallpaperInfo.flags
            public uint HookIndex { get; set; }
            public uint PlotIndex { get; set; } = int.MaxValue;
            public float Scale { get; set; }
            public Vector3 Position { get; set; } = new();
            public Quaternion Rotation { get; set; } = new();
            public uint HousingDecorInfoId { get; set; }
            public uint ActivePropUnitId { get; set; } // Prop unit the decor is attached to
            public ulong ParentDecorId { get; set; }
            public ushort ColourShift { get; set; }

            public void Write(GamePacketWriter writer)
            {
                ResidenceIdentity.Write(writer);
                writer.Write(DecorId);
                writer.Write(DecorType, 32u);
                writer.Write(MannequinDecorId);
                writer.Write(Flags);
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
                writer.Write(HousingDecorInfoId);
                writer.Write(ActivePropUnitId);
                writer.Write(ParentDecorId);
                writer.Write(ColourShift, 14u);
            }
        }

        public uint MessagesRemaining { get; set; } // decor data must be broken into chunks of 100 decor items per message, until this reaches 0 the client will not try to display the zone
        public List<Decor> DecorData { get; set; } = []; // maximum 100 per message

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DecorData.Count);
            writer.Write(MessagesRemaining);
            DecorData.ForEach(decor => decor.Write(writer));
        }
    }
}
