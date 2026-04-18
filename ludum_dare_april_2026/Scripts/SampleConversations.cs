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
}
