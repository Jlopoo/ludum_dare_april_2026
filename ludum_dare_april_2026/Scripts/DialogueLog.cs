using Godot;
using Microsoft.VisualBasic;
using System.Collections.Generic;

public partial class DialogueLog : VBoxContainer
{

	public static DialogueLog Instance {get; private set;}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	private readonly Dictionary<string, string> _speakerColors = new()
	{
		{"Alice", "00ffff"},
		{"Bob", "ffff00"},
	};

	public void AddEntry(string speaker, string message)
	{
		var entry = new RichTextLabel();
		entry.BbcodeEnabled = true;
		entry.FitContent = true;
		entry.ScrollActive = false;
		entry.AutowrapMode = TextServer.AutowrapMode.Word;
		entry.SizeFlagsHorizontal = SizeFlags.Fill;

		string color = _speakerColors.TryGetValue(speaker, out var c) ? c : "ffffff";
		entry.Text = $"[b][color={color}]{speaker}:[/color][/b] {message}";

		AddChild(entry);
		ScrollToBottom();
	}


	private async void ScrollToBottom()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		var scroll = GetParent<ScrollContainer>();
		scroll.ScrollVertical = (int)scroll.GetVScrollBar().MaxValue;
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
