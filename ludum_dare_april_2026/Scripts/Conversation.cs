using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

#nullable enable

[GlobalClass]
public partial class Conversation : Resource
{
    /// <summary>
    /// Stable identifier for the conversation. Used by listeners (analytics, save data,
    /// quest tracking) and emitted with <see cref="ConversationManager.ConversationEnded"/>.
    /// </summary>
    [Export] public string ConversationId { get; set; } = "";

    /// <summary>
    /// Who is being talked to. Drives cue resolution via <see cref="CueResolver"/>.
    /// Optional so non-puzzle conversations (tutorial, narration) can omit it.
    /// </summary>
    [Export] public Alien? Alien { get; set; }

    /// <summary>
    /// End-of-date success condition. A conversation is considered successful only when
    /// the player's aura is strictly greater than this value (Aura &gt; Threshold).
    /// </summary>
    [Export] public float SuccessAuraThreshold { get; set; } = 50f;

    /// <summary>
    /// Optional line logged when the date ends successfully.
    /// </summary>
    [Export(PropertyHint.MultilineText)] public string SuccessOutro { get; set; } = "";

    /// <summary>
    /// Optional line logged when the date ends unsuccessfully.
    /// </summary>
    [Export(PropertyHint.MultilineText)] public string FailureOutro { get; set; } = "";

    [Export] public Array<DialogueLine> Lines { get; set; } = new();

    /// <summary>
    /// Walks the conversation looking for authoring mistakes that would silently
    /// break scoring at runtime — primarily lines that reference a <see cref="Cue"/>
    /// the configured <see cref="Alien"/>'s species doesn't know about.
    ///
    /// Returns a (possibly empty) list of human-readable warning strings; callers
    /// decide what to do with them. <see cref="ConversationManager.Start"/> calls
    /// this and forwards each warning to <c>GD.PushWarning</c> so issues surface
    /// the first time a conversation runs.
    /// </summary>
    public IReadOnlyList<string> Validate()
    {
        List<string> warnings = new();

        if (Lines.Count == 0)
        {
            warnings.Add("Conversation has no lines.");
            return warnings;
        }

        for (int i = 0; i < Lines.Count; i++)
        {
            DialogueLine? line = Lines[i];
            if (line is null)
            {
                warnings.Add($"Line {i}: null entry in Lines array.");
                continue;
            }

            if (line.Cue == Cue.None) continue; // unscored line — nothing to validate

            if (Alien is null)
            {
                warnings.Add($"Line {i}: has cue {line.Cue} but conversation has no Alien — cue will not be scored.");
                continue;
            }

            if (Alien.Species is null)
            {
                warnings.Add($"Line {i}: has cue {line.Cue} but alien '{Alien.Name}' has no Species assigned — cue will not be scored.");
                continue;
            }

            if (Alien.Species.ToneFor(line.Cue) is null)
            {
                string knownCues = string.Join(", ", Alien.Species.CueTones.Where(e => e is not null && e.Cue != Cue.None).Select(e => e.Cue.ToString()));
                if (string.IsNullOrEmpty(knownCues)) knownCues = "(none)";
                warnings.Add($"Line {i}: cue '{line.Cue}' is not in species '{Alien.Species.Name}' — known cues for this species: [{knownCues}].");
            }
        }

        return warnings;
    }
}
