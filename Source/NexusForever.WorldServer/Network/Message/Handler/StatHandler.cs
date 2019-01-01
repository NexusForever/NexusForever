using NexusForever.Shared.Network.Message;
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
            if (session.Player.Stats.ContainsKey(Stat.Sheathed))
            {
                if (session.Player.Stats[Stat.Sheathed].Value == 1)
                    session.Player.Stats[Stat.Sheathed].Value = 0;
                else
                    session.Player.Stats[Stat.Sheathed].Value = 1;
            }

            session.Player.EnqueueToVisible(new ServerStatUpdateInt
            {
                Guid = session.Player.Guid,
                Stat = Stat.Sheathed,
                Value = (uint)session.Player.Stats[Stat.Sheathed].Value
            });
        }
    }
}
