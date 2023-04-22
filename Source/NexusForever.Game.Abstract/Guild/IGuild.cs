using NexusForever.Game.Abstract.Achievement;

namespace NexusForever.Game.Abstract.Guild
{
    public interface IGuild : IGuildChat
    {
        IGuildStandard Standard { get; }
        IGuildAchievementManager AchievementManager { get; }
        string MessageOfTheDay { get; set; }
        string AdditionalInfo { get; set; }

        /// <summary>
        /// Set if taxes are enabled for <see cref="IGuild"/>.
        /// </summary>
        void SetTaxes(bool enabled);
    }
}