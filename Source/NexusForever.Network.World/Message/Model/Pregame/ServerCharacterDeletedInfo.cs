using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Pregame
{
    // Not a very useful message but included for completeness.
    // Client only reads this message when on the CharacaterSelect screen.
    // Data is accessible by calling lua function CharacterScreenLib::GetDeletedCharacterInfo
    // which is not used by the Carbine UI. Requires custom CharacterSelect screen to be useful.
    [Message(GameMessageOpcode.ServerCharacterDeletedInfo)]
    public class ServerCharacterDeletedInfo : IWritable
    {
        public class DeletedCharacterInfo : IWritable
        {
            public ulong CharacterId { get; set; }
            public uint Unused { get; set; }
            public string Name { get; set; }
            public uint Level { get; set; }
            public ushort Class { get; set; }
            public ushort Race { get; set; }
            public ushort Gender { get; set; }
            public ushort Faction { get; set; }
            public ushort Path { get; set; }
            public ushort WorldId { get; set; }
            public ushort WorldZoneId { get; set; }
            public float PurgeDate { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(CharacterId);
                writer.Write(Unused);
                writer.WriteStringWide(Name);
                writer.Write(Level);
                writer.Write(Class, 14u);
                writer.Write(Race, 14u);
                writer.Write(Gender, 2u);
                writer.Write(Faction, 14u);
                writer.Write(Path, 3u);
                writer.Write(WorldId, 15u);
                writer.Write(WorldZoneId, 15u);
                writer.Write(PurgeDate);
            }
        }

        public List<DeletedCharacterInfo> DelectedCharacters { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(DelectedCharacters.Count);
            DelectedCharacters.ForEach(m => m.Write(writer));
        }
    }
}
