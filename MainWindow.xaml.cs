using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CyberSecurity_Awareness_Chatbot.Bot;
using CyberSecurity_Awareness_Chatbot.Models;
using CyberSecurity_Awareness_Chatbot.Services;

namespace CyberSecurity_Awareness_Chatbot
{
    
    public partial class MainWindow : Window
    {
        private readonly ChatBot _bot = new ChatBot();
        private readonly AudioService _audio = new AudioService();
        private User? _user;            
        private bool _chatEnded;       
        private bool _showingHistory;   

        
        private readonly List<string> _history = new List<string>();

        private const int MaxNameLength = 30;
        private const int TypingDelayMs = 10;   

        // Colours: the name label and the bubble background for each speaker
        private static readonly Brush BotColour = Brushes.LightGreen;
        private static readonly Brush UserColour = Brushes.LightSkyBlue;
        private static readonly Brush BotBubble = new SolidColorBrush(Color.FromRgb(49, 50, 68));
        private static readonly Brush UserBubble = new SolidColorBrush(Color.FromRgb(44, 62, 94));
        private static readonly Brush ErrorBubble = new SolidColorBrush(Color.FromRgb(74, 42, 51));

      

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>Runs once the window is showing: plays the voice greeting and welcomes the user.</summary>
        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MessagePanel.Children.Clear();

            
            _audio.PlayGreeting(wait: false);

            SetInputEnabled(false);
            await AddMessage("Bot", "Welcome to the Cybersecurity Awareness Bot! Please enter your name.", isBot: true);
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

        
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ShowChatView();
            MessagePanel.Children.Clear();
            AddNote("Chat cleared. Press History to see earlier messages.");
        }

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

       
        private async Task HandleInput()
        {
            string text = InputBox.Text.Trim();
            InputBox.Clear();

            ShowChatView();

           
            SetInputEnabled(false);

            if (string.IsNullOrWhiteSpace(text))
            {
                
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

        
        private async Task HandleName(string text)
        {
            if (text.Length > MaxNameLength)
            {
                await AddMessage("Bot", $"That name is too long (max {MaxNameLength} characters). Please try again.",
                    isBot: true, isError: true);
                return;
            }

            _user = new User(text);
            _bot.SetUser(_user); 

            await AddMessage("You", text, isBot: false, typing: false);
            await AddMessage("Bot",
                $"Hello, {_user.Name}! Ask me about passwords, phishing, safe browsing, malware, privacy, scams, ransomware or social engineering. Type 'exit' to leave.",
                isBot: true);
        }

        
        private async Task HandleChat(User user, string text)
        {
            await AddMessage(user.Name, text, isBot: false, typing: false);

            if (_bot.IsExitCommand(text))
            {
                await AddMessage("Bot", $"Thank you, {user.Name}! Stay safe online.", isBot: true);
                _chatEnded = true;
                InputBox.IsEnabled = false;
                SendButton.IsEnabled = false;

                
                ClearButton.IsEnabled = true;
                HistoryButton.IsEnabled = true;
                return;
            }

            await AddMessage("Bot", _bot.GetResponse(text), isBot: true);
        }

        
        private async Task AddMessage(string sender, string message, bool isBot, bool typing = true, bool isError = false)
        {
            
            _history.Add($"[{DateTime.Now:HH:mm}] {sender}: {message}");

            // The sender's name, in bold and in their colour
            TextBlock nameLabel = new TextBlock
            {
                Text = sender,
                FontWeight = FontWeights.Bold,
                FontSize = 12,
                Foreground = nameColour,
                Margin = new Thickness(0, 0, 0, 2)
            };

            
            TextBlock body = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Foreground = textColour,
                FontSize = 14
            };

            
            StackPanel content = new StackPanel();
            content.Children.Add(nameLabel);
            content.Children.Add(body);

           
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

        /// <summary>Shows the ASCII art at the top of the chat.</summary>
        private void AddBanner()
        {
            MessagePanel.Children.Add(new TextBlock
            {
                Text = Banner,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                Foreground = BotColour,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 12)
            });
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