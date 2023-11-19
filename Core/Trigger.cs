namespace Kalkatos.Firecard.Core
{
    /// <summary>
    /// Moments in the match relevant for the rules.
    /// </summary>
    public enum Trigger
    {
        OnMatchStarted,     // 
        OnMatchEnded,       //
        OnTurnStarted,      // Turn number
        OnTurnEnded,        // Turn number
        OnPhaseStarted,     // Phase name
        OnPhaseEnded,       // Phase name
        OnCardUsed,         // Card
        OnZoneUsed,         // Zone
        OnCardEnteredZone,  // Card , Zone
        OnCardLeftZone,     // Card , Zone
        OnActionUsed,       // Action name
        OnVariableChanged,  // Variable name , old value , new value
        OnRuleActivated,    // Rule
    }
}