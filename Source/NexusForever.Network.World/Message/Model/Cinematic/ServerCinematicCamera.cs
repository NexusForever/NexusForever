using NexusForever.Game.Static.Cinematic;
using NexusForever.Network.Message;

namespace NexusForever.Network.World.Message.Model.Cinematic
{
    [Message(GameMessageOpcode.ServerCinematicCamera)]
    public class ServerCinematicCamera : IWritable
    {
        public uint Delay { get; set; }
        public CameraAddFlags Flags { get; set; }
        public uint EndTransition { get; set; } // specifies type of transition
        public ushort TranDurationStart { get; set; }
        public ushort TranDurationMid { get; set; }
        public ushort TranDurationEnd { get; set; }

        public void Write(GamePacketWriter writer)
        {
            writer.Write(Delay);
            writer.Write(Flags, 32u);
            writer.Write(EndTransition);
            writer.Write(TranDurationStart);
            writer.Write(TranDurationMid);
            writer.Write(TranDurationEnd);
        }
    }
}
