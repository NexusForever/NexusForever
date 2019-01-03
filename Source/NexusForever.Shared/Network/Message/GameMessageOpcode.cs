namespace NexusForever.Shared.Network.Message
{
    public enum GameMessageOpcode
    {
        State                           = 0x0000,
        State2                          = 0x0001,
        ServerHello                     = 0x0003,
        ServerMaxCharacterLevelAchieved = 0x0036,
        ServerPlayerEnteredWorld        = 0x0061,
        ServerAuthEncrypted             = 0x0076,
        ServerCharacterLogoutStart      = 0x0092,
        ServerChangeWorld               = 0x00AD,
        ServerBuybackItemUpdated        = 0x00BA,
        ClientBuybackItemFromVendor     = 0x00BB,
        ServerBuybackItems              = 0x00BC,
        ServerBuybackItemRemoved        = 0x00BD,
        ClientVendorPurchase            = 0x00BE,
        ClientCharacterLogout           = 0x00BF,
        ClientLogout                    = 0x00C0,
        ServerCharacterCreate           = 0x00DC,
        ServerChannelUpdateLoot         = 0x00DD,
        Server00F1                      = 0x00F1,
        Server0104                      = 0x0104, // Galactic Archive
        ServerCharacter                 = 0x010F, // single character
        ServerItemAdd                   = 0x0111,
        ServerCharacterList             = 0x0117,
        ServerMountUnlocked             = 0x0129,
        ServerItemDelete                = 0x0148,
        ClientItemDelete                = 0x0149,
        ServerCharacterSelectFail       = 0x0162,
        ClientSellItemToVendor          = 0x0166,
        ServerAbilityPoints             = 0x0169,
        ClientItemSplit                 = 0x017D,
        ServerItemStackCountUpdate      = 0x017F,
        ClientItemMove                  = 0x0182,
        ClientEntitySelect              = 0x0185,
        ServerFlightPathUpdate          = 0x0188,
        ServerTitleSet                  = 0x0189,
        ServerTitleUpdate               = 0x018A,
        ServerTitles                    = 0x018B,
        ServerPlayerChanged             = 0x019B,
        ServerActionSet                 = 0x019D,
        ServerAbilities                 = 0x01A0,
        Server01A3                      = 0x01A3, // AMP
        ServerPathUpdateXP              = 0x01AA,
        ServerChatJoin                  = 0x01BC,
        ServerChatAccept                = 0x01C2,
        ClientChat                      = 0x01C3,
        ServerChat                      = 0x01C8,
        Server0237                      = 0x0237, // UI related, opens or closes different UI windows (bank, barber, ect...)
        ClientPing                      = 0x0241,
        ClientEncrypted                 = 0x0244,
        ServerCombatLog                 = 0x0247,
        ClientCharacterCreate           = 0x025B,
        ClientPacked                    = 0x025C, // the same as ClientEncrypted except the contents isn't encrypted?
        ServerPlayerCreate              = 0x025E,
        ServerEntityCreate              = 0x0262,
        ClientCharacterDelete           = 0x0352,
        ServerEntityDestory             = 0x0355,
        ClientEmote                     = 0x037E,
        Server03AA                      = 0x03AA, // friendship account related
        Server03BE                      = 0x03BE, // friendship related
        ServerRealmInfo                 = 0x03DB,
        ServerRealmEncrypted            = 0x03DC,
        ClientCheat                     = 0x03E0,
        Server0497                      = 0x0497, // guild info
        ClientCastSpell                 = 0x04DB,
        ServerItemSwap                  = 0x0568,
        ServerItemMove                  = 0x0569,
        ClientHelloRealm                = 0x058F,
        ServerAuthAccepted              = 0x0591,
        ClientHelloAuth                 = 0x0592,
        ServerMovementControl           = 0x0636,
        ServerClientLogout              = 0x0594,
        ClientEntityCommand             = 0x0637, // bidirectional? packet has both read and write handlers 
        ServerEntityCommand             = 0x0638, // bidirectional? packet has both read and write handlers
        ClientPathActivate              = 0x06B2,
        ServerPathActivateResult        = 0x06B3,
        ServerPathRefresh               = 0x06B4,
        ServerPathEpisodeProgress       = 0x06B5,
        Server06B6                      = 0x06B6,
        Server06B7                      = 0x06B7,
        Server06B8                      = 0x06B8,
        Server06B9                      = 0x06B9,
        ServerPathMissionActivate       = 0x06BA, 
        ServerPathMissionUpdate         = 0x06BB, 
        ServerPathLog                   = 0x06BC,
        ClientPathUnlock                = 0x06BD,
        ServerPathUnlockResult          = 0x06BE,
        ServerPathCurrentEpisode        = 0x06BF,
        ServerRealmList                 = 0x0761, // bidirectional? packet has both read and write handlers
        ServerRealmMessages             = 0x0763,
        ClientTitleSet                  = 0x078E,
        ClientRealmList                 = 0x07A4,
        ClientCharacterList             = 0x07E0,
        ClientVendor                    = 0x07EA,
        ClientCharacterSelect           = 0x07DD,
        Server07FF                      = 0x07FF,
        ClientStorefrontRequestCatalog  = 0x082D,
        Server0854                      = 0x0854, // crafting schematic
        Server0856                      = 0x0856, // tradeskills
        Server086F                      = 0x086F,
        Server08B3                      = 0x08B3,
        ServerSetUnitPathType           = 0x08B8,
        ServerVendorItemsUpdated        = 0x090B,
        ServerPlayerCurrencyChanged     = 0x0919,
        ServerItemVisualUpdate          = 0x0933,
        Server0934                      = 0x0934,
        ServerEmote                     = 0x093C,
        ClientWhoRequest                = 0x0959,
        ServerWhoResponse               = 0x095A,
        ServerGrantAccountCurrency      = 0x0967,
        ServerAccountEntitlements       = 0x0968
    }
}
