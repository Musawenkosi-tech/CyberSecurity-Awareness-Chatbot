# Cybersecurity Awareness Chatbot (Part 2: WPF GUI)

**Module:** PROG6221 – Programming 2A | **Author:** Musawenkosi Mhlambi

A Windows desktop chatbot (WPF, C#, .NET 8) that teaches South African citizens about staying safe online. Part 2 turns the Part 1 console bot into a graphical application and adds keyword recognition, random responses, conversation flow, memory and sentiment detection.

**Video presentation:** <!-- TODO: paste your unlisted YouTube link here -->

---

## Features

| Feature | What it does |
|---|---|
| **GUI** | WPF window with a title header, an ASCII art banner, chat bubbles (bot on the left, you on the right) and a typing effect |
| **Clear and History** | **Clear** wipes the chat on screen; **History** shows every message from the session with timestamps, even after a clear |
| **Voice greeting** | A recorded WAV welcome message plays when the app starts |
| **Personalised greeting** | Asks for your name (with validation) and uses it in the conversation |
| **Keyword recognition** | Recognises 8 topics: password, phishing, browsing, malware, privacy, scam, ransomware, social engineering |
| **Random responses** | Every topic has several answers stored in a list, and the bot picks one at random without repeating the last one |
| **Conversation flow** | Follow-ups like "tell me more", "another tip" or "I don't understand" stay on the current topic |
| **Memory and recall** | Remembers your name, your favourite topic and the topics you've asked about, and brings them up later |
| **Sentiment detection** | Spots *worried*, *frustrated* and *curious* messages, responds with empathy and shares a tip straight away |
| **Error handling** | Empty or unknown input gets a friendly default reply and never crashes the app |

---

## Requirements

- Windows 10 or 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 with the **.NET desktop development** workload (recommended)

## How to run

**Visual Studio**
1. Clone the repository: `git clone <your-repository-url>`
2. Open the `.sln` file.
3. Press **F5** (or **Build → Rebuild Solution**, then **Start**).

**Command line**
```
cd CyberSecurity_Awareness_Chatbot
dotnet run
```

The voice greeting file (`Audio/welcome.wav`) is copied next to the program automatically when you build. Turn your volume on to hear it.

---

## How to use the chatbot

1. Wait for the welcome message, then type your **name** and press **Enter** or click **Send**.
2. Ask about any topic, for example `phishing`, `tell me about passwords` or `what is ransomware`.
3. Ask follow-ups like `tell me more`, `another tip` or `I'm confused`.
4. Tell the bot what you care about: `I'm interested in privacy`.
5. Ask `what do you remember about me?` to see what it has stored.
6. Type `exit` to leave.

### Example conversation

```
Bot: Welcome to the Cybersecurity Awareness Bot! Please enter your name.
You: Musa
Bot: Hello, Musa! Ask me about passwords, phishing, safe browsing, ...

Musa: phishing
Bot: Check the sender's real email address, not just the display name. ...

Musa: tell me more
Bot: Hover over a link before clicking it. ...

Musa: I'm interested in privacy
Bot: Great! I'll remember that you're interested in privacy. It's a crucial part of staying safe online.

Musa: I'm worried about online scams
Bot: It's completely understandable to feel that way. ...
     Tip: Never pay or share details under pressure. ...

Musa: what do you remember about me?
Bot: Here's what I remember: your name is Musa. You're interested in privacy. We've talked about: phishing, scam.
```

---

## Project structure

```
CyberSecurity_Awareness_Chatbot/
├── App.xaml / App.xaml.cs              Application entry point
├── MainWindow.xaml / .xaml.cs          The chat window (layout, colours, typing effect)
├── Bot/
│   └── ChatBot.cs                      The chatbot "brain" (no UI code)
├── Models/
│   └── User.cs                         The user's name and what the bot remembers
├── Services/
│   └── AudioService.cs                 Plays the voice greeting
└── Audio/
    └── welcome.wav                     Recorded greeting
```

### How the code works

- **`MainWindow`** only handles what the user sees and clicks. It sends text to `ChatBot` and displays the reply.
- **`ChatBot.GetResponse`** passes the message through a list of **delegate** handlers (`ResponseHandler`): small talk, memory, sentiment, topic, confusion, follow-up and general tip. The first handler that returns a reply wins; a handler returns `null` when the message isn't for it.
- **Collections:** topics are stored in a `Dictionary<string, List<string>>` (keyword → possible answers), and tips in a `List<string>`. One `Random` instance picks answers.
- **Memory:** `ChatBot` keeps the current topic for follow-ups, and `User` stores the favourite topic and a list of topics discussed.
- **Sentiment:** two dictionaries map each mood to its trigger words and to an empathetic reply.

---

## Continuous Integration

A GitHub Actions workflow (`.github/workflows/dotnet.yml`) restores and builds the project on every push.

<!-- TODO: add a screenshot of a green CI run here, e.g. ![CI passing](docs/ci-green.png) -->

## Releases

| Version | Notes |
|---|---|
| **v2.0** | <!-- TODO: e.g. WPF GUI, keywords, random responses, flow, memory, sentiment --> |
| **v2.1** | <!-- TODO: e.g. GUI polish: coloured messages, dividers, typing effect --> |

## Part 1

The original console version is preserved under the `part1-final` tag / `v1.0` release.