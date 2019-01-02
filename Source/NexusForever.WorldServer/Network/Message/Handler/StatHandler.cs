using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Entity;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Network.Message.Model;
using NLog;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class StatHandler
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        [MessageHandler(GameMessageOpcode.ClientToggleWeapons)]
        public static void HandleWeaponToggle(WorldSession session, ClientToggleWeapons clientToggleWeapons)
        {
            // TODO: Ensure player not in combat

            if (session.Player.Stats.ContainsKey(Stat.Sheathed))
                session.Player.Stats[Stat.Sheathed].Value = clientToggleWeapons.ToggleState;
            else
                session.Player.Stats.Add(Stat.Sheathed, new StatValue(Stat.Sheathed, clientToggleWeapons.ToggleState));

            session.Player.EnqueueToVisible(new ServerStatUpdateInt
            {
                Guid = session.Player.Guid,
                Stat = Stat.Sheathed,
                Value = (uint)session.Player.Stats[Stat.Sheathed].Value
            });
        }
    }
}
