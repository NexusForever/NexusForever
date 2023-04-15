using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Guild;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class GuildHandler
    {
        [MessageHandler(GameMessageOpcode.ClientGuildRegister)]
        public static void HandleGuildRegister(IWorldSession session, ClientGuildRegister guildRegister)
        {
            IGuildResultInfo GetResult()
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
                        if (guildRegister.AlternateCost && !session.Account.CurrencyManager.CanAfford(AccountCurrencyType.ServiceToken, entry.Dataint01))
                            return new GuildResultInfo(GuildResult.NotEnoughCredits); // this right guild result for account credits?
                        if (!guildRegister.AlternateCost && !session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, entry.Dataint0))
                            return new GuildResultInfo(GuildResult.NotEnoughCredits);
                        break;
                    }
                }

                return session.Player.GuildManager.CanRegisterGuild(guildRegister);
            }

            IGuildResultInfo result = GetResult();
            if (result.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(session, result);
                return;
            }

            session.Player.GuildManager.RegisterGuild(guildRegister);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildHolomarkUpdate)]
        public static void HandleHolomarkUpdate(IWorldSession session, ClientGuildHolomarkUpdate guildHolomarkUpdate)
        {
            session.Player.GuildManager.UpdateHolomark(guildHolomarkUpdate.LeftHidden, guildHolomarkUpdate.RightHidden,
                guildHolomarkUpdate.BackHidden, guildHolomarkUpdate.DistanceNear);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildOperation)]
        public static void HandleOperation(IWorldSession session, ClientGuildOperation clientGuildOperation)
        {
            GlobalGuildManager.Instance.HandleGuildOperation(session.Player, clientGuildOperation);
        }

        [MessageHandler(GameMessageOpcode.ClientGuildInviteResponse)]
        public static void HandleInviteResponse(IWorldSession session, ClientGuildInviteResponse guildInviteResponse)
        {
            IGuildResultInfo info = session.Player.GuildManager.CanAcceptInviteToGuild();
            if (info.Result != GuildResult.Success)
            {
                GuildBase.SendGuildResult(session, info);
                return;
            }

            session.Player.GuildManager.AcceptInviteToGuild(guildInviteResponse.Accepted);
        }
    }
}
