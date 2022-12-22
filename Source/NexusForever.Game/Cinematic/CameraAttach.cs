using NexusForever.Game.Network;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Cinematic
{
    public class CameraAttach : IKeyframeAction
    {
        public uint AttachType { get; set; }
        public uint AttachId { get; set; }
        public uint Delay { get; set; }
        public uint ParentUnit { get; set; }
        public bool UseRotation { get; set; }

        public CameraAttach(uint delay, uint attachId, Camera parentUnit, uint attachType = 0, bool useRotation = true)
        {
            Delay       = delay;
            AttachId    = attachId;
            ParentUnit  = parentUnit.CameraActor.Id;
            AttachType  = attachType;
            UseRotation = useRotation;
        }

        public void Send(WorldSession session)
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
