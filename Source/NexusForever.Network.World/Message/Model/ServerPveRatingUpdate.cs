using NexusForever.Game.Static.Matching;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model
{
    // Very similar to ServerPvpRatingUpdate, but not sure what goes in it
    // This triggers the lua event PveRatingUpdated but there are no uses of it in Carbine's lua code
    [Message(GameMessageOpcode.ServerPveRatingUpdate)]
    public class ServerPveRatingUpdate : IWritable
    {
        public class PveRating : IWritable
        {
            public uint Category { get; set; } // similar to ServerPvpRatingUpdate, not sure what the categories are
            public MatchingGameRatingType Type { get; set; }
            public uint Unknown1 { get; set; }
            public uint Unknown2 { get; set; }
            public uint Unknown3 { get; set; }
            public uint Unknown4 { get; set; }

            public void Write(GamePacketWriter writer)
            {
                writer.Write(Category, 8u);
                writer.Write(Type, 3u);
                writer.Write(Unknown1);
                writer.Write(Unknown2);
                writer.Write(Unknown3);
                writer.Write(Unknown4);
            }
        }

        public List<PveRating> PveRatings { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PveRatings.Count);
            foreach (var rating in PveRatings)
            {
                rating.Write(writer);
            }
        }
    }
}
