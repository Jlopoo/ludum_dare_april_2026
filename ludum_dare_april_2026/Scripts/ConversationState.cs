/// <summary>
/// Lifecycle state of <see cref="ConversationManager"/>. Mutually exclusive — never combined.
/// </summary>
public enum ConversationState
{
    /// <summary>No conversation has been started yet this session.</summary>
    Idle,

    /// <summary>A line is currently displayed and the manager is waiting for <c>Advance()</c>.</summary>
    Presenting,

    /// <summary>A line with choices is displayed and the manager is waiting for <c>SelectChoice()</c>.</summary>
    AwaitingChoice,

    /// <summary>The most recent conversation has ended and no new one has started.</summary>
    Finished,
}
