# Signals — Ludum Dare April 2026

> Theme: **SIGNALS**
>
> You're an alien looking for love. You're clueless about the signals other
> species give. At a 9-date speed-dating event, you have to learn which of each
> species' signals mean "good", "neutral", or "bad" — and say the right things
> to fill your aura meter to 80% or more. Find a match and get a baby.

Built with **Godot 4.6** + **C# / .NET 8**.

---

## Running the project

1. Install [Godot 4.6 (.NET build)](https://godotengine.org/).
2. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download).
3. Open `ludum_dare_april_2026/project.godot` in Godot. First open will import
   the `.cs` scripts and regenerate `.godot/`.
4. Press **F5** to run. The included example date (Vorbax the Zorblaxian)
   should start after clicking **Start Dating**.

---

## Architecture

The game is split into four clearly-separated layers. Each layer has its own
folder under `ludum_dare_april_2026/Scripts/`:

| Layer | Folder | Role |
|---|---|---|
| **Data** | `Scripts/Data/` | Strongly-typed `Resource` classes. Edited as `.tres` files in the editor. No behavior. |
| **Core** | `Scripts/Core/` | Autoload singletons: `GameManager`, `SceneManager`, `AudioManager`. Own run state and side effects. |
| **Gameplay** | `Scripts/Gameplay/` | `DateController`, `AuraMeter`. The rules of a single date. |
| **UI** | `Scripts/UI/` | Menu / summary / journal screens. Scene-root scripts only. |

### Data model

```
DateLibrary
└── DateScenario[]
    ├── AlienName (string)
    ├── Species (AlienSpecies)
    │   ├── SpeciesId, SpeciesName, HomePlanet
    │   ├── Portrait, ThemeColor
    │   ├── GoodSignal   (SignalCue)
    │   ├── NeutralSignal(SignalCue)
    │   └── BadSignal    (SignalCue)
    └── Prompts (DialoguePrompt[])
        ├── AlienLine (string)
        ├── ResponseA (ResponseOption = Text + ReactionType)
        ├── ResponseB (ResponseOption)
        └── ResponseC (ResponseOption)
```

Every one of those types is a `[GlobalClass] Resource`, so designers can
create/edit them in the Godot inspector without touching code. Scripts are the
schema; `.tres` files are the content.

### Scoring

`ReactionType` maps to a score:

| Reaction | Score |
|---|---|
| Bad     | 0.0 |
| Neutral | 0.5 |
| Good    | 1.0 |

`AuraMeter.CurrentAura = sum(scores) / responses_given`. A date succeeds when
the final aura meets `DateScenario.SuccessThresholdOverride` (or the game-wide
`GameState.DefaultSuccessThreshold` of **0.80**).

### Run flow

```
main.tscn → main_menu.tscn
                ↓ (Start Dating)
            date.tscn ←─────────────┐
                ↓ (prompts done)    │ (more dates?)
          date_summary.tscn ────────┘
                ↓ (run complete)
          game_complete.tscn
                ↓
            main_menu.tscn
```

`GameManager` (autoload) owns `GameState` and `DateLibrary`. `SceneManager`
(autoload) centralizes scene paths and transitions. No scene ever hard-codes
another scene's path.

---

## Folder layout

```
ludum_dare_april_2026/
├── project.godot                          Godot project config + autoloads
├── ludum_dare_april_2026.csproj           .NET project (nullable on, .NET 8)
├── Data/                                  CONTENT (.tres files)
│   ├── date_library.tres                  Ordered list of 9 dates
│   ├── Species/
│   │   └── zorblaxian.tres                Example species
│   └── Dates/
│       └── date_01_vorbax.tres            Example date scenario
├── Scenes/                                Godot scenes (.tscn)
│   ├── main.tscn                          Boot → main_menu
│   ├── main_menu.tscn
│   ├── options_menu.tscn
│   ├── HUD.tscn                           Embedded in date.tscn
│   ├── date.tscn                          Actual gameplay
│   ├── date_summary.tscn
│   ├── game_complete.tscn
│   └── signal_journal.tscn
└── Scripts/                               C# code
    ├── Main.cs, MainMenu.cs, Hud.cs, OptionsMenu.cs
    ├── Core/
    │   ├── GameManager.cs                 Autoload: run state + DateLibrary
    │   ├── SceneManager.cs                Autoload: scene transitions
    │   ├── AudioManager.cs                Autoload: SFX/music
    │   └── GameState.cs                   Pure POCO: state data
    ├── Data/
    │   ├── ReactionType.cs                enum + extension scoring
    │   ├── SignalCue.cs                   Resource
    │   ├── AlienSpecies.cs                Resource
    │   ├── ResponseOption.cs              Resource
    │   ├── DialoguePrompt.cs              Resource
    │   ├── DateScenario.cs                Resource
    │   └── DateLibrary.cs                 Resource
    ├── Gameplay/
    │   ├── DateController.cs              Orchestrates date.tscn
    │   └── AuraMeter.cs                   ProgressBar + scoring
    └── UI/
        ├── DateSummary.cs
        ├── GameComplete.cs
        └── SignalJournal.cs
```

---

## Authoring content

### Create a new alien species

1. In the Godot editor, right-click inside `Data/Species/` → **New Resource…**
2. Search for `AlienSpecies`, click **Create**.
3. Fill out the fields in the inspector:
   - `SpeciesId` — stable unique key (e.g. `"glimmerfolk"`). **Don't change after shipping.**
   - `SpeciesName`, `HomePlanet`, `Description`, `ThemeColor`
   - `Portrait` — drag a texture in
   - `GoodSignal`, `NeutralSignal`, `BadSignal` — click **[empty]** →
     **New SignalCue** for each, then fill description / glyph / color.
4. Save the resource (`Ctrl-S`) into `Data/Species/<species>.tres`.

### Create a new date

1. Right-click inside `Data/Dates/` → **New Resource…** → `DateScenario`.
2. Fill `AlienName`, drag a species into `Species`.
3. Write `IntroLine`, `SuccessLine`, `FailureLine`.
4. Expand `Prompts`, click **Add Element**, then **New DialoguePrompt**.
5. For each prompt:
   - Write `AlienLine` (what the alien says).
   - Fill `ResponseA` / `B` / `C` with **New ResponseOption**, set `Text` and
     `Reaction` (Bad / Neutral / Good).
6. Save as `Data/Dates/date_NN_<alienname>.tres`.

### Add the date to the run

1. Open `Data/date_library.tres` in the inspector.
2. Expand `Dates`, click **Add Element**, drag your new `.tres` file into the
   slot.
3. Arrange the order by dragging elements.

That's it — press F5 and the new date plays in order.

### Tuning difficulty

- Edit `SuccessThresholdOverride` on a specific date to make it harder/easier
  than the default 80%.
- Edit `GameState.DefaultSuccessThreshold` (code) to change the game-wide
  default.
- Total dates is `GameState.TotalDates = 9`. Change in code if you want a
  shorter/longer run.

---

## Code conventions

- **Nullable reference types are enabled.** New code should use `?` on
  reference types that can legitimately be null; otherwise avoid null.
- **Namespaces** match folder structure:
  `LudumDareApril2026.{Core|Data|Gameplay|UI}`.
- **Autoloads are accessed via the static `Instance` property** (e.g.
  `GameManager.Instance.State`). Never `GetNode("/root/GameManager")`.
- **Content never lives in code.** All dialogue, names, and signal
  descriptions live in `.tres` files under `Data/`.
- **No magic strings for scene paths.** Use `SceneManager.GoToX()` methods or
  the `SceneManager.XScene` constants.
- **UI scripts look up children via `GetNodeOrNull<T>(path)`** in `_Ready`,
  with an explicit null check and `GD.PushError` if a required node is
  missing. This is more reliable than Godot 4's `[Export]` Node-reference
  bindings, which have serialization quirks for C# properties of Node types.
  The node paths are declared once at the top of `_Ready`; if the scene is
  rearranged, update both in one place.

---

## License

MIT — see `LICENSE`.
