using NexusForever.Shared.Network;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Setting.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SettingHandler
    {
        [MessageHandler(GameMessageOpcode.BiInputKeySet)]
        public static void HandleKeybindingUpdate(WorldSession session, BiInputKeySet biInputKeySet)
        {
            session.Player.KeybindingManager.SaveKeybinding(biInputKeySet);
        }

        [MessageHandler(GameMessageOpcode.ClientRequestInputKeySet)]
        public static void HandleRequestInputKeySet(WorldSession session, ClientRequestInputKeySet clientRequestInputKeySet)
        {
            session.Player.KeybindingManager.SendInputKeySet(clientRequestInputKeySet.CharacterId);
        }

        [MessageHandler(GameMessageOpcode.ClientSetInputKeySet)]
        public static void HandleSetInputKeySet(WorldSession session, ClientSetInputKeySet clientSetInputKeySet)
        {
            if (clientSetInputKeySet.InputKeySetEnum < InputSets.MaxValue && clientSetInputKeySet.InputKeySetEnum >= (InputSets)0)
                session.Player.InputKeySet = clientSetInputKeySet.InputKeySetEnum;
            else
                throw new InvalidPacketValueException($"Invalid InputKeySet received: {clientSetInputKeySet.InputKeySetEnum}");
        }
    }
}
