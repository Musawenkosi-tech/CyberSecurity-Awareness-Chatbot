using System.Windows;
using System.Windows.Controls;
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
        private bool _showingHistory;   // true while the History view is open

        // Every message is also saved here, so the History button still works after Clear
        private readonly List<string> _history = new List<string>();

        private const int MaxNameLength = 30;
        private const int TypingDelayMs = 10;   // pause between letters for the typing effect

        // Colours (light theme): bot bubbles are light grey, user bubbles are blue
        private static readonly Brush BotNameColour = Hex("#15803D");   // green name label
        private static readonly Brush UserNameColour = Hex("#DBEAFE");  // pale blue name label
        private static readonly Brush BotBubble = Hex("#F1F5F9");
        private static readonly Brush UserBubble = Hex("#2563EB");
        private static readonly Brush ErrorBubble = Hex("#FEE2E2");
        private static readonly Brush ErrorText = Hex("#991B1B");
        private static readonly Brush DarkText = Hex("#1F2937");

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>Turns a hex colour like "#2563EB" into a brush.</summary>
        private static SolidColorBrush Hex(string colour)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(colour)!;
        }

        /// <summary>Runs once the window is showing: plays the voice greeting and welcomes the user.</summary>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessagePanel.Children.Clear();

            // wait: false so the window doesn't freeze while the audio plays
            _audio.PlayGreeting(wait: false);

            SetInputEnabled(false);
            await AddMessage("Bot", "Welcome to the Cybersecurity Awareness Bot! Please enter your name.", isBot: true);
            SetInputEnabled(true);
        }

        // ---------------------------------------------------------------
        // Buttons
        // ---------------------------------------------------------------

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

        /// <summary>Clears the chat on screen. The History button still has every message.</summary>
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ShowChatView();
            MessagePanel.Children.Clear();
            AddNote("Chat cleared. Press History to see earlier messages.");
        }

        /// <summary>Switches between the chat and the full message history.</summary>
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_showingHistory)
            {
                ShowChatView();
                return;
            }

            HistoryBox.Text = _history.Count == 0 ? "No messages yet." : string.Join("\n\n", _history);
            HistoryBox.ScrollToEnd();

            ChatScroll.Visibility = Visibility.Collapsed;
            HistoryBox.Visibility = Visibility.Visible;
            HistoryButton.Content = "Back to chat";
            _showingHistory = true;
        }

        private void ShowChatView()
        {
            HistoryBox.Visibility = Visibility.Collapsed;
            ChatScroll.Visibility = Visibility.Visible;
            HistoryButton.Content = "History";
            _showingHistory = false;
        }

        // ---------------------------------------------------------------
        // Handling what the user types
        // ---------------------------------------------------------------

        /// <summary>Called when the user presses Send or Enter.</summary>
        private async Task HandleInput()
        {
            string text = InputBox.Text.Trim();
            InputBox.Clear();

            ShowChatView();

            // Switch the input off while the bot is "typing" so messages can't overlap
            SetInputEnabled(false);

            if (string.IsNullOrWhiteSpace(text))
            {
                // Input validation: empty entries
                string error = _user == null ? "Please enter a valid name." : "How can I help you today?";
                await AddMessage("Bot", error, isBot: true, isError: true);
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
                await AddMessage("Bot", $"That name is too long (max {MaxNameLength} characters). Please try again.",
                    isBot: true, isError: true);
                return;
            }

            _user = new User(text);
            _bot.SetUser(_user); // lets the bot remember details about this user

            await AddMessage("You", text, isBot: false, typing: false);
            await AddMessage("Bot",
                $"Hello, {_user.Name}! Ask me about passwords, phishing, safe browsing, malware, privacy, scams, ransomware or social engineering. Type 'exit' to leave.",
                isBot: true);
        }

        /// <summary>Every message after the name goes to the chatbot.</summary>
        private async Task HandleChat(User user, string text)
        {
            await AddMessage(user.Name, text, isBot: false, typing: false);

            if (_bot.IsExitCommand(text))
            {
                await AddMessage("Bot", $"Thank you, {user.Name}! Stay safe online.", isBot: true);
                _chatEnded = true;
                InputBox.IsEnabled = false;
                SendButton.IsEnabled = false;

                // Clear and History stay usable after the chat has ended
                ClearButton.IsEnabled = true;
                HistoryButton.IsEnabled = true;
                return;
            }

            await AddMessage("Bot", _bot.GetResponse(text), isBot: true);
        }

        // ---------------------------------------------------------------
        // Drawing the chat
        // ---------------------------------------------------------------

        /// <summary>
        /// Adds one chat bubble. Bot messages sit on the left, the user's on the right.
        /// With typing = true the letters appear one at a time.
        /// </summary>
        private async Task AddMessage(string sender, string message, bool isBot, bool typing = true, bool isError = false)
        {
            // Keep a copy for the History view
            _history.Add($"[{DateTime.Now:HH:mm}] {sender}: {message}");

            // Pick the colours for this bubble
            Brush bubbleColour = isError ? ErrorBubble : (isBot ? BotBubble : UserBubble);
            Brush nameColour = isError ? ErrorText : (isBot ? BotNameColour : UserNameColour);
            Brush textColour = isError ? ErrorText : (isBot ? DarkText : Brushes.White);

            // The sender's name, in bold
            TextBlock nameLabel = new TextBlock
            {
                Text = sender,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Foreground = nameColour,
                Margin = new Thickness(0, 0, 0, 2)
            };

            // The message text (starts empty so it can be typed out)
            TextBlock body = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = textColour,
                FontSize = 14
            };

            // Name above, message below
            StackPanel content = new StackPanel();
            content.Children.Add(nameLabel);
            content.Children.Add(body);

            // The bubble: left for the bot, right for the user
            Border bubble = new Border
            {
                Child = content,
                Background = bubbleColour,
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(14, 10, 14, 10),
                Margin = new Thickness(0, 0, 0, 10),
                MaxWidth = 520,
                HorizontalAlignment = isBot ? HorizontalAlignment.Left : HorizontalAlignment.Right
            };
            MessagePanel.Children.Add(bubble);

            if (typing)
            {
                foreach (char letter in message.Replace("\r", ""))
                {
                    body.Text += letter;
                    ChatScroll.ScrollToEnd();
                    await Task.Delay(TypingDelayMs);
                }
            }
            else
            {
                body.Text = message;
            }

            ChatScroll.ScrollToEnd();
        }

        /// <summary>Adds a small grey centred note (not saved in History).</summary>
        private void AddNote(string text)
        {
            MessagePanel.Children.Add(new TextBlock
            {
                Text = text,
                Foreground = Brushes.Gray,
                FontStyle = FontStyles.Italic,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            });
        }

        /// <summary>Turns the input box and buttons on or off (used while the bot is typing).</summary>
        private void SetInputEnabled(bool enabled)
        {
            InputBox.IsEnabled = enabled;
            SendButton.IsEnabled = enabled;
            ClearButton.IsEnabled = enabled;
            HistoryButton.IsEnabled = enabled;

            if (enabled)
            {
                InputBox.Focus();
            }
        }
    }
}