using NexusForever.Network.Message;
using static NexusForever.Network.World.Message.Model.ServerMatchingPvpRatingUpdated;

namespace NexusForever.Network.World.Message.Model
{
    // Very similar to ServerPvpRatingUpdate, but not sure what goes in it
    // This triggers the lua event PveRatingUpdated but there are no uses of it in Carbine's lua code
    [Message(GameMessageOpcode.ServerPveRatingUpdate)]
    public class ServerPveRatingUpdate : IWritable
    {
        public class PvpRating : IWritable
        {
            public uint Category; // similar to ServerPvpRatingUpdate, not sure what the categories are
            MatchingGameRatingType Type;
            public uint Unknown1; 
            public uint Unknown2;
            public uint Unknown3;
            public uint Unknown4;

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

        public List<PvpRating> PvpRatingList { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            writer.Write(PvpRatingList.Count);
            foreach (var rating in PvpRatingList)
            {
                rating.Write(writer);
            }
        }
    }
}
