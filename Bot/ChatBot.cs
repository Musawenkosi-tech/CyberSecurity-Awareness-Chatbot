using System.Text.RegularExpressions;
using CyberSecurity_Awareness_Chatbot.Models;

namespace CyberSecurity_Awareness_Chatbot.Bot
{
    /// <summary>
    /// The chatbot's "brain". It takes text in and gives text out, and never
    /// touches the UI. That separation lets the GUI (and later features) reuse it.
    /// </summary>
    public class ChatBot
    {
        public const string DefaultResponse =
            "I didn't quite understand that. Could you rephrase?";

        // Created ONCE and reused. Making a new Random() on every message can
        // return the same "random" number when messages arrive close together.
        private readonly Random _random = new Random();

        // --- Conversation flow: what the bot remembers between messages ---
        private string? _currentTopic;   // e.g. "phishing" (null until a topic is discussed)
        private string? _lastResponse;   // so we don't repeat the same answer twice in a row

        // Phrases that mean "stay on the same topic"
        private static readonly string[] FollowUpWords =
            { "another", "more", "explain", "again", "next", "continue" };

        // Phrases that show the user is confused or unsure
        private static readonly string[] ConfusionPhrases =
            { "confused", "don't understand", "dont understand", "what do you mean", "unclear", "not sure what" };

        /// <summary>The topic currently being discussed (used later for memory and the GUI).</summary>
        public string? CurrentTopic => _currentTopic;

        // --- Memory: what the bot remembers about the user ---
        private User? _user;        // set by the window once the user has entered their name
        private int _replyCount;    // used to decide when to add a personal recall line

        // Phrases that mean "this is something I'm interested in"
        private static readonly string[] InterestPhrases =
            { "interested in", "i like", "i love", "favourite", "favorite", "i care about", "i want to learn about" };

        // Personal tip shown later: "As someone interested in {topic}, {tip}"
        private readonly Dictionary<string, string> _recallTips = new()
        {
            { "password", "you might want to check that your most important accounts each have a unique password." },
            { "phishing", "try pausing before you click links in any email that asks you to act urgently." },
            { "browsing", "take a minute to make sure your browser is up to date." },
            { "malware", "it's worth running a full security scan on your devices this week." },
            { "privacy", "you might want to review the privacy settings on your accounts." },
            { "scam", "double-check any urgent request for money through an official channel." },
            { "ransomware", "now is a good time to back up your important files." },
            { "social engineering", "stay alert for unexpected requests that ask for codes or passwords." }
        };

        /// <summary>Gives the bot the current user so it can remember and use their details.</summary>
        public void SetUser(User user)
        {
            _user = user;
        }

        // Topic keyword -> LIST of possible answers. The bot picks one at random.
        // Adding a topic = one new entry. Adding variety = one more string in a list.
        private readonly Dictionary<string, List<string>> _topics = new()
        {
            { "password", new List<string>
                {
                    "Use a strong, unique password for every account. Avoid personal details like your birthday or pet's name.",
                    "A password manager can create and store strong passwords for you, so you only remember one master password.",
                    "Turn on multi-factor authentication wherever you can. Even if a password leaks, attackers still can't get in.",
                    "Longer is stronger: a passphrase of four or more random words is harder to crack than a short, complex password."
                }
            },
            { "phishing", new List<string>
                {
                    "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                    "Check the sender's real email address, not just the display name. Phishing addresses often have small misspellings.",
                    "Hover over a link before clicking it. If the web address looks odd or doesn't match the company, don't click.",
                    "Urgent wording like 'act now or your account will be closed' is a classic phishing trick. Slow down and verify."
                }
            },
            { "browsing", new List<string>
                {
                    "Look for HTTPS and the padlock icon before entering any personal information on a website.",
                    "Keep your browser and security software up to date so known weaknesses are patched.",
                    "Only download files and apps from official, trusted sources. Unknown downloads are a common way malware spreads.",
                    "Avoid doing banking or entering sensitive details on public Wi-Fi unless you're using a trusted VPN."
                }
            },
            { "malware", new List<string>
                {
                    "Malware is malicious software, such as viruses, spyware and trojans, designed to damage your device or steal data.",
                    "Keep your operating system and antivirus updated. Many infections use weaknesses that updates already fix.",
                    "Don't open unexpected email attachments, even from people you know. Their account may have been hacked.",
                    "If your device suddenly slows down or shows pop-ups you didn't expect, run a full security scan."
                }
            },
            { "privacy", new List<string>
                {
                    "Online privacy means controlling who can see your personal information. Review your account privacy settings regularly.",
                    "Think before you post. Details like your school, workplace or location can help criminals target you.",
                    "Check which permissions your apps have. A torch app doesn't need your contacts or microphone.",
                    "Use different email addresses for important accounts and for newsletters, to limit what a data leak exposes."
                }
            },
            { "scam", new List<string>
                {
                    "Scams create pressure: urgent messages, prizes that seem too good to be true, or fake 'bank' calls. Slow down.",
                    "Never pay or share details under pressure. Contact the organisation through its official number or website instead.",
                    "If someone asks you to pay with gift cards, crypto or a rushed bank transfer, treat it as a red flag.",
                    "Talk to someone you trust before acting. Scammers rely on you making quick decisions alone."
                }
            },
            { "ransomware", new List<string>
                {
                    "Ransomware locks or encrypts your files and demands payment to unlock them.",
                    "Back up important files regularly, ideally offline or in a separate cloud account, so you can restore them without paying.",
                    "Keep software updated and never open unexpected attachments. They are a common way ransomware gets in.",
                    "If you're hit by ransomware, disconnect from the network straight away and get help before doing anything else."
                }
            },
            { "social engineering", new List<string>
                {
                    "Social engineering tricks people, rather than computers, into giving up access or information.",
                    "Be wary of anyone who suddenly asks for passwords or codes, even if they claim to be IT support or a colleague.",
                    "Verify the person's identity through a separate, trusted channel before sharing anything sensitive.",
                    "Attackers use trust, fear and urgency. If a request feels rushed or unusual, pause and check."
                }
            }
        };

        // General tips for when the user just asks for "a tip"
        private readonly List<string> _generalTips = new()
        {
            "Think before you click. If a message creates urgency or asks for personal details, stop and verify it first.",
            "Keep your devices and apps updated. Updates often fix security holes.",
            "Use unique passwords and turn on multi-factor authentication.",
            "Back up your important files regularly in case something goes wrong.",
            "Lock your device whenever you step away, even for a minute."
        };

        /// <summary>Returns true if the user wants to leave the chat.</summary>
        public bool IsExitCommand(string? input)
        {
            return ContainsWord(input, "exit") || ContainsWord(input, "quit") || ContainsWord(input, "bye");
        }

        /// <summary>Works out the bot's reply to whatever the user typed.</summary>
        public string GetResponse(string? input)
        {
            // Input validation: empty or whitespace-only entries
            if (string.IsNullOrWhiteSpace(input))
            {
                return "I didn't catch that. How can I help you today?";
            }

            string message = input.Trim().ToLowerInvariant();

            // Basic conversation questions
            if (message.Contains("how are you"))
            {
                return $"I'm well, thanks{NameSuffix()}! Which cyber topic would you like to learn about to stay safe online today?";
            }

            if (message.Contains("purpose") || message.Contains("what do you do") || message.Contains("define cyber"))
            {
                return "My purpose is to teach you about cybersecurity and help you stay safe online.";
            }

            if (message.Contains("what can i ask") || message.Contains("what can you") || ContainsWord(message, "help"))
            {
                return "You can ask me about: " + string.Join(", ", _topics.Keys) +
                       ". You can also ask for a cybersecurity tip.";
            }

            // Memory check: "what do you remember about me?"
            if (message.Contains("remember") || message.Contains("know about me") || message.Contains("what have we talked"))
            {
                return DescribeMemory();
            }

            // Memory store: "I'm interested in privacy"
            if (_user != null && InterestPhrases.Any(phrase => message.Contains(phrase)))
            {
                string? interest = FindTopic(message);
                if (interest != null)
                {
                    _user.FavouriteTopic = interest;
                    _currentTopic = interest;
                    return $"Great! I'll remember that you're interested in {interest}. It's a crucial part of staying safe online.";
                }
            }

            // 1. Topic keywords: remember the topic, then pick ONE random answer from its list
            foreach (KeyValuePair<string, List<string>> topic in _topics)
            {
                if (message.Contains(topic.Key))
                {
                    _currentTopic = topic.Key;
                    _user?.RememberTopic(topic.Key);
                    return WithRecall(PickRandom(topic.Value));
                }
            }

            // 2. Confusion: explain the current topic again, differently
            if (ConfusionPhrases.Any(phrase => message.Contains(phrase)))
            {
                if (_currentTopic != null)
                {
                    return "No problem, let me explain it another way. " + PickRandom(_topics[_currentTopic]);
                }
                return "No worries! Which topic would you like me to explain? " + TopicList();
            }

            // 3. Follow-ups ("tell me more", "another tip"): stay on the current topic
            if (FollowUpWords.Any(word => ContainsWord(message, word)))
            {
                if (_currentTopic != null)
                {
                    return WithRecall(PickRandom(_topics[_currentTopic]));
                }
                if (message.Contains("tip"))
                {
                    return PickRandom(_generalTips);
                }
                return "Happy to go further! Which topic should we start with? " + TopicList();
            }

            // 4. A plain request for a tip
            if (message.Contains("tip"))
            {
                return PickRandom(_generalTips);
            }

            // Unsupported query
            return DefaultResponse;
        }

        /// <summary>
        /// Picks one item from a list at random, avoiding the answer we just gave
        /// so "another tip" never repeats itself.
        /// </summary>
        private string PickRandom(List<string> options)
        {
            string choice;
            do
            {
                // Next(n) returns 0 up to n-1, which matches the valid list positions
                choice = options[_random.Next(options.Count)];
            }
            while (options.Count > 1 && choice == _lastResponse);

            _lastResponse = choice;
            return choice;
        }

        /// <summary>Finds which known topic (if any) appears in the message.</summary>
        private string? FindTopic(string message)
        {
            foreach (string key in _topics.Keys)
            {
                if (message.Contains(key))
                {
                    return key;
                }
            }
            return null;
        }

        /// <summary>", Name" when we know the user's name, otherwise nothing.</summary>
        private string NameSuffix()
        {
            return _user != null ? $", {_user.Name}" : "";
        }

        /// <summary>
        /// Every third reply, adds a personal line based on the user's favourite topic.
        /// </summary>
        private string WithRecall(string reply)
        {
            _replyCount++;

            string? favourite = _user?.FavouriteTopic;
            if (favourite != null && _replyCount % 3 == 0 && _recallTips.TryGetValue(favourite, out string? tip))
            {
                return reply + $"\n\nAs someone interested in {favourite}, {tip}";
            }
            return reply;
        }

        /// <summary>Summarises everything the bot remembers about the user.</summary>
        private string DescribeMemory()
        {
            if (_user == null)
            {
                return "I don't know much about you yet. Tell me your name first!";
            }

            string summary = $"Here's what I remember: your name is {_user.Name}.";

            summary += _user.FavouriteTopic != null
                ? $" You're interested in {_user.FavouriteTopic}."
                : " You haven't told me a favourite topic yet. Try saying \"I'm interested in privacy\".";

            if (_user.TopicsDiscussed.Count > 0)
            {
                summary += " We've talked about: " + string.Join(", ", _user.TopicsDiscussed) + ".";
            }
            return summary;
        }

        /// <summary>The available topics as readable text.</summary>
        private string TopicList()
        {
            return "You can ask about: " + string.Join(", ", _topics.Keys) + ".";
        }

        /// <summary>
        /// Whole-word match, so "exit" matches "exit" but not "exited" or "existing".
        /// </summary>
        private static bool ContainsWord(string? text, string word)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            return Regex.IsMatch(text, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase);
        }
    }
}