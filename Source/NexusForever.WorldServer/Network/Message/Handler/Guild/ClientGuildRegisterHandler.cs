using NexusForever.Game.Abstract.Guild;
using NexusForever.Game.Guild;
using NexusForever.Game.Static.Account;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Guild;
using NexusForever.GameTable;
using NexusForever.GameTable.Model;
using NexusForever.Network.Message;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler.Guild
{
    public class ClientGuildRegisterHandler : IMessageHandler<IWorldSession, ClientGuildRegister>
    {
        #region Dependency Injection

        private readonly IGameTableManager gameTableManager;

        public ClientGuildRegisterHandler(
            IGameTableManager gameTableManager)
        {
            this.gameTableManager = gameTableManager;
        }

        #endregion

        public void HandleMessage(IWorldSession session, ClientGuildRegister guildRegister)
        {
            IGuildResultInfo GetResult()
            {
                // hardcoded GameFormula entries come from client GuildLib.GetCreateCost/GetAlternateCreateCost
                switch (guildRegister.GuildType)
                {
                    case GuildType.Guild:
                    {
                        GameFormulaEntry entry = gameTableManager.GameFormula.GetEntry(764);
                        if (!session.Player.CurrencyManager.CanAfford(CurrencyType.Credits, entry.Dataint0))
                            return new GuildResultInfo(GuildResult.NotEnoughCredits);
                        break;
                    }
                    case GuildType.Community:
                    {
                        GameFormulaEntry entry = gameTableManager.GameFormula.GetEntry(1159);
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
    }
}
