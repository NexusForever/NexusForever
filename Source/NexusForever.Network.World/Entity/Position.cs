using System.Numerics;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Entity
{
    public class Position : IReadable, IWritable
    {
        public Vector3 Vector { get; private set; }

        public Position()
        {
        }

        public Position(Vector3 vector)
        {
            Vector = vector;
        }

        public void Read(GamePacketReader reader)
        {
            Vector = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle());
        }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Vector.X);
            writer.Write(Vector.Y);
            writer.Write(Vector.Z);
        }
    }
}
