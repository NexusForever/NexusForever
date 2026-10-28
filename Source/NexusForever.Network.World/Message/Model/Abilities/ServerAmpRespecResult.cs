using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Static;

namespace NexusForever.Network.World.Message.Model.Abilities
{
    // Originally called Eldan Augmentations. See eldanAugmentation tbl for information
    [Message(GameMessageOpcode.ServerAmpRespecResult)]
    public class ServerAmpRespecResult : IWritable
    {
        public class AmpResult
        {
            public byte SpecIndex { get; set; }
            public LimitedActionSetResult Result { get; set; }
        }

        public List<AmpResult> Results { get; set; } = [];

        public void Write(GamePacketWriter writer)
        {
            // the client reads the count as 7 bits and then each array with the byte array primitive,
            // so every spec index and every result is a whole byte, not a 3 bit field.
            writer.Write(Results.Count, 7u);
            Results.ForEach(result => writer.Write(result.SpecIndex, 8u));
            Results.ForEach(result => writer.Write(result.Result, 8u));
        }
    }
}
