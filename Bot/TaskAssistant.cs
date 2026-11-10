using System.Text.RegularExpressions;
using CyberSecurity_Awareness_Chatbot.Models;
using CyberSecurity_Awareness_Chatbot.Services;

namespace CyberSecurity_Awareness_Chatbot.Bot
{
    public class TaskAssistant
    {
        private readonly TaskRepository? _repository;
        private readonly ActivityLog _log;

        private int _pendingTaskId = -1;
        private string _pendingTitle = "";

        private readonly Dictionary<string, string> _descriptions = new()
        {
            { "privacy", "Review account privacy settings to ensure your data is protected." },
            { "password", "Update your passwords so each account has a strong, unique one." },
            { "2fa", "Turn on two-factor authentication for an extra layer of account security." },
            { "two-factor", "Turn on two-factor authentication for an extra layer of account security." },
            { "backup", "Back up your important files so you can restore them if something goes wrong." },
            { "update", "Install the latest updates to fix known security weaknesses." },
            { "antivirus", "Make sure your antivirus is running and up to date." },
            { "phishing", "Practise spotting suspicious emails and links before you click." }
        };

        private static readonly Dictionary<string, int> Months = new()
        {
            { "jan", 1 }, { "feb", 2 }, { "mar", 3 }, { "apr", 4 }, { "may", 5 }, { "jun", 6 },
            { "jul", 7 }, { "aug", 8 }, { "sep", 9 }, { "oct", 10 }, { "nov", 11 }, { "dec", 12 }
        };

        public TaskAssistant(TaskRepository? repository, ActivityLog log)
        {
            _repository = repository;
            _log = log;
        }

        public string? Handle(string message)
        {
            if (_pendingTaskId != -1)
            {
                string? answer = HandleReminderAnswer(message);
                if (answer != null)
                {
                    return answer;
                }
            }

            bool adding = IsAddRequest(message);
            bool completing = !adding && message.Contains("task") && ContainsAny(message, "complete", "done", "finished", "mark");
            bool deleting = !adding && message.Contains("task") && ContainsAny(message, "delete", "remove");
            bool listing = !adding && !completing && !deleting && message.Contains("task")
                           && ContainsAny(message, "show", "view", "list", "see", "display", "my", "what");

            if (!adding && !completing && !deleting && !listing)
            {
                return null;
            }

            if (_repository == null)
            {
                return "I can't reach the task database right now. Check that MySQL is running and dbsettings.json is set up.";
            }

            if (adding)
            {
                return AddTask(message);
            }
            if (completing)
            {
                return CompleteTask(message);
            }
            if (deleting)
            {
                return DeleteTask(message);
            }
            return ListTasks();
        }

        private string AddTask(string message)
        {
            string title = ExtractTitle(message);
            if (title == "")
            {
                return "What should the task be? For example: add task - review privacy settings";
            }

            string description = FindDescription(title);
            DateTime? reminder = ParseReminder(message);

            int id = _repository!.AddTask(title, description, reminder);
            if (id == -1)
            {
                return "Sorry, I couldn't save that task: " + _repository.LastError;
            }

            if (reminder != null)
            {
                _log.Add($"Task added: '{title}' (reminder set for {reminder:dd MMM yyyy HH:mm})");

                if (message.Contains("remind me"))
                {
                    return $"Reminder set for '{title}' {Describe(reminder.Value)}.";
                }
                return $"Task added: '{title}'. {description}\nGot it! I'll remind you {Describe(reminder.Value)}.";
            }

            _log.Add($"Task added: '{title}' (no reminder set)");
            _pendingTaskId = id;
            _pendingTitle = title;
            return $"Task added: '{title}'. {description}\nWould you like a reminder? (for example: yes, in 3 days / tomorrow / next week / 15 December, or no)";
        }

        private string? HandleReminderAnswer(string message)
        {
            bool no = Regex.IsMatch(message, @"^(no|nope|skip|not now|don't|dont)\b");
            bool yes = Regex.IsMatch(message, @"\b(yes|yeah|yep|sure|ok|okay|please)\b");
            DateTime? when = ParseReminder(message);

            if (when != null)
            {
                _repository!.SetReminder(_pendingTaskId, when.Value);
                string title = _pendingTitle;
                _log.Add($"Reminder set: '{title}' on {when:dd MMM yyyy HH:mm}");
                ClearPending();
                return $"Got it! I'll remind you {Describe(when.Value)}.";
            }

            if (no)
            {
                string title = _pendingTitle;
                ClearPending();
                return $"No problem. '{title}' is saved without a reminder.";
            }

            if (yes)
            {
                return "When should I remind you? For example: in 3 days, tomorrow, next week or 15 December.";
            }

            ClearPending();
            return null;
        }

        private string CompleteTask(string message)
        {
            int? id = FindNumber(message);
            if (id == null)
            {
                return "Which task? Say something like 'complete task 2'. Type 'show tasks' to see the numbers.";
            }

            if (_repository!.MarkCompleted(id.Value))
            {
                _log.Add($"Task #{id} marked as completed");
                return $"Nice work! Task #{id} is marked as completed.";
            }
            return $"I couldn't find task #{id}. Type 'show tasks' to see your list.";
        }

        private string DeleteTask(string message)
        {
            int? id = FindNumber(message);
            if (id == null)
            {
                return "Which task? Say something like 'delete task 2'. Type 'show tasks' to see the numbers.";
            }

            if (_repository!.DeleteTask(id.Value))
            {
                _log.Add($"Task #{id} deleted");
                return $"Task #{id} has been deleted.";
            }
            return $"I couldn't find task #{id}. Type 'show tasks' to see your list.";
        }

        private string ListTasks()
        {
            List<TaskItem> tasks = _repository!.GetAllTasks();
            if (tasks.Count == 0)
            {
                return "You have no tasks yet. Try: add task - review privacy settings";
            }

            string text = "Your tasks:";
            foreach (TaskItem task in tasks)
            {
                string status = task.IsCompleted ? "[done]" : "[ ]";
                text += $"\n#{task.Id} {status} {task.Title} (reminder: {task.ReminderText})";
            }
            return text;
        }

        private static bool IsAddRequest(string message)
        {
            bool addWord = ContainsAny(message, "add", "create", "new", "make", "set up", "set a", "set my");
            return (message.Contains("task") && addWord)
                   || message.Contains("remind me")
                   || (message.Contains("reminder") && addWord);
        }

        private static string ExtractTitle(string message)
        {
            Match match = Regex.Match(message,
                @"(remind me to|remind me|reminder to|reminder for|reminder|task to|task for|task)\s*[-:]?\s*(.*)$");
            if (!match.Success)
            {
                return "";
            }

            string title = match.Groups[2].Value;
            title = Regex.Replace(title, @"\b(on\s+)?\d{1,2}\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)[a-z]*(\s+\d{4})?\b", "");
            title = Regex.Replace(title, @"\b(on\s+)?\d{4}-\d{2}-\d{2}\b", "");
            title = Regex.Replace(title, @"\b(in\s+)?\d+\s*(hours?|days?|weeks?)\b", "");
            title = Regex.Replace(title, @"\b(tomorrow|next week)\b", "");
            title = title.Trim(' ', '.', '?', '!', '-', ':');
            title = Regex.Replace(title, @"\s+", " ");
            title = Regex.Replace(title, @"^to ", "");

            if (title.Length == 0)
            {
                return "";
            }
            return char.ToUpper(title[0]) + title.Substring(1);
        }

        private string FindDescription(string title)
        {
            string lower = title.ToLowerInvariant();
            foreach (KeyValuePair<string, string> entry in _descriptions)
            {
                if (lower.Contains(entry.Key))
                {
                    return entry.Value;
                }
            }
            return $"Cybersecurity task: {title}.";
        }

        public static DateTime? ParseReminder(string message)
        {
            Match relative = Regex.Match(message, @"(\d+)\s*(hour|day|week)");
            if (relative.Success)
            {
                int amount = int.Parse(relative.Groups[1].Value);
                string unit = relative.Groups[2].Value;
                if (unit == "hour")
                {
                    return DateTime.Now.AddHours(amount);
                }
                if (unit == "week")
                {
                    return DateTime.Now.AddDays(7 * amount);
                }
                return DateTime.Now.AddDays(amount);
            }

            Match named = Regex.Match(message, @"\b(\d{1,2})\s*(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)[a-z]*(?:\s+(\d{4}))?\b");
            if (named.Success)
            {
                int day = int.Parse(named.Groups[1].Value);
                int month = Months[named.Groups[2].Value];
                int year = named.Groups[3].Success ? int.Parse(named.Groups[3].Value) : DateTime.Now.Year;

                DateTime? date = MakeDate(year, month, day);
                if (date != null && !named.Groups[3].Success && date.Value < DateTime.Now)
                {
                    date = MakeDate(year + 1, month, day);
                }
                return date;
            }

            Match iso = Regex.Match(message, @"\b(\d{4})-(\d{2})-(\d{2})\b");
            if (iso.Success)
            {
                return MakeDate(int.Parse(iso.Groups[1].Value), int.Parse(iso.Groups[2].Value), int.Parse(iso.Groups[3].Value));
            }

            if (message.Contains("tomorrow"))
            {
                return DateTime.Now.AddDays(1);
            }
            if (message.Contains("next week"))
            {
                return DateTime.Now.AddDays(7);
            }
            return null;
        }

        private static DateTime? MakeDate(int year, int month, int day)
        {
            try
            {
                return new DateTime(year, month, day, 9, 0, 0);
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        private static string Describe(DateTime when)
        {
            int days = (when.Date - DateTime.Today).Days;
            if (days <= 0)
            {
                return $"today at {when:HH:mm}";
            }
            if (days == 1)
            {
                return "tomorrow";
            }
            if (days <= 14)
            {
                return $"in {days} days";
            }
            return $"on {when:dd MMM yyyy}";
        }

        private static int? FindNumber(string message)
        {
            Match match = Regex.Match(message, @"\d+");
            return match.Success ? int.Parse(match.Value) : null;
        }

        private static bool ContainsAny(string message, params string[] words)
        {
            return words.Any(word => message.Contains(word));
        }

        private void ClearPending()
        {
            _pendingTaskId = -1;
            _pendingTitle = "";
        }
    }
}