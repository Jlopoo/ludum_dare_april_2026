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
/// Delete this node (and this file) once you've wired conversations into your real gameplay flow.
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

        _manager.Start(StartConversation ?? SampleConversations.AlienOneIntro());
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
                _manager.Start(StartConversation ?? SampleConversations.AlienOneIntro());
                GetViewport().SetInputAsHandled();
                break;
        }
    }

    private static void OnConversationStarted(Conversation conversation)
    {
        GD.Print($"=== Conversation '{conversation.ConversationId}' started ===");
    }

    private static void OnLineAdvanced(DialogueLine line)
    {
        GD.Print($"{line.SpeakerId}: {line.Text}");
    }

    private static void OnChoicesPresented(Array<DialogueChoice> choices)
    {
        GD.Print("  Press a number key:");
        for (int i = 0; i < choices.Count; i++)
        {
            GD.Print($"    [{i + 1}] {choices[i].Text}  (affection {FormatDelta(choices[i].AffectionDelta)})");
        }
    }

    private static void OnChoiceSelected(DialogueChoice choice)
    {
        GD.Print($"  -> chose '{choice.Text}'");
    }

    private static void OnConversationEnded(string conversationId)
    {
        GD.Print($"=== Conversation '{conversationId}' ended (Space to restart) ===");
    }

    private static string FormatDelta(int delta) => delta >= 0 ? $"+{delta}" : delta.ToString();
}
