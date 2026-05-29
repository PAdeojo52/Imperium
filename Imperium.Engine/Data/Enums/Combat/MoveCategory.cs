namespace Imperium.Engine.Data.Enums.Combat
{
    public enum CombatMode
    {
        Arena,
        Battle
    }

    public enum MoveCategory
    {
        Unarmed,
        Weapon,
        Magic,
        Special,
        Taunt,
        Whisper,
        Defensive,
        Item
    }

    public enum MoveTarget
    {
        Opponent,
        Self,
        Crowd,
        AllEnemies,
        AllAllies
    }

    public enum CrowdReaction
    {
        Bored,
        Neutral,
        Interested,
        Excited,
        Electric
    }

    public enum CombatOutcome
    {
        Victory,
        Defeat,
        Draw,
        Fled,
        Submission
    }

    public enum PostBattleDecision
    {
        Spare,
        Kill
    }

    public enum WhisperResult
    {
        Accepted,
        Refused,
        Exposed
    }
}
