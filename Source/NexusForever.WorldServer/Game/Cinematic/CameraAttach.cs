using NexusForever.WorldServer.Game.Cinematic.Static;
using NexusForever.WorldServer.Network;
using NexusForever.WorldServer.Network.Message.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Cinematic
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
            Delay = delay;
            AttachId = attachId;
            ParentUnit = parentUnit.CameraActor.Id;
            AttachType = attachType;
            UseRotation = useRotation;
        }

        public void Send(WorldSession session)
        {
            session.EnqueueMessageEncrypted(new ServerCinematicCameraAttach
            {
                AttachType = AttachType,
                AttachId = AttachId,
                Delay = Delay,
                ParentUnit = ParentUnit,
                UseRotation = UseRotation
            });
        }
    }
}
