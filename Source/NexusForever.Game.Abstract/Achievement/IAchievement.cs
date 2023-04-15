using NexusForever.Database.Character;
using NexusForever.Network.Message;

namespace NexusForever.Game.Abstract.Achievement
{
    public interface IAchievement : IDatabaseCharacter, INetworkBuildable<Network.World.Message.Model.Shared.Achievement>
    {
        IAchievementInfo Info { get; }
        ushort Id { get; }
        uint Data0 { get; set; }
        uint Data1 { get; set; }
        DateTime? DateCompleted { get; set; }

        /// <summary>
        /// Returns if <see cref="IAchievement"/> has been completed.
        /// </summary>
        bool IsComplete();
    }
}