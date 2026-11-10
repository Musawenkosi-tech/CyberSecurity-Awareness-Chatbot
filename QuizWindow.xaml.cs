using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CyberSecurity_Awareness_Chatbot.Bot;
using CyberSecurity_Awareness_Chatbot.Models;

namespace CyberSecurity_Awareness_Chatbot
{
    public partial class QuizWindow : Window
    {
        private readonly ActivityLog _log;
        private readonly List<QuizQuestion> _questions;
        private int _index;
        private int _score;
        private bool _answered;
        private bool _finished;

        public string ResultMessage { get; private set; } = "";

        public QuizWindow(ActivityLog log)
        {
            InitializeComponent();
            _log = log;

            Random random = new Random();
            _questions = QuizBank.Create().OrderBy(question => random.Next()).ToList();

            _log.Add($"Quiz started ({_questions.Count} questions)");
            ShowQuestion();
        }

        private static SolidColorBrush Hex(string colour)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(colour)!;
        }

        private void ShowQuestion()
        {
            QuizQuestion question = _questions[_index];
            _answered = false;

            ProgressText.Text = $"Question {_index + 1} of {_questions.Count}   |   Score: {_score}";
            QuestionText.Text = question.Text;
            FeedbackText.Text = "";
            NextButton.Visibility = Visibility.Collapsed;
            OptionsPanel.Children.Clear();

            for (int i = 0; i < question.Options.Count; i++)
            {
                string label = question.Options.Count == 2
                    ? question.Options[i]
                    : $"{(char)('A' + i)})  {question.Options[i]}";

                Button button = new Button
                {
                    Content = label,
                    Tag = i,
                    FontSize = 14,
                    Padding = new Thickness(12, 8, 12, 8),
                    Margin = new Thickness(0, 0, 0, 8),
                    HorizontalContentAlignment = HorizontalAlignment.Left,
                    Background = Brushes.White,
                    BorderBrush = Hex("#CBD5E1")
                };
                button.Click += Option_Click;
                OptionsPanel.Children.Add(button);
            }

            NextButton.Content = _index == _questions.Count - 1 ? "See my score" : "Next question";
        }

        private void Option_Click(object sender, RoutedEventArgs e)
        {
            if (_answered)
            {
                return;
            }
            _answered = true;

            Button clicked = (Button)sender;
            int chosen = (int)clicked.Tag;
            QuizQuestion question = _questions[_index];
            bool correct = chosen == question.CorrectIndex;

            if (correct)
            {
                _score++;
            }

            ((Button)OptionsPanel.Children[question.CorrectIndex]).Background = Hex("#BBF7D0");
            if (!correct)
            {
                clicked.Background = Hex("#FECACA");
            }

            FeedbackText.Text = (correct ? "Correct! " : "Not quite. ") + question.Explanation;
            FeedbackText.Foreground = correct ? Hex("#166534") : Hex("#991B1B");
            ProgressText.Text = $"Question {_index + 1} of {_questions.Count}   |   Score: {_score}";
            NextButton.Visibility = Visibility.Visible;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_finished)
            {
                Close();
                return;
            }

            _index++;
            if (_index >= _questions.Count)
            {
                ShowResult();
            }
            else
            {
                ShowQuestion();
            }
        }

        private void ShowResult()
        {
            _finished = true;
            int total = _questions.Count;
            double ratio = (double)_score / total;

            string feedback;
            if (ratio >= 0.8)
            {
                feedback = "Great job! You're a cybersecurity pro!";
            }
            else if (ratio >= 0.5)
            {
                feedback = "Good effort! A little more practice and you'll be a pro.";
            }
            else
            {
                feedback = "Keep learning to stay safe online!";
            }

            ResultMessage = $"Quiz complete! You scored {_score} out of {total}. {feedback}";
            _log.Add($"Quiz completed - scored {_score}/{total}");

            ProgressText.Text = $"Final score: {_score} / {total}";
            QuestionText.Text = "Quiz complete!";
            OptionsPanel.Children.Clear();
            FeedbackText.Text = $"You scored {_score} out of {total}.\n{feedback}";
            FeedbackText.Foreground = Hex("#1E3A8A");
            FeedbackText.FontSize = 16;
            NextButton.Content = "Close";
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!_finished)
            {
                int answered = _index + (_answered ? 1 : 0);
                _log.Add($"Quiz closed early - {answered} of {_questions.Count} questions answered");
            }
            base.OnClosed(e);
        }
    }
}