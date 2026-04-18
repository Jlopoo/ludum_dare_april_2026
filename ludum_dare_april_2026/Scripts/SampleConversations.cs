using Godot;
using Godot.Collections;

#nullable enable

/// <summary>
/// Hand-built conversations for prototyping and tests. In production these would live as
/// .tres Resource files authored in the Godot editor — building them in code is just the
/// fastest way to validate that the data shape and ConversationManager flow work end-to-end
/// before you commit to authoring tooling.
/// </summary>
public static class SampleConversations
{
    public static Conversation AlienOneIntro()
    {
        return new Conversation
        {
            ConversationId = "alien1_intro",
            Lines = new Array<DialogueLine>
            {
                new DialogueLine
                {
                    SpeakerId = "alien1",
                    Text = "Greetings, Earth-organism. State your reproductive status.",
                    Choices = new Array<DialogueChoice>(),
                },
                new DialogueLine
                {
                    SpeakerId = "player",
                    Text = "...uh.",
                    Choices = new Array<DialogueChoice>
                    {
                        new DialogueChoice
                        {
                            Text = "Single, painfully so.",
                            AffectionDelta = +3,
                            ResponseText = "I'm glad to hear that. I'm single too.",
                            NextConversation = null,
                        },
                        new DialogueChoice
                        {
                            Text = "What's it to you?",
                            AffectionDelta = -2,
                            ResponseText = "I'm sorry to hear that. I'm single too.",
                            NextConversation = null,
                        },
                        new DialogueChoice
                        {
                            Text = "Define 'reproductive status'.",
                            AffectionDelta = 0,
                            ResponseText = "I'm not sure what you mean. I'm single too.",
                            NextConversation = null,
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "alien1",
                    Text = "Acceptable. We shall now exchange... pleasantries.",
                    Choices = new Array<DialogueChoice>(),
                },
            },
        };
    }

    /// <summary>
    /// Sample that exercises the full cue system end-to-end. Squilliam is a Floopian;
    /// for Floopians, <see cref="Cue.BubbleEars"/> demands an <see cref="Tone.Affectionate"/>
    /// response and <see cref="Cue.AntennaTwitch"/> demands an <see cref="Tone.Intellectual"/>
    /// one. (You can flip these around freely — that's just one species' rules.)
    /// </summary>
    public static Conversation FloopianIntro()
    {
        Species alien2 = new()
        {
            Name = "Alien 2",
            Description = "Alien 2 is handsome squidward.",
            CueTones = new Array<CueToneEntry>
            {
                new() { Cue = Cue.BloodshotEyes,    Tone = Tone.Affectionate },
                new() { Cue = Cue.WidePupils,    Tone = Tone.Aggressive },
                new() { Cue = Cue.TongueOut, Tone = Tone.Intellectual },
            },
        };
        Alien squilliam = new() { Name = "Squilliam", Species = alien2 };

        return new Conversation
        {
            ConversationId = "alien2_intro",
            Alien = squilliam,
            Lines = new Array<DialogueLine>
            {
                new DialogueLine
                {
                    SpeakerId = "Squilliam",
                    Cue = Cue.BloodshotEyes,
                    Text = "Nice to meet you, I'm Squilliam from Jumbilio.",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "Nice to meet you, they say the most beautiful entities are from Jumbilio.",
                            Tone = Tone.Affectionate,
                            ResponseText = "Do they really say that?",
                        },
                        new()
                        {
                            Text = "Jumbilians really know how to get me going.",
                            Tone = Tone.Aggressive,
                            ResponseText = "*Squilliam recoils sharply.* Okay then.",
                        },
                        new()
                        {
                            Text = "Nice to make your acquaintance, Squilliam, is Jumbilio in the Groopsnop solar system?",
                            Tone = Tone.Intellectual,
                            ResponseText = "It is, but it's a long way from here.",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Squilliam",
                    Cue = Cue.WidePupils,
                    Text = "*Squilliam's pupils widen.* I confess I have studied your species' courtship lit.",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "Oh? Which works in particular?",
                            Tone = Tone.Intellectual,
                            ResponseText = "Squilliam brightens, antennae twitching faster.",
                        },
                        new()
                        {
                            Text = "Cute. Now kiss me, you fool.",
                            Tone = Tone.Affectionate,
                            ResponseText = "Squilliam's antennae freeze mid-twitch.",
                        },
                        new()
                        {
                            Text = "That's creepy. I was trying to be candid here.",
                            Tone = Tone.Aggressive,
                            ResponseText = "Squilliam's antennae droop.",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Squilliam",
                    Text = "Mm. The encounter is... tolerable.",
                    Choices = new Array<DialogueChoice>(),
                },
            },
        };
    }
}
