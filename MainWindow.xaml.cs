using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CyberSecurity_Awareness_Chatbot.Bot;
using CyberSecurity_Awareness_Chatbot.Models;
using CyberSecurity_Awareness_Chatbot.Services;

namespace CyberSecurity_Awareness_Chatbot
{
    /// <summary>
    /// The chat window. It only handles what the user sees and clicks;
    /// all the thinking is done by ChatBot.
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ChatBot _bot = new ChatBot();
        private readonly AudioService _audio = new AudioService();
        private User? _user;            // null until the user has entered their name
        private bool _chatEnded;        // true once the user types "exit"

        private const int MaxNameLength = 30;
        private const int TypingDelayMs = 10;   // pause between letters for the typing effect

        // Colours: one per speaker so the conversation is easy to follow
        private static readonly Brush BotColour = Brushes.LightGreen;
        private static readonly Brush UserColour = Brushes.LightSkyBlue;
        private static readonly Brush ErrorColour = Brushes.LightCoral;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>Runs once the window is showing: plays the voice greeting and welcomes the user.</summary>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ChatLog.Document.Blocks.Clear();

            // wait: false so the window doesn't freeze while the audio plays
            _audio.PlayGreeting(wait: false);

            SetInputEnabled(false);
            await AddMessage("Bot", "Welcome to the Cybersecurity Awareness Bot! Please enter your name.", BotColour);
            SetInputEnabled(true);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await HandleInput();
        }

        private async void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await HandleInput();
            }
        }

        /// <summary>Called when the user presses Send or Enter.</summary>
        private async Task HandleInput()
        {
            string text = InputBox.Text.Trim();
            InputBox.Clear();

            // Switch the input off while the bot is "typing" so messages can't overlap
            SetInputEnabled(false);

            if (string.IsNullOrWhiteSpace(text))
            {
                // Input validation: empty entries
                string error = _user == null ? "Please enter a valid name." : "How can I help you today?";
                await AddMessage("Bot", error, ErrorColour);
            }
            else if (_user == null)
            {
                await HandleName(text);
            }
            else
            {
                await HandleChat(_user, text);
            }

            if (!_chatEnded)
            {
                SetInputEnabled(true);
            }
        }

        /// <summary>The first message the user sends is their name.</summary>
        private async Task HandleName(string text)
        {
            if (text.Length > MaxNameLength)
            {
                await AddMessage("Bot", $"That name is too long (max {MaxNameLength} characters). Please try again.", ErrorColour);
                return;
            }

            _user = new User(text);
            _bot.SetUser(_user); // lets the bot remember details about this user

            await AddMessage("You", text, UserColour, typing: false);
            await AddMessage("Bot",
                $"Hello, {_user.Name}! Ask me about passwords, phishing, safe browsing, malware, privacy, scams, ransomware or social engineering. Type 'exit' to leave.",
                BotColour);
        }

        /// <summary>Every message after the name goes to the chatbot.</summary>
        private async Task HandleChat(User user, string text)
        {
            await AddMessage(user.Name, text, UserColour, typing: false);

            if (_bot.IsExitCommand(text))
            {
                await AddMessage("Bot", $"Thank you, {user.Name}! Stay safe online.", BotColour);
                _chatEnded = true;
                InputBox.IsEnabled = false;
                SendButton.IsEnabled = false;
                return;
            }

            await AddMessage("Bot", _bot.GetResponse(text), BotColour);
        }

        /// <summary>
        /// Adds one message to the chat: a coloured name, the text, and a divider line.
        /// With typing = true the letters appear one at a time.
        /// </summary>
        private async Task AddMessage(string sender, string message, Brush colour, bool typing = true)
        {
            // A Paragraph is one block of the chat. Its bottom border is the divider line.
            Paragraph paragraph = new Paragraph
            {
                Margin = new Thickness(0, 0, 0, 8),
                Padding = new Thickness(0, 0, 0, 8),
                BorderBrush = Brushes.DimGray,
                BorderThickness = new Thickness(0, 0, 0, 1)
            };

            // The sender's name, in bold and in their colour
            paragraph.Inlines.Add(new Run(sender + ": ") { Foreground = colour, FontWeight = FontWeights.Bold });

            // The message text, added letter by letter
            Run body = new Run("") { Foreground = Brushes.White };
            paragraph.Inlines.Add(body);
            ChatLog.Document.Blocks.Add(paragraph);

            foreach (char letter in message.Replace("\r", ""))
            {
                if (letter == '\n')
                {
                    // Start a new line, then keep adding text to a fresh Run
                    paragraph.Inlines.Add(new LineBreak());
                    body = new Run("") { Foreground = Brushes.White };
                    paragraph.Inlines.Add(body);
                }
                else
                {
                    body.Text += letter;
                }

                if (typing)
                {
                    ChatLog.ScrollToEnd();
                    await Task.Delay(TypingDelayMs);
                }
            }

            ChatLog.ScrollToEnd();
        }

        /// <summary>Turns the input box and Send button on or off.</summary>
        private void SetInputEnabled(bool enabled)
        {
            InputBox.IsEnabled = enabled;
            SendButton.IsEnabled = enabled;
            if (enabled)
            {
                InputBox.Focus();
            }
        }
    }
}