using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusForever.WorldServer.Game.Loot.Static
{
    public enum LootConditionType
    {
        None                    = 0,
        IsClass                 = 1,
        IsRace                  = 2,
        IsLevel                 = 3,
        IsLessThanLevel         = 4,
        IsMoreThanLevel         = 5,
        QuestIsComplete         = 6,
        QuestNotComplete        = 7,
        QuestObjectiveActive    = 8
    }
}
