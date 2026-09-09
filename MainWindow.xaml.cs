using System.Windows;
using System.Windows.Input;
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
        private User? _user; // null until the user has entered their name

        public MainWindow()
        {
            InitializeComponent();

            // wait: false so the window doesn't freeze while the audio plays
            _audio.PlayGreeting(wait: false);
            AddMessage("Bot", "Welcome to the Cybersecurity Awareness Bot! Please enter your name.");
            InputBox.Focus();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            HandleInput();
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                HandleInput();
            }
        }

        /// <summary>Called when the user presses Send or Enter.</summary>
        private void HandleInput()
        {
            string text = InputBox.Text.Trim();
            InputBox.Clear();

            // Input validation: empty entries
            if (string.IsNullOrWhiteSpace(text))
            {
                AddMessage("Bot", _user == null ? "Please enter a valid name." : "How can I help you today?");
                return;
            }

            // The first message is the user's name
            if (_user == null)
            {
                _user = new User(text);
                _bot.SetUser(_user); // lets the bot remember details about this user
                AddMessage("You", text);
                AddMessage("Bot", $"Hello, {_user.Name}! Ask me about passwords, phishing, safe browsing, malware, privacy, scams, ransomware or social engineering. Type 'exit' to leave.");
                return;
            }

            AddMessage(_user.Name, text);

            if (_bot.IsExitCommand(text))
            {
                AddMessage("Bot", $"Thank you, {_user.Name}! Stay safe online.");
                InputBox.IsEnabled = false;
                SendButton.IsEnabled = false;
                return;
            }

            AddMessage("Bot", _bot.GetResponse(text));
        }

        /// <summary>Adds one line to the chat history and scrolls to the bottom.</summary>
        private void AddMessage(string sender, string message)
        {
            ChatLog.AppendText($"{sender}: {message}\n\n");
            ChatLog.ScrollToEnd();
        }
    }
}