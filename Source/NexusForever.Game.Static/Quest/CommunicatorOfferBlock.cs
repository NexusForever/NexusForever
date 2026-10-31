namespace NexusForever.Game.Static.Quest
{
    /// <summary>
    /// Why a call can't offer its quest, or <see cref="None"/> when it can.
    /// </summary>
    public enum CommunicatorOfferBlock : byte
    {
        None,
        RecordExists,
        Disabled,
        CannotStart,
        NoCallbackFlag
    }
}
