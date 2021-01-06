using NexusForever.Shared.GameTable;
using NexusForever.Shared.GameTable.Model;
using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Account.Static;
using NexusForever.WorldServer.Game.Entity.Static;
using NexusForever.WorldServer.Game.Guild;
using NexusForever.WorldServer.Game.Guild.Static;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class GuildHandler
    {
        [MessageHandler(GameMessageOpcode.ClientGuildRegister)]
        public static void HandleGuildRegister(WorldSession session, ClientGuildRegister guildRegister)
        {
            GuildResultInfo GetResult()
            {
                // hardcoded GameFormula entries come from client GuildLib.GetCreateCost/GetAlternateCreateCost
                switch (guildRegister.GuildType)
                {
                    case GuildType.Guild:
                    {
                        GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(764);
                        if (!session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, entry.Dataint0))
                            return new GuildResultInfo(GuildResult.NotEnoughCredits);
                        break;
                    }
                    case GuildType.Community:
                    {
                        GameFormulaEntry entry = GameTableManager.Instance.GameFormula.GetEntry(1159);
                        if (guildRegister.AlternateCost && !session.AccountCurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, entry.Dataint01))
                            return new GuildResultInfo(GuildResult.NotEnoughCredits); // this right guild result for account credits?
                        if (!session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, entry.Dataint0))
                            return new GuildResultInfo(GuildResult.NotEnoughCredits);
                        break;
                    }
                }

                return session.Player.GuildManager.CanRegisterGuild(guildRegister);
            }

            GuildResultInfo result = GetResult();
            if (result.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(session, result);
                return;
            }

            session.Player.GuildManager.RegisterGuild(guildRegister);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildHolomarkUpdate)]
        public static void HandleHolomarkUpdate(WorldSession session, ClientGuildHolomarkUpdate guildHolomarkUpdate)
        {
            session.Player.GuildManager.UpdateHolomark(guildHolomarkUpdate.LeftHidden, guildHolomarkUpdate.RightHidden,
                guildHolomarkUpdate.BackHidden, guildHolomarkUpdate.DistanceNear);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildOperation)]
        public static void HandleOperation(WorldSession session, ClientGuildOperation clientGuildOperation)
        {
            GlobalGuildManager.Instance.HandleGuildOperation(session.Player, clientGuildOperation);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildInviteResponse)]
        public static void HandleInviteResponse(WorldSession session, ClientGuildInviteResponse guildInviteResponse)
        {
            GuildResultInfo info = session.Player.GuildManager.CanAcceptInviteToGuild();
            if (info.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(session, info);
                return;
            }

            session.Player.GuildManager.AcceptInviteToGuild(guildInviteResponse.Accepted);
        }
    }
}
