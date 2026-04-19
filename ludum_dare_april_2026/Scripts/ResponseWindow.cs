using System.Collections.Generic;
using Godot;
using Godot.Collections;
using TonePathDict = System.Collections.Generic.Dictionary<Tone, string>;
using ToneTextureDict = System.Collections.Generic.Dictionary<Tone, global::Godot.Texture2D>;

/// <summary>
/// Response panel UI.
///
/// During an active conversation line with choices, shows up to 3 response buttons.
/// After a date ends, hides responses and shows a "Next Date" button so the player
/// explicitly controls progression.
/// </summary>
public partial class ResponseWindow : Control
{
    private Button response1;
    private Button response2;
    private Button response3;
    private Button nextDateButton;

    private TextureRect icon1;
    private TextureRect icon2;
    private TextureRect icon3;

    private GameManager _gameManager;
    private ConversationManager _conversationManager;

    private static readonly TonePathDict ToneIconPaths = new()
    {
        { Tone.Affectionate, "res://Assets/Art/toneAffectionate.png" },
        { Tone.Aggressive, "res://Assets/Art/toneAggression.png" },
        { Tone.Intellectual, "res://Assets/Art/toneIntellectual.png" },
    };

    private readonly ToneTextureDict _toneIconCache = new();

    public override void _Ready()
    {
        base._Ready();

        // Buttons live inside a VBoxContainer so they auto-distribute vertical space.
        response1 = GetNode<Button>("VBoxContainer/Response_1");
        response2 = GetNode<Button>("VBoxContainer/Response_2");
        response3 = GetNode<Button>("VBoxContainer/Response_3");
        nextDateButton = GetNode<Button>("VBoxContainer/NextDateButton");

        icon1 = GetNode<TextureRect>("VBoxContainer/Response_1/Icon_1");
        icon2 = GetNode<TextureRect>("VBoxContainer/Response_2/Icon_2");
        icon3 = GetNode<TextureRect>("VBoxContainer/Response_3/Icon_3");

        // Disable keyboard focus so spacebar / enter doesn't accidentally re-trigger a button.
        response1.FocusMode = FocusModeEnum.Click;
        response2.FocusMode = FocusModeEnum.Click;
        response3.FocusMode = FocusModeEnum.Click;
        nextDateButton.FocusMode = FocusModeEnum.Click;

        CacheToneIcons();

        // Resolve managers via singleton instead of scene-tree name lookups.
        _gameManager = GameManager.Instance;
        _conversationManager = _gameManager?.Conversations;

        if (_gameManager == null || _conversationManager == null)
        {
            GD.PrintErr("ResponseWindow: GameManager or ConversationManager not available.");
            return;
        }

        response1.Pressed += () => _conversationManager.SelectChoice(0);
        response2.Pressed += () => _conversationManager.SelectChoice(1);
        response3.Pressed += () => _conversationManager.SelectChoice(2);
        nextDateButton.Pressed += OnNextDatePressed;

        _conversationManager.ChoicesPresented += OnChoicesPresented;
        _conversationManager.ConversationStarted += OnConversationStarted;
        _conversationManager.ConversationEnded += OnConversationEnded;

        HideNextDateButton();
        GD.Print("ResponseWindow connected to ConversationManager");
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        if (_conversationManager != null)
        {
            _conversationManager.ChoicesPresented -= OnChoicesPresented;
            _conversationManager.ConversationStarted -= OnConversationStarted;
            _conversationManager.ConversationEnded -= OnConversationEnded;
        }
    }

    private void CacheToneIcons()
    {
        _toneIconCache.Clear();
        foreach (KeyValuePair<Tone, string> pair in ToneIconPaths)
        {
            Texture2D texture = GD.Load<Texture2D>(pair.Value);
            if (texture != null)
            {
                _toneIconCache[pair.Key] = texture;
            }
        }
    }

    private void OnConversationStarted(Conversation _conversation)
    {
        HideNextDateButton();
    }

    private void OnChoicesPresented(Array<DialogueChoice> choices)
    {
        HideNextDateButton();

        SetResponseSlot(response1, icon1, choices, 0);
        SetResponseSlot(response2, icon2, choices, 1);
        SetResponseSlot(response3, icon3, choices, 2);
    }

    private void SetResponseSlot(Button button, TextureRect icon, Array<DialogueChoice> choices, int index)
    {
        bool hasChoice = choices.Count > index;
        button.Visible = hasChoice;
        icon.Visible = hasChoice;

        if (!hasChoice)
        {
            button.Text = "";
            icon.Texture = null;
            return;
        }

        DialogueChoice choice = choices[index];
        button.Text = choice.Text;
        SetToneIcon(icon, choice.Tone);
    }

    private void OnConversationEnded(string _conversationId)
    {
        // Replace responses with a single progression button.
        response1.Visible = false;
        response2.Visible = false;
        response3.Visible = false;
        icon1.Visible = false;
        icon2.Visible = false;
        icon3.Visible = false;

        nextDateButton.Visible = true;

        if (_gameManager == null)
        {
            nextDateButton.Text = "Continue";
            nextDateButton.Disabled = true;
            GD.Print("[ResponseWindow] Conversation ended, but GameManager is null.");
            return;
        }

        if (_gameManager.HasPendingNextDate)
        {
            nextDateButton.Text = "Next Date";
            nextDateButton.Disabled = false;
            GD.Print("[ResponseWindow] Showing Next Date button.");
        }
        else if (_gameManager.RunComplete)
        {
            nextDateButton.Text = "Run Complete";
            nextDateButton.Disabled = true;
            GD.Print("[ResponseWindow] Run complete; no next date.");
        }
        else
        {
            nextDateButton.Text = "Continue";
            nextDateButton.Disabled = true;
            GD.Print("[ResponseWindow] Conversation ended with no pending next date.");
        }
    }

    private void OnNextDatePressed()
    {
        if (_gameManager == null || !_gameManager.HasPendingNextDate)
        {
            GD.Print("[ResponseWindow] Next Date pressed, but no pending date is available.");
            return;
        }

        // Prevent double-click race while next conversation starts.
        nextDateButton.Disabled = true;
        GD.Print("[ResponseWindow] Next Date pressed; requesting progression.");
        _gameManager.ContinueToNextDate();
    }

    private void HideNextDateButton()
    {
        nextDateButton.Visible = false;
        nextDateButton.Disabled = true;
    }

    private void SetToneIcon(TextureRect textureRect, Tone tone)
    {
        if (tone == Tone.None || !_toneIconCache.TryGetValue(tone, out Texture2D texture))
        {
            textureRect.Texture = null;
            return;
        }

        textureRect.Texture = texture;
    }
}
