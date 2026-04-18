using Godot;
using Godot.Collections;

#nullable enable

/// <summary>
/// THIS IS A TEST BOOTSTRAP FOR THE CONVERSATION SYSTEM. IT IS NOT PART OF THE FINAL GAME, SO YOU CAN SAFELY DELETE IT.
/// Drop-in test driver: when added to a scene tree, it spawns a <see cref="ConversationManager"/>
/// child, subscribes to every signal with a console log, and lets you drive the conversation
/// from the keyboard:
///   <list type="bullet">
///     <item><description><b>Space / Enter</b> — advance the current line.</description></item>
///     <item><description><b>1–9</b> — pick a choice when one is presented.</description></item>
///     <item><description><b>Space / Enter</b> after a conversation ends — restart it.</description></item>
///   </list>
/// Also pokes <see cref="GameState"/> with the scored aura delta so you can see the meter
/// move as you test. Delete this file once you've wired conversations into your real flow.
/// </summary>
public partial class ConversationTestBootstrap : Node
{
    /// <summary>
    /// Optional editor-assigned Conversation. Drag a <c>.tres</c> Conversation resource
    /// onto this slot in the inspector to run it instead of the hardcoded sample.
    /// </summary>
    [Export] public Conversation? StartConversation { get; set; }

    private ConversationManager _manager = null!;

    public override void _Ready()
    {
        base._Ready();

        _manager = new ConversationManager { Name = "ConversationManager" };
        AddChild(_manager);

        _manager.ConversationStarted += OnConversationStarted;
        _manager.LineAdvanced += OnLineAdvanced;
        _manager.ChoicesPresented += OnChoicesPresented;
        _manager.ChoiceSelected += OnChoiceSelected;
        _manager.ConversationEnded += OnConversationEnded;

        _manager.Start(StartConversation ?? DefaultSample());
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event is not InputEventKey key || !key.Pressed || key.Echo)
        {
            return;
        }

        switch (_manager.State)
        {
            case ConversationState.Presenting
                when key.Keycode == Key.Space || key.Keycode == Key.Enter:
                _manager.Advance();
                GetViewport().SetInputAsHandled();
                break;

            case ConversationState.AwaitingChoice
                when key.Keycode >= Key.Key1 && key.Keycode <= Key.Key9:
                int index = (int)(key.Keycode - Key.Key1);
                _manager.SelectChoice(index);
                GetViewport().SetInputAsHandled();
                break;

            case ConversationState.Finished
                when key.Keycode == Key.Space || key.Keycode == Key.Enter:
                _manager.Start(StartConversation ?? DefaultSample());
                GetViewport().SetInputAsHandled();
                break;
        }
    }

    private static Conversation DefaultSample() => SampleConversations.FloopianIntro();

    private static void OnConversationStarted(Conversation conversation)
    {
        string alienBlurb = conversation.Alien is { } a
            ? $" ({a.Name}, {a.Species?.Name ?? "?"})"
            : "";
        GD.Print($"=== Conversation '{conversation.ConversationId}' started{alienBlurb} ===");
    }

    private void OnLineAdvanced(DialogueLine line)
    {
        string cueBlurb = line.Cue != Cue.None ? $"  [signal: {line.Cue}]" : "";
        GD.Print($"{line.SpeakerId}: {line.Text}{cueBlurb}");
    }

    private void OnChoicesPresented(Array<DialogueChoice> choices)
    {
        Tone? expected = CueResolver.Resolve(_manager.Current, _manager.CurrentLine);
        GD.Print(expected is { } e
            ? $"  (expected tone: {e}) — press a number key:"
            : "  (no scored cue on this line) — press a number key:");

        for (int i = 0; i < choices.Count; i++)
        {
            DialogueChoice c = choices[i];
            int delta = expected is { } ex ? ToneScoring.AuraDelta(c.Tone, ex) : 0;
            GD.Print($"    [{i + 1}] ({c.Tone}) {c.Text}  →  aura {FormatDelta(delta)}");
        }
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        Tone? expected = CueResolver.Resolve(_manager.Current, _manager.CurrentLine);
        int delta = expected is { } ex ? ToneScoring.AuraDelta(choice.Tone, ex) : 0;

        GD.Print($"  -> chose '{choice.Text}' (tone={choice.Tone})");
        if (!string.IsNullOrEmpty(choice.ResponseText))
        {
            GD.Print($"     {choice.ResponseText}");
        }

        if (delta != 0)
        {
            // Drive the live aura meter so the player sees the impact immediately.
            // GameState's autoload registration is what makes this lookup work.
            GameState? state = GameState.Instance;
            if (state is not null)
            {
                state.ApplyAffectionDelta(delta);
                GD.Print($"     aura {FormatDelta(delta)}  (now {state.Aura:F0}/{state.MaxAura:F0})");
            }
            else
            {
                GD.Print($"     aura {FormatDelta(delta)}  (GameState autoload not available)");
            }
        }
    }

    private static void OnConversationEnded(string conversationId)
    {
        GD.Print($"=== Conversation '{conversationId}' ended (Space to restart) ===");
    }

    private static string FormatDelta(int delta) => delta >= 0 ? $"+{delta}" : delta.ToString();
}
