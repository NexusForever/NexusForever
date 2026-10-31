namespace NexusForever.Game.Static.Quest
{
    /// <summary>
    /// How a CommunicatorMessages row reaches the client. Flags bit 0 marks a row the server pushes; the client drops a
    /// push of any other row (client 0x14043b7d3) and fires those itself.
    /// </summary>
    public enum CommunicatorKind : byte
    {
        /// <summary>Bit 0, armed by a quest state or objective: the server pushes it.</summary>
        PushedScheduled,
        /// <summary>Bit 0, delivers a quest, nothing to arm it: needs a script.</summary>
        PushedUnconditioned,
        /// <summary>Bit 0, delivers nothing and has no trigger: only a script can send it.</summary>
        PushedScript,
        /// <summary>Bit 0 and the next message of another row: the client plays it after its head.</summary>
        ChainTarget,
        /// <summary>Bit 0 clear, delivers a quest, has a trigger: the client shows it; the server only mentions the quest.</summary>
        ClientScheduled,
        /// <summary>Bit 0 clear, delivers a quest, no trigger: client-fired.</summary>
        ClientUnscheduled,
        /// <summary>Bit 0 clear and delivers nothing: client-fired, nothing for the server to do.</summary>
        ClientOnly
    }
}
