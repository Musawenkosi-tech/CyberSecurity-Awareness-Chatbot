using System.Text.RegularExpressions;

namespace CyberSecurity_Awareness_Chatbot.Bot
{
    public class NlpResult
    {
        public string Text { get; }
        public string? Intent { get; }

        public NlpResult(string text, string? intent)
        {
            Text = text;
            Intent = intent;
        }
    }

    
    public class NlpEngine
    {
        private class Rule
        {
            public string Intent { get; }
            public Regex Pattern { get; }
            public string Replacement { get; }

            public Rule(string intent, string pattern, string replacement)
            {
                Intent = intent;
                Pattern = new Regex(pattern, RegexOptions.IgnoreCase);
                Replacement = replacement;
            }
        }

        private readonly List<Rule> _rules = new List<Rule>
        {
            new Rule("reminder",
                @"\b(?:don't let me forget|dont let me forget|help me remember|make sure i remember|notify me|alert me|ping me|nudge me)(?:\s+to)?\s+",
                "remind me to "),

            new Rule("add task",
                @"\b(?:note down|jot down|write down|put on my list|add to my list)\s+(?:a\s+|that i need to\s+|to\s+)?",
                "add task to "),

            new Rule("add task",
                @"\b(?:to-do|todo|to do)\s+(?:item|list item|entry)\b",
                "task"),

            new Rule("show tasks",
                @"\b(?:what(?:'s| is| are)?\s+(?:on\s+)?my\s+(?:list|schedule|reminders|plans)|what do i (?:need|have) to do|do i have any (?:tasks|reminders)|have i got any (?:tasks|reminders)|my reminders|my to-?do list|my todo list|to-?do list)\b",
                "show tasks"),

            new Rule("complete task",
                @"\b(?:finished|completed|done with|i did|ticked off)\s+(?:with\s+)?(?:the\s+)?(?:task\s*)?#?(\d+)",
                "complete task $1"),

            new Rule("delete task",
                @"\b(?:get rid of|cancel|scrap|drop|forget about)\s+(?:the\s+)?(?:task|reminder)\s*#?(\d+)",
                "delete task $1"),

            new Rule("quiz",
                @"\b(?:challenge me|question me|quiz me|test what i know|check what i know|practice questions|play (?:a |the )?(?:game|quiz)|game time|mini.?game|test my knowledge|test me)\b",
                "quiz"),

            new Rule("activity log",
                @"\b(?:what (?:did|have) you (?:do|done)|what have you been doing|history of (?:actions|activity)|recent activity|show (?:me )?(?:my )?activity|what happened so far)\b",
                "activity log")
        };

        private readonly Dictionary<string, string[]> _topicWords = new Dictionary<string, string[]>
        {
            { "password", new[] { "password", "pasword", "passcode", "passphrase", "pwd", "login credentials", "log in details", "sign in details" } },
            { "phishing", new[] { "phishing", "phising", "fishing email", "fake email", "suspicious email", "suspicious link", "spoof", "spam email", "email scam" } },
            { "browsing", new[] { "browsing", "browser", "wifi", "wi-fi", "vpn", "https", "website", "surfing", "online shopping", "download" } },
            { "malware", new[] { "malware", "virus", "trojan", "spyware", "worm", "adware", "keylogger", "infected", "antivirus", "anti-virus" } },
            { "privacy", new[] { "privacy", "personal data", "personal information", "data protection", "tracking", "cookies", "popia", "my data" } },
            { "scam", new[] { "scam", "fraud", "swindle", "fake offer", "too good to be true", "con artist", "get rich quick" } },
            { "ransomware", new[] { "ransomware", "ransom", "locked my files", "files locked", "encrypted my files", "pay to unlock" } },
            { "social engineering", new[] { "social engineering", "social-engineering", "manipulat", "impersonat", "pretexting", "tailgating", "trick people", "fake caller" } }
        };

        public NlpResult Process(string input)
        {
            string text = input.ToLowerInvariant();
            text = text.Replace('\u2019', '\'').Replace('\u2018', '\'');
            text = Regex.Replace(text, @"[?!,;]", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();

            string? intent = null;
            foreach (Rule rule in _rules)
            {
                if (rule.Pattern.IsMatch(text))
                {
                    text = rule.Pattern.Replace(text, rule.Replacement);
                    intent ??= rule.Intent;
                }
            }

            return new NlpResult(text, intent);
        }

        public string? FindTopic(string message)
        {
            foreach (KeyValuePair<string, string[]> topic in _topicWords)
            {
                if (topic.Value.Any(word => message.Contains(word)))
                {
                    return topic.Key;
                }
            }
            return null;
        }
    }
}