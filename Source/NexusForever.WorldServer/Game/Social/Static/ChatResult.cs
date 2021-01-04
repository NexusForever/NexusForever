namespace NexusForever.WorldServer.Game.Social.Static
{
    public enum ChatResult
    {
		Ok,
        DoesntExist = 1,
	    BadPassword = 2,
        NoPermissions = 3,
        NoSpeaking = 4,
        Muted = 5,
	    Throttled = 6,
	    NotInGroup = 8,
	    NotInGuild = 9,
	    NotInSociety = 10,
	    NotGuildOfficer = 11,
	    AlreadyMember = 12,
	    BadName = 13,
	    NotMember = 14,
	    NotInWarParty = 15,
	    NotWarPartyOfficer = 16,
	    InvalidMessageText = 17,
	    InvalidPasswordText = 18,
	    TruncatedText = 19,
	    InvalidCharacterName = 20,
	    GMMuted = 21,
        TooManyCustomChannels = 23,
	    MissingEntitlement = 24
    }
}
