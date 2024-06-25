using NexusForever.Network.Message;
using NexusForever.Network.Session;
using NexusForever.Network.World.Entity;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface IActor
    {
        uint Id { get; }
        uint InitialDelay { get; }
        uint CreatureType { get; }
        ushort Flags { get; }
        ushort Unknown0 { get; }
        uint MovementMode { get; }
        float? Angle { get; }
        Position InitialPosition { get; }
        List<IVisualEffect> InitialVisualEffects { get; }
        List<IKeyframeAction> Keyframes { get; }
        ulong ActivePropId { get; }
        uint SocketId { get; }

        List<IWritable> PacketsToSend { get; }

        void AddVisualEffect(IVisualEffect visualEffect);
        void AddPacketToSend(IWritable packet);
        void AddVisibility(uint delay, bool hide);
        void SendInitialPackets(IGameSession session);
    }
}