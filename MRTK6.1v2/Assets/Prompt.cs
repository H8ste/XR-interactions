public class Prompt
{
    private PromptOption[] promptOptions;
    public PromptOption[] PromptOptions { get { return promptOptions; }}

    private string promptQuestion;
    public string PromptQuestion { get { return promptQuestion; } }

    /// <summary>
    /// Constructor for prompt.
    /// </summary>
    /// <param name="options">Possible prompt options</param>
    /// <param name="question">Question asked by prompt</param>
    public Prompt(PromptOption[] options, string question )
    {
        promptOptions = options;
        promptQuestion = question;
    }

}
