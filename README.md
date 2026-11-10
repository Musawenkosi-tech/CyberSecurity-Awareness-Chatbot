# Cybersecurity Awareness Chatbot (Part 3)

**Module:** PROG6221 – Programming 2A | **Author:** Musawenkosi Mhlambi

A Windows desktop chatbot (WPF, C#, .NET 8) that teaches South African citizens how to stay safe online. Part 1 was the console bot, Part 2 added the GUI, keyword recognition, memory and sentiment, and **Part 3** adds a MySQL task assistant with reminders, a cybersecurity quiz, an activity log and simple natural language processing (NLP).


---

## Features

| Feature | What it does |
|---|---|
| **GUI** | WPF window with a header, chat bubbles (bot on the left, you on the right), a typing effect, and **Clear**, **History**, **Tasks** and **Quiz** buttons |
| **Voice greeting** | A recorded WAV welcome message plays on start-up |
| **Keyword recognition** | Recognises 8 topics: password, phishing, browsing, malware, privacy, scam, ransomware, social engineering |
| **Random responses, flow and memory** | Several answers per topic, follow-ups like "tell me more", and it remembers your name and favourite topic |
| **Sentiment detection** | Spots *worried*, *frustrated* and *curious* messages and replies with empathy and a tip |
| **Task assistant (MySQL)** | Add, list, complete and delete cybersecurity tasks from the chat or the Tasks window. Tasks are stored in a MySQL database |
| **Reminders** | Set a reminder in words ("in 3 days", "tomorrow", "next week", "15 December"). A timer checks every 30 seconds and the bot posts a message when a task is due |
| **Quiz mini-game** | 12 questions (multiple choice and true/false), one at a time, instant feedback with an explanation, score tracking and a final message |
| **Activity log** | Records what the bot has done. Ask "What have you done for me?" and type "show more" to see older entries |
| **NLP simulation** | Understands different ways of asking for the same thing, using regex rewrite rules and synonym lists |

---

## Requirements

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 with the **.NET desktop development** workload
- [MySQL Community Server 8](https://dev.mysql.com/downloads/mysql/) and MySQL Workbench (needed for the task assistant only; the rest of the chatbot works without it)

---

## Setup

### 1. Clone the repository
```
git clone https://github.com/Musawenkosi-tech/CyberSecurity-Awareness-Chatbot.git
```

### 2. Create the database
Open MySQL Workbench, connect to your local server and run these statements **one at a time** (Ctrl+Enter):

```sql
CREATE DATABASE IF NOT EXISTS cyber_chatbot;

CREATE TABLE IF NOT EXISTS cyber_chatbot.tasks (
    id            INT AUTO_INCREMENT PRIMARY KEY,
    title         VARCHAR(200) NOT NULL,
    description   VARCHAR(500),
    reminder_date DATETIME NULL,
    is_completed  TINYINT(1) NOT NULL DEFAULT 0,
    created_at    DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
```

### 3. Add your database settings
The password is kept out of Git on purpose.
1. Copy `dbsettings.example.json` and rename the copy to `dbsettings.json` in the project folder (next to the `.csproj`).
2. Open it and put your own MySQL password in:

```json
{
  "Server": "localhost",
  "Port": 3306,
  "Database": "cyber_chatbot",
  "User": "root",
  "Password": "YOUR_PASSWORD_HERE"
}
```

`dbsettings.json` is listed in `.gitignore` and is copied next to the program when you build.

### 4. Run
Open the solution in Visual Studio and press **F5**. Turn your volume on to hear the greeting.

If MySQL is not running or `dbsettings.json` is missing, the chatbot still starts. It shows a note that tasks are unavailable.

---

## How to use the chatbot

1. Enter your **name** when asked.
2. Ask about a topic: `phishing`, `how do I make a strong password`, `what is ransomware`.
3. Manage tasks and reminders:
   - `add task - review privacy settings`
   - `remind me to update my password tomorrow`
   - `show tasks`, `complete task 1`, `delete task 2`
4. Take the quiz: click **Quiz** or type `quiz`.
5. Ask `what have you done for me?` to see the activity log.
6. Type `exit` to leave.

### Example conversation

```
You:  Musa
Bot:  Hello, Musa! Ask me about passwords, phishing, ... or 'quiz'.

Musa: Add a task - review my privacy settings
Bot:  Task added: 'Review my privacy settings'. Review account privacy settings to ensure your data is protected.
      Would you like a reminder?

Musa: yes, in 3 days
Bot:  Got it! I'll remind you in 3 days.

Musa: don't let me forget to back up my files tomorrow
Bot:  Reminder set for 'Back up my files' tomorrow.

Musa: challenge me
Bot:  Great idea! Let's test your cybersecurity knowledge. Opening the quiz now...
Bot:  Quiz complete! You scored 10 out of 12. Great job! You're a cybersecurity pro!

Musa: what have you done for me?
Bot:  Here's a summary of recent actions:
      1. Quiz completed - scored 10/12
      2. ...
```

### NLP examples

| You type | The bot understands it as |
|---|---|
| `Don't let me forget to back up my files` | a reminder request |
| `Jot down review my privacy settings` | add a task |
| `What's on my list?` | show tasks |
| `I finished task 3` | complete task 3 |
| `Challenge me` / `test me` | start the quiz |
| `Is my laptop infected with a virus?` | the malware topic |
| `Is wifi at the mall safe?` | the browsing topic |

---

## Project structure

```
CyberSecurity_Awareness_Chatbot/
├── App.xaml / App.xaml.cs               Application entry point
├── MainWindow.xaml / .xaml.cs           Chat window, reminder timer, quiz and tasks buttons
├── TasksWindow.xaml / .xaml.cs          Table view of tasks (complete, delete, refresh)
├── QuizWindow.xaml / .xaml.cs           Quiz screen with feedback and score
├── Bot/
│   ├── ChatBot.cs                       The chatbot "brain" (delegate handler pipeline)
│   ├── TaskAssistant.cs                 Task and reminder commands, date parsing
│   ├── ActivityLog.cs                   Activity log with "show more"
│   ├── NlpEngine.cs                     Regex rewrite rules and topic synonyms
│   └── QuizBank.cs                      The 12 quiz questions
├── Models/
│   ├── User.cs                          Name, favourite topic, topics discussed
│   ├── TaskItem.cs                      One task from the database
│   └── QuizQuestion.cs                  One quiz question
├── Services/
│   ├── AudioService.cs                  Voice greeting
│   ├── DatabaseSettings.cs              Reads dbsettings.json
│   └── TaskRepository.cs                All MySQL queries (parameterised)
└── Audio/welcome.wav
```

### How the code works

- **Delegates:** `ChatBot.GetResponse` passes the message through a list of `ResponseHandler` delegates (tasks, activity log, quiz, small talk, social, memory, sentiment, topic, confusion, follow-up, general tip). The first handler that returns a reply wins; a handler returns `null` when the message isn't for it.
- **NLP:** `NlpEngine` first cleans the text. A list of regex rules then rewrites many phrasings into simple commands (for example "jot down X" becomes "add task to X"). Each topic also has a synonym list. Both are plain lists and dictionaries, so the logic is easy to follow.
- **Tasks:** `TaskAssistant` finds the intent (add, list, complete, delete), extracts the title with a regex and parses dates such as "in 3 days" or "15 December". `TaskRepository` runs the SQL with parameters, so user text is never pasted into a query.
- **Reminders:** a `DispatcherTimer` in `MainWindow` checks the database every 30 seconds and posts a message for each due task once.
- **Quiz:** questions are stored in a `List<QuizQuestion>`, shuffled each time, and shown one at a time with a score counter.
- **Activity log:** every action adds a timestamped line to a list, shown five at a time (newest first).
- **Collections used:** `Dictionary<string, List<string>>` for topics, `List<T>` for tasks, questions and the log, and `HashSet<int>` for reminders already shown.

---

## Continuous Integration

A GitHub Actions workflow (`.github/workflows/dotnet.yml`) restores and builds the project on every push. It does not need a database.

<!-- TODO: add a screenshot of a green CI run, e.g. ![CI passing](docs/ci-green.png) -->

## Releases

| Version | Notes |
|---|---|
| **v3.0** | <!-- TODO: MySQL task assistant, reminders, Tasks window, activity log --> |
| **v3.1** | <!-- TODO: cybersecurity quiz mini-game --> |
| **v3.2** | <!-- TODO: NLP simulation, final integration and README --> |

## Previous parts

- **Part 1** (console version): tag `part1-final` / release `v1.0`
- **Part 2** (WPF GUI): release `v2.x`
