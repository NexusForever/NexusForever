using NexusForever.Game.Abstract.Cinematic;
using NexusForever.Network.Session;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class CameraAttach : ICameraAttach
    {
        public uint AttachType { get; set; }
        public uint AttachId { get; set; }
        public uint Delay { get; set; }
        public uint ParentUnit { get; set; }
        public bool UseRotation { get; set; }

        public CameraAttach(uint delay, uint attachId, ICamera parentUnit, uint attachType = 0, bool useRotation = true)
        {
            Delay       = delay;
            AttachId    = attachId;
            ParentUnit  = parentUnit.CameraActor.Id;
            AttachType  = attachType;
            UseRotation = useRotation;
        }

        public void Send(IGameSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicCameraAttach
            {
                AttachType  = AttachType,
                AttachId    = AttachId,
                Delay       = Delay,
                ParentUnit  = ParentUnit,
                UseRotation = UseRotation
            });
        }
    }
}
