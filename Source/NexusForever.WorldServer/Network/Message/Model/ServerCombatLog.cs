using System.Collections.Generic;
using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;

namespace NexusForever.WorldServer.Network.Message.Model
{
    [Message(GameMessageOpcode.ServerCombatLog, MessageDirection.Server)]
    public class ServerCombatLog : IWritable
    {
        public byte function { get; set; }
        public uint UintId { get; set; }
        public uint Damage { get; set; }
        public uint Unknown3 { get; set; } = 0;

        public uint Unknown4 { get; set; } = 0;
        public byte Unknown5 { get; set; } = 0;
        public bool Unknown6 { get; set; } = false;
        public uint UnitId1 { get; set; } = 0;
        public uint UnitId2 { get; set; } = 0;
        public uint Spell4Id { get; set; } = 0;
        public byte Unknown7 { get; set; }
        
        public bool Unknown8 { get; set; } = false;

        public void Write(GamePacketWriter writer)
        {
            writer.Write(function, 6);
            if (function == 9)
            {
                writer.Write(UintId);
                writer.Write(Damage);
                writer.Write(Unknown3);
            } else if (function == 16) // 32 5 1  [ 32 32 18 4 ] 
            {
                writer.Write(Unknown4);
                writer.Write(Unknown5, 5);
                writer.Write(Unknown6);
                writer.Write(UnitId1);
                writer.Write(UnitId2);
                writer.Write(Spell4Id, 18);
                writer.Write(Unknown7, 4);
            } else if (function == 17) // 1 [ 32 32 18 4 ]
            {
                writer.Write(Unknown8);
                writer.Write(UnitId1);
                writer.Write(UnitId2);
                writer.Write(Spell4Id, 18);
                writer.Write(Unknown7, 4);
            }
        }
    }
}
