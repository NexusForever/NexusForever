using NexusForever.Game.Static.Setting;
using NexusForever.Network;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class SettingHandler
    {
        [MessageHandler(GameMessageOpcode.BiInputKeySet)]
        public static void HandleKeybindingUpdate(IWorldSession session, BiInputKeySet biInputKeySet)
        {
            if (biInputKeySet.CharacterId != 0ul)
                session.Player.KeybindingManager.Update(biInputKeySet);
            else
                session.Account.KeybindingManager.Update(biInputKeySet);
        }

        [MessageHandler(GameMessageOpcode.ClientRequestInputKeySet)]
        public static void HandleRequestInputKeySet(IWorldSession session, ClientRequestInputKeySet clientRequestInputKeySet)
        {
            if (clientRequestInputKeySet.CharacterId != 0ul)
                session.EnqueueMessageEncrypted(session.Player.KeybindingManager.Build());
            else
                session.EnqueueMessageEncrypted(session.Account.KeybindingManager.Build());
        }

        [MessageHandler(GameMessageOpcode.ClientSetInputKeySet)]
        public static void HandleSetInputKeySet(IWorldSession session, ClientSetInputKeySet clientSetInputKeySet)
        {
            if (clientSetInputKeySet.InputKeySetEnum is not InputSets.Account and not InputSets.Character)
                throw new InvalidPacketValueException($"Invalid InputKeySet received: {clientSetInputKeySet.InputKeySetEnum}"!);

            session.Player.InputKeySet = clientSetInputKeySet.InputKeySetEnum;
        }
    }
}
