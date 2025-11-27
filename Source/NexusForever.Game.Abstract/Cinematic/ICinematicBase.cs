using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Static.Cinematic;

namespace NexusForever.Game.Abstract.Cinematic
{
    public interface ICinematicBase
    {
        ushort CinematicId { get; set; }
        uint Duration { get; set; }
        CinematicFlags InitialFlags { get; set; }
        CancelType InitialCancelMode { get; set; }
        Dictionary<uint, IActor> Actors { get; }
        Dictionary<uint, uint> Texts { get; }
        Dictionary<string, List<IKeyframeAction>> Keyframes { get; }
        List<ICamera> Cameras { get; }
        ITransition StartTransition { get; }
        ITransition EndTransition { get; }

        /// <summary>
        /// Starts Playback for this <see cref="ICinematicBase"/>, sending the packets to the Player.
        /// </summary>
        void StartPlayback(IPlayer player);
    }
}