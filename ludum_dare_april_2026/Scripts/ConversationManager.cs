using System;
using Godot;
using Godot.Collections;

#nullable enable

/// <summary>
/// Drives a single in-flight <see cref="Conversation"/>: tracks the current line, presents
/// choices, and emits typed signals so UI (HUD) and gameplay systems (affection, scoring)
/// can react without coupling to each other.
///
/// Lives as a child of the gameplay scene (e.g. <c>Main</c>) — not an autoload. Persistent
/// cross-scene state (which aliens you've dated, global affection) belongs in a separate
/// GameState autoload.
/// </summary>
public partial class ConversationManager : Node
{
    [Signal] public delegate void ConversationStartedEventHandler(Conversation conversation);

    [Signal] public delegate void LineAdvancedEventHandler(DialogueLine line);

    [Signal] public delegate void ChoicesPresentedEventHandler(Array<DialogueChoice> choices);

    [Signal] public delegate void ChoiceSelectedEventHandler(DialogueChoice choice);

    [Signal] public delegate void ConversationEndedEventHandler(string conversationId);

    /// <summary>
    /// Optional conversation to auto-start on <c>_Ready</c>. Useful for prototyping;
    /// production flow should call <see cref="Start"/> explicitly from gameplay code.
    /// </summary>
    [Export] public Conversation? StartOnReady { get; set; }

    public ConversationState State { get; private set; } = ConversationState.Idle;

    public Conversation? Current { get; private set; }

    public DialogueLine? CurrentLine =>
        Current is { } c && _lineIndex >= 0 && _lineIndex < c.Lines.Count
            ? c.Lines[_lineIndex]
            : null;

    private int _lineIndex = -1;

    public override void _Ready()
    {
        base._Ready();

        if (StartOnReady is not null)
        {
            // Defer so listeners (HUD, etc.) have a frame to wire up their signal handlers.
            CallDeferred(nameof(Start), StartOnReady);
        }
    }

    /// <summary>
    /// Begins a new conversation. If one is already in progress, it is silently ended first
    /// (so a choice's <see cref="DialogueChoice.NextConversation"/> can chain cleanly).
    /// Runs <see cref="Conversation.Validate"/> first and forwards any warnings to the
    /// editor Output panel — mismatched cues, missing species, etc. surface here.
    /// </summary>
    public void Start(Conversation conversation)
    {
        ArgumentNullException.ThrowIfNull(conversation);

        foreach (string warning in conversation.Validate())
        {
            GD.PushWarning($"[Conversation '{conversation.ConversationId}'] {warning}");
        }

        if (Current is not null)
        {
            EndCurrent();
        }

        Current = conversation;
        _lineIndex = -1;
        State = ConversationState.Presenting;
        EmitSignal(SignalName.ConversationStarted, conversation);

        Advance();
    }

    /// <summary>
    /// Advances to the next line. No-op when awaiting a choice — call <see cref="SelectChoice"/>
    /// instead. Ends the conversation when there are no more lines.
    /// </summary>
    public void Advance()
    {
        if (Current is null)
        {
            return;
        }

        if (State == ConversationState.AwaitingChoice)
        {
            GD.PushWarning(
                $"ConversationManager.Advance() ignored while AwaitingChoice — call SelectChoice() instead.");
            return;
        }

        _lineIndex++;

        if (_lineIndex >= Current.Lines.Count)
        {
            EndCurrent();
            return;
        }

        DialogueLine line = Current.Lines[_lineIndex];
        EmitSignal(SignalName.LineAdvanced, line);

        State = line.Choices.Count > 0
            ? ConversationState.AwaitingChoice
            : ConversationState.Presenting;

        if (State == ConversationState.AwaitingChoice)
        {
            EmitSignal(SignalName.ChoicesPresented, line.Choices);
        }
    }

    /// <summary>
    /// Selects choice <paramref name="index"/> on the currently presented line. If the choice
    /// has a <see cref="DialogueChoice.NextConversation"/>, that conversation begins; otherwise
    /// the current conversation continues to the next line.
    /// </summary>
    public void SelectChoice(int index)
    {
        if (State != ConversationState.AwaitingChoice || CurrentLine is null)
        {
            GD.PushWarning(
                $"ConversationManager.SelectChoice({index}) called outside AwaitingChoice (state={State}).");
            return;
        }

        Array<DialogueChoice> choices = CurrentLine.Choices;
        if (index < 0 || index >= choices.Count)
        {
            GD.PushError(
                $"ConversationManager.SelectChoice({index}) out of range [0,{choices.Count}).");
            return;
        }

        DialogueChoice choice = choices[index];
        EmitSignal(SignalName.ChoiceSelected, choice);

        if (choice.NextConversation is { } next)
        {
            Start(next);
        }
        else
        {
            // Move past the choice line we just resolved.
            State = ConversationState.Presenting;
            Advance();
        }
    }

    private void EndCurrent()
    {
        string endedId = Current?.ConversationId ?? "";
        Current = null;
        _lineIndex = -1;
        State = ConversationState.Finished;
        EmitSignal(SignalName.ConversationEnded, endedId);
    }
}
