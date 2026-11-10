namespace CyberSecurity_Awareness_Chatbot.Bot
{
    public class ActivityLog
    {
        private const int PageSize = 5;

        private readonly List<string> _entries = new List<string>();
        private int _pagesShown = 1;

        public void Add(string description)
        {
            _entries.Add($"{DateTime.Now:dd MMM HH:mm} - {description}");
        }

        public string ShowRecent()
        {
            _pagesShown = 1;
            return BuildText();
        }

        public string ShowMore()
        {
            _pagesShown++;
            return BuildText();
        }

        private string BuildText()
        {
            if (_entries.Count == 0)
            {
                return "I haven't done anything for you yet. Try adding a task or starting the quiz!";
            }

            int count = Math.Min(_pagesShown * PageSize, _entries.Count);
            string text = "Here's a summary of recent actions:";

            for (int i = 0; i < count; i++)
            {
                text += $"\n{i + 1}. {_entries[_entries.Count - 1 - i]}";
            }

            if (count < _entries.Count)
            {
                text += "\nType 'show more' to see older actions.";
            }
            return text;
        }
    }
}