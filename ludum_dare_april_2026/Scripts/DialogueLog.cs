using System.Collections.Generic;
using Godot;

/// <summary>
/// Scrolling dialogue history shown inside the DialogueWindow. Lives as the script
/// on a VBoxContainer; new entries append as <see cref="RichTextLabel"/> children.
///
/// Owns no state beyond display history. Speaker color is looked up from a small
/// table; unknown speakers fall back to white. Register additional colors with
/// <see cref="RegisterSpeakerColor"/> from gameplay code (e.g. when a new alien
/// appears, give them a distinct hue).
/// </summary>
public partial class DialogueLog : VBoxContainer
{
	public static DialogueLog Instance { get; private set; }

	private readonly Dictionary<string, string> _speakerColors = new()
	{
		{ "You",      "8ee0ff" },
		{ "System",   "888888" },
		{ "Squilliam", "ff8866" },
		{ "Big Candace", "ff8866" },
		{ "Lmanaga", "ff8866" },
	};

	public override void _Ready()
	{
		Instance = this;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (Instance == this) Instance = null;
	}

	public void RegisterSpeakerColor(string speaker, string hexColor)
	{
		_speakerColors[speaker] = hexColor;
	}

	public void AddEntry(string speaker, string message)
	{
		var entry = new RichTextLabel
		{
			BbcodeEnabled = true,
			FitContent = true,
			ScrollActive = false,
			AutowrapMode = TextServer.AutowrapMode.Word,
			SizeFlagsHorizontal = SizeFlags.Fill,
		};

		// Bump the default font size so log entries are legible in the bigger window.
		entry.AddThemeFontSizeOverride("normal_font_size", 16);
		entry.AddThemeFontSizeOverride("bold_font_size", 16);

		string color = _speakerColors.TryGetValue(speaker, out var c) ? c : "ffffff";
		entry.Text = $"[b][color={color}]{speaker}:[/color][/b] {message}";

		AddChild(entry);
		ScrollToBottom();
	}

	private async void ScrollToBottom()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		var scroll = GetParent<ScrollContainer>();
		if (scroll != null)
		{
			scroll.ScrollVertical = (int)scroll.GetVScrollBar().MaxValue;
		}
	}
}
