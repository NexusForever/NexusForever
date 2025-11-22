namespace NexusForever.Game.Static.PlayerPath
{
    public enum PlayerPathSoldierResult
    {
        FailUnknown        = 0,
        FailTimeOut        = 1,
        FailDefenceDeath   = 2,
        FailNoParticipants = 4,
        FailLeaveArea      = 5,
        FailDeath          = 6,
        FailLostResources  = 3,
        FailParticipation  = 7,
        ScriptCancel       = 8,
        Success            = 9,
    };
}
