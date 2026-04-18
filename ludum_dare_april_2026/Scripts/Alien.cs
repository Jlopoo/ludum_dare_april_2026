using Godot;

#nullable enable

/// <summary>
/// One date — a named individual of a particular <see cref="Species"/>.
/// Attached to a <see cref="Conversation"/> so <see cref="CueResolver"/> knows
/// whose lookup table to consult when interpreting a <see cref="Cue"/>.
///
/// Future-proofing: a <c>HomePlanet</c> field will be added here when planetary
/// customs land; it'll layer on top of the species mapping rather than replace it.
/// </summary>
[GlobalClass]
public partial class Alien : Resource
{
    [Export] public string Name { get; set; } = "";

    [Export] public Species? Species { get; set; }
}
