using NexusForever.Shared.Network.Message;
using NexusForever.WorldServer.Game.Storefront;
using NexusForever.WorldServer.Network.Message.Model;

namespace NexusForever.WorldServer.Network.Message.Handler
{
    public static class AccountHandler
    {
        [MessageHandler(GameMessageOpcode.ClientStorefrontRequestCatalog)]
        public static void HandleStorefrontRequestCatalogRealm(WorldSession session, ClientStorefrontRequestCatalog storefrontRequest)
        {
            // Packet order below, for reference and implementation

            // 0x096D - Account inventory

            // 0x0974 - Server Account Item Cooldowns (Boom Box!)
            
            // 0x0968 - Entitlements

            // 0x097F - Account Tier (Basic/Signature)

            // 0x0966 - SetAccountCurrencyAmounts

            // 0x096F - Weekly Omnibit progress

            // 0x096E - Daily Rewards packet
                // 0x078F - Claim Reward Button

            // 0x0981 - Unknown

            // Store packets
            // 0x0988 - Store catalogue categories 
            // 0x098B - Store catalogue offer grouips + offers
            // 0x0987 - Store catalogue finalised message
            GlobalStorefrontManager.Instance.HandleCatalogRequest(session);
        }
    }
}