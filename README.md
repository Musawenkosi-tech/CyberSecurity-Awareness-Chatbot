# Cybersecurity Awareness Chatbot POE

**Module:** PROG6221 – Programming 2A
**Student:** Musawenkosi Mhlambi

## What is this project?

This is a chatbot that teaches South African citizens how to stay safe online. It started as a console app in Part 1, became a desktop app with a window in Part 2, and in Part 3 I added tasks, reminders, a quiz and smarter understanding of what the user types.

It is built with **C#**, **WPF** and **.NET 8**. The tasks are saved in a **MySQL** database.

## What can it do?

**From Part 1 and Part 2**
- Asks for your name and greets you, with a voice greeting when the app starts.
- Answers questions about 8 topics: passwords, phishing, safe browsing, malware, privacy, scams, ransomware and social engineering.
- Gives a different answer each time you ask, and follows up when you say "tell me more".
- Remembers your name and your favourite topic.
- Notices if you sound worried, frustrated or curious and replies kindly.
- Has Clear and History buttons, and shows the bot's messages on the left and yours on the right.

**New in Part 3**
- **Task assistant:** add, list, complete and delete cybersecurity tasks. They are saved in MySQL.
- **Reminders:** say things like "in 3 days", "tomorrow" or "15 December" and the bot reminds you when the time comes.
- **Activity log:** ask "What have you done for me?" to see a list of what the bot did.
- **Quiz:** 12 questions (multiple choice and true/false) with feedback after every answer and a score at the end.
- **Natural language processing (NLP):** the bot understands different ways of asking for the same thing.

## How to run it

You need **Windows**, **Visual Studio 2022** (with the ".NET desktop development" option) and **MySQL**. MySQL is only needed for tasks. The rest of the chatbot works without it.

**1. Get the code**
```
git clone https://github.com/Musawenkosi-tech/CyberSecurity-Awareness-Chatbot.git
```

**2. Create the database**
Open MySQL Workbench and run these one at a time:
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

**3. Add your database password**
1. Find `dbsettings.example.json` in the project folder.
2. Make a copy and rename the copy to `dbsettings.json`.
3. Open it and replace the text after `"Password":` with your own MySQL password.

The real `dbsettings.json` is never uploaded to GitHub, so my password stays private.

**4. Start the app**
Open the solution in Visual Studio and press **F5**. If MySQL is not set up yet, the app still opens and the grey note at the top tells you what went wrong.

## How to use it

Type your name first. After that you can try:

| Type this | What happens |
|---|---|
| `phishing` or `how do I make a strong password` | The bot gives a tip about that topic |
| `tell me more` | Another tip about the same topic |
| `add task - review privacy settings` | Saves a task and asks if you want a reminder |
| `remind me to update my password tomorrow` | Saves the task with a reminder |
| `show tasks` | Lists your tasks |
| `complete task 1` / `delete task 2` | Marks a task as done or removes it |
| `quiz` or the **Quiz** button | Starts the quiz |
| `what have you done for me?` | Shows the activity log |
| `exit` | Ends the chat |

### Example chat

```
You:  Musa
Bot:  Hello, Musa! Ask me about passwords, phishing, ... or 'quiz'.

Musa: add task - review privacy settings
Bot:  Task added: 'Review privacy settings'. Review account privacy settings to ensure your data is protected.
      Would you like a reminder?

Musa: yes, in 3 days
Bot:  Got it! I'll remind you in 3 days.

Musa: challenge me
Bot:  Great idea! Let's test your cybersecurity knowledge. Opening the quiz now...
```

## How does the NLP work?

I kept it simple so I can explain it. It is not machine learning, just two ideas:

1. **Rewrite rules.** The bot cleans the message (lowercase, no punctuation) and then uses patterns to turn different phrasings into a command it already knows. For example:
   - "don't let me forget to back up my files" becomes "remind me to back up my files"
   - "challenge me" becomes "quiz"
   - "what's on my list" becomes "show tasks"
2. **Synonym lists.** Each topic has a list of related words, so "virus", "trojan" and "spyware" all lead to the malware topic.

If the bot still does not understand, it suggests things you can ask instead of just giving up.

## How is the code organised?

```
Bot/        the chatbot's "brain" (chat replies, tasks, quiz questions, activity log, NLP)
Models/     simple data classes (User, TaskItem, QuizQuestion)
Services/   database code (settings and queries) and the voice greeting
*.xaml      the windows: main chat, tasks, quiz
```

A few things I want to point out:
- **Delegates:** every message goes through a list of small handler methods. The first one that has an answer replies.
- **Collections:** topics are stored in a `Dictionary`, and tasks, quiz questions and the activity log are stored in `List`s.
- **Safe SQL:** the database code uses parameters, so what the user types can never break the query.

## GitHub, CI and releases

Every time I push, GitHub Actions builds the project to check that it still compiles.

<!-- TODO: add a screenshot of the green CI run, for example ![CI passing](docs/ci-green.png) -->

| Release | What was added |
|---|---|
| v3.0 | Task assistant with MySQL, reminders and the activity log |
| v3.1 | Cybersecurity quiz |
| v3.2 | NLP and final polish |

Older parts: Part 1 is the `part1` tag and Part 2 is the `part2-final` tag.
