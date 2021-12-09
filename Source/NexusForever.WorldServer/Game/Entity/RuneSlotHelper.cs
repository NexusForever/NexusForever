using NexusForever.Shared;
using NexusForever.WorldServer.Game.Entity.Static;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusForever.WorldServer.Game.Entity
{
    public class RuneSlotHelper : Singleton<RuneSlotHelper>
    {
        /// <summary>
        /// Generates a Rune Slot Mask for <see cref="Item"/>. Used to instruct the client what Rune Slot exist and their types.
        /// </summary>
        public uint GetRuneSlotMask(IEnumerable<RuneType> runeSlots, bool slotUnlocked)
        {
            uint mask = 0;
            int index = 0;

            foreach (RuneType type in runeSlots)
            {
                mask |= (uint)((int)type - 6) << (7 + 3 * index);
                index++;
            }

            if (slotUnlocked)
                mask |= 1;

            return mask;
        }

        /// <summary>
        /// Returns a random <see cref="RuneType"/>.
        /// </summary>
        public RuneType GetRandomRuneType(IEnumerable<RuneType> exclusionList = null)
        {
            // Optional parameters must be set to a constant. But, we want to run .Contains later.
            if (exclusionList == null)
                exclusionList = Enumerable.Empty<RuneType>();

            // Note: Fusion is excluded from the Random Roll. It's assumed that all Fusion sockets were placed using the ItemRuneInstance value,
            // and then the rest of the slots were filled in
            RuneType randomType = (RuneType)Random.Shared.Next((int)RuneType.Air, (int)RuneType.Fusion);
            if (exclusionList.Contains(randomType))
                return GetRandomRuneType(exclusionList);

            return randomType;
        }
    }
}
