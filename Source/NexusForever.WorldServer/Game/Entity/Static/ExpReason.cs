using System;
using System.Collections.Generic;
using System.Text;

namespace NexusForever.WorldServer.Game.Entity.Static
{
    public enum ExpReason
    {
        Invalid     = -1,
        Load,
        Cheat,
        KillCreature,
        Quest,
        ClusterBonus,
        Spell,
        Exploration,
        QuestEpisodeCompletion,
        PathMission,
        PathEpisode,
        KillPerformance,
        MultiKill,
        KillingSpree,
        TelegraphInterrupt,
        TelegraphEvade,
        Momentum,
        Rested,
        PublicEvent,
        PeriodicQuest
    }
}
