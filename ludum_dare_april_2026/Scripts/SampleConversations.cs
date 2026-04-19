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

    // Lmanga (an Alien-1 species).
    public static Conversation LmangaIntro()
    {
        // Preload textures once. GD.Load caches resources, so repeated calls across
        // multiple Conversation builders are cheap.
        Texture2D portraitDefault = GD.Load<Texture2D>("res://Assets/Art/alien1_raw.png");
        Texture2D imgPink = GD.Load<Texture2D>("res://Assets/Art/alien1_negative.png");
        Texture2D imgTan = GD.Load<Texture2D>("res://Assets/Art/alien1_neutral.png");
        Texture2D imgBlue = GD.Load<Texture2D>("res://Assets/Art/alien1_positive.png");

        Species alien1 = new()
        {
            Name = "Alien 1",
            Description = "Alien 1 is a flat noodle monster.",
            DefaultPortrait = portraitDefault,
            CueTones = new Array<CueToneEntry>
            {
                new() { Cue = Cue.Blue, Tone = Tone.Affectionate, CueImage = imgBlue },
                new() { Cue = Cue.Pink,    Tone = Tone.Aggressive,   CueImage = imgPink },
                new() { Cue = Cue.Tan,     Tone = Tone.Intellectual, CueImage = imgTan },
            },
        };
        Alien Lmanaga = new() { Name = "Lmanaga", Species = alien1 };

        return new Conversation
        {
            ConversationId = "alien1_intro",
            Alien = Lmanaga,
            Lines = new Array<DialogueLine>
            {
                new DialogueLine
                {
                    SpeakerId = "Lmanaga",
                    Cue = Cue.Tan,
                    Text = "greetings -- creature -- I Lmanaga -- love intended? -- postulate",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "A being from another dimension? I am a whore for space, time, and whatever's in between.",
                            Tone = Tone.Affectionate,
                            ResponseText = "strange -- strong will -- social blunder -- too sensitive",
                        },
                        new()
                        {
                            Text = "I'm going to grip those noodles and swing them silly.",
                            Tone = Tone.Aggressive,
                            ResponseText = "pain expected -- Lmanaga fear -- slide",
                        },
                        new()
                        {
                            Text = "Pleasure to meet you Lmanaga. I am indeed seeking a life partner.",
                            Tone = Tone.Intellectual,
                            ResponseText = "intrigue -- Lmanaga is special -- creature special -- postulate",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Lmanaga",
                    Cue = Cue.Pink,
                    Text = "error detected -- creature harms -- violence -- capacity? ",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "My species prefer to answer conflict with rational discourse.",
                            Tone = Tone.Intellectual,
                            ResponseText = "translation -- cowards -- liars -- politicians",
                        },
                        new()
                        {
                            Text = "I'm a lover not a fighter.",
                            Tone = Tone.Affectionate,
                            ResponseText = "crumble -- weakness -- pliable nature -- slime ball",
                        },
                        new()
                        {
                            Text = "Watch your tone you worthless pile of spaghetti.",
                            Tone = Tone.Aggressive,
                            ResponseText = "intriguing -- strong -- warrior culture -- willpower",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Lmanaga",
                    Cue = Cue.Blue,
                    Text = "conclusion -- primitive apes -- unthinking rabble -- unfit breeding partner",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "Trying to get into my pants by appealing to my wild side?",
                            Tone = Tone.Aggressive,
                            ResponseText = "suspected outcome -- eradication inevitable -- genes worthless",
                        },
                        new()
                        {
                            Text = "My kind have sailed the stars and mastered technology! We are the masters of our domain.",
                            Tone = Tone.Intellectual,
                            ResponseText = "postulate -- conclusion wrong -- vast capacity -- love?",
                        },
                        new()
                        {
                            Text = "I'm sorry you think that. Maybe we can hold hands?",
                            Tone = Tone.Affectionate,
                            ResponseText = "no hands -- disgusting implements -- distusting apes",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Lmananga",
                    Text = "analysis concluded -- grations",
                    Choices = new Array<DialogueChoice>(),
                },
            },
        };
    }

    /// <summary>
    /// Squilliam from Jumbilio (an Alien-2 species). Demonstrates the cue system
    /// end-to-end: each alien line carries a <see cref="Cue"/>, the species table
    /// resolves it to the expected response <see cref="Tone"/>, and the matching
    /// portrait texture rides along so <see cref="GameManager"/> can swap the
    /// character display in sync with the line.
    /// </summary>
    public static Conversation SquilliamIntro()
    {
        // Preload textures once. GD.Load caches resources, so repeated calls across
        // multiple Conversation builders are cheap.
        Texture2D portraitDefault = GD.Load<Texture2D>("res://Assets/Art/alien2_raw.png");
        Texture2D imgBloodshotEyes = GD.Load<Texture2D>("res://Assets/Art/alien2_signal1.png");
        Texture2D imgWidePupils    = GD.Load<Texture2D>("res://Assets/Art/alien2_signal2.png");
        Texture2D imgTongueOut     = GD.Load<Texture2D>("res://Assets/Art/alien2_signal3.png");

        Species alien2 = new()
        {
            Name = "Alien 2",
            Description = "Alien 2 is handsome squidward.",
            DefaultPortrait = portraitDefault,
            CueTones = new Array<CueToneEntry>
            {
                new() { Cue = Cue.BloodshotEyes, Tone = Tone.Affectionate, CueImage = imgBloodshotEyes },
                new() { Cue = Cue.WidePupils,    Tone = Tone.Aggressive,   CueImage = imgWidePupils },
                new() { Cue = Cue.TongueOut,     Tone = Tone.Intellectual, CueImage = imgTongueOut },
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
                    Cue = Cue.TongueOut,
                    Text = "*Squilliam's tongue lolls out, pensive.* Tell me — how do your kind decide whom to love?",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "Honestly, vibes. Maybe pheromones. We don't think about it.",
                            Tone = Tone.Aggressive,
                            ResponseText = "Squilliam blinks slowly, unimpressed.",
                        },
                        new()
                        {
                            Text = "We tend to pair via revealed preference and repeated exposure.",
                            Tone = Tone.Intellectual,
                            ResponseText = "Squilliam nods, tongue retracting in approval.",
                        },
                        new()
                        {
                            Text = "Truthfully? When we feel safe with someone.",
                            Tone = Tone.Affectionate,
                            ResponseText = "Squilliam's tongue trembles slightly. *That is... lovely.*",
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

    // Candace, or 'Big Candace' as she's known in the streets (an Alien-3 species).
    public static Conversation BigCandaceIntro()
    {
        // Preload textures once. GD.Load caches resources, so repeated calls across
        // multiple Conversation builders are cheap.
        Texture2D portraitDefault = GD.Load<Texture2D>("res://Assets/Art/alien3_raw_nobackground.png");
        Texture2D imgBubbles = GD.Load<Texture2D>("res://Assets/Art/alien3_raw_nobackground_signal.png");
        Texture2D imgGreenStars = GD.Load<Texture2D>("res://Assets/Art/alien3_raw_nobackground_signal2.png");
        Texture2D imgPinkSkin = GD.Load<Texture2D>("res://Assets/Art/alien3_raw_nobackground_signal3.png");

        Species alien3 = new()
        {
            Name = "Alien 3",
            Description = "Alien 3 is a huge tit woman with a fake tan and three eyes.",
            DefaultPortrait = portraitDefault,
            CueTones = new Array<CueToneEntry>
            {
                new() { Cue = Cue.Bubbles, Tone = Tone.Affectionate, CueImage = imgBubbles },
                new() { Cue = Cue.PinkSkin,    Tone = Tone.Aggressive,   CueImage = imgPinkSkin },
                new() { Cue = Cue.GreenStars,     Tone = Tone.Intellectual, CueImage = imgGreenStars },
            },
        };
        Alien BigCandace = new() { Name = "Big Candace", Species = alien3 };

        return new Conversation
        {
            ConversationId = "alien3_intro",
            Alien = BigCandace,
            Lines = new Array<DialogueLine>
            {
                new DialogueLine
                {
                    SpeakerId = "Big Candace",
                    Cue = Cue.Bubbles,
                    Text = "He-Lo gorgeous! BREEE-YAAAARRRKKK! Don't see many cuts of meat like you on Skinnaronk",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "It's a pleasure, Skinnaronk is a beuatiful little moon full of romance.",
                            Tone = Tone.Affectionate,
                            ResponseText = "Hot mama I'm feeling loose! BREEE-YAAAARRRKKK!",
                        },
                        new()
                        {
                            Text = "Y'know, Skinnaronkians are known for their libido.",
                            Tone = Tone.Aggressive,
                            ResponseText = "*Big Candace recoils sharply.* You don't own me.",
                        },
                        new()
                        {
                            Text = "Salutations, Candace, I've never met any Skinnaronkians before.",
                            Tone = Tone.Intellectual,
                            ResponseText = "It's Big Candace to you chump. You may never meet another. *licks lips*",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Big Candace",
                    Cue = Cue.PinkSkin,
                    Text = "BIG GIRL'S WORKIN UP SOMETHIN' FIERCE. BREEEEEEEEEEE-YAAAAAAARRRRRKKK!",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "How interesting! Mind of I record you for further study.",
                            Tone = Tone.Intellectual,
                            ResponseText = "You have killed my vibe beta-cuck.",
                        },
                        new()
                        {
                            Text = "I am ready to be stepped on and loved from afar!",
                            Tone = Tone.Affectionate,
                            ResponseText = "CLENCH YOUR CHEEKS. MAMA'S READY TO DOMINATE.",
                        },
                        new()
                        {
                            Text = "You're a dirty little minx.",
                            Tone = Tone.Aggressive,
                            ResponseText = "Dirty talking is great, but Mama was hoping for some love.",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Big Candace",
                    Cue = Cue.GreenStars,
                    Text = "Ooooo! I'm getting a tingly sensation. Tell me something interesting!",
                    Choices = new Array<DialogueChoice>
                    {
                        new()
                        {
                            Text = "I'm thinking about railing you against the wall.",
                            Tone = Tone.Aggressive,
                            ResponseText = "Coming on too strong handsome. I might take my talents elsewhere.",
                        },
                        new()
                        {
                            Text = "Skinnaronk is one of 30 moons around Fleebarous, but it's the only one with an ocean!",
                            Tone = Tone.Intellectual,
                            ResponseText = "OH YEA! Sensational for my mind and my body.",
                        },
                        new()
                        {
                            Text = "I am in search of true love! You could be the one cosmo in a galaxy of black holes.",
                            Tone = Tone.Affectionate,
                            ResponseText = "I have friends, hun, and I don't appreciate you calling them holes.",
                        },
                    },
                },
                new DialogueLine
                {
                    SpeakerId = "Big Candace",
                    Text = "Thanks for the convo, but I gotta run. Try to find me in the streets.",
                    Choices = new Array<DialogueChoice>(),
                },
            },
        };
    }

}
