namespace CyberSecurity_Awareness_Chatbot.Models
{
    public class QuizQuestion
    {
        public string Text { get; }
        public List<string> Options { get; }
        public int CorrectIndex { get; }
        public string Explanation { get; }

        public QuizQuestion(string text, List<string> options, int correctIndex, string explanation)
        {
            Text = text;
            Options = options;
            CorrectIndex = correctIndex;
            Explanation = explanation;
        }
    }
}