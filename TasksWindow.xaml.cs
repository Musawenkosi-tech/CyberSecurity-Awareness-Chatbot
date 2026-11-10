using CyberSecurity_Awareness_Chatbot.Models;
using CyberSecurity_Awareness_Chatbot.Services;
using System.Diagnostics;
using System.Windows;

namespace CyberSecurity_Awareness_Chatbot
{
    public partial class TasksWindow : Window
    {
        private readonly TaskRepository _repository;

        public TasksWindow(TaskRepository repository)
        {
            InitializeComponent();
            _repository = repository;
            LoadTasks();
        }

        private void LoadTasks()
        {
            TaskList.ItemsSource = _repository.GetAllTasks();

            if (_repository.LastError != null)
            {
                MessageBox.Show("Could not load tasks: " + _repository.LastError, "Database error");
            }
        }

        private TaskItem? GetSelectedTask()
        {
            TaskItem? task = TaskList.SelectedItem as TaskItem;
            if (task == null)
            {
                MessageBox.Show("Please select a task first.", "No task selected");
            }
            return task;
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            TaskItem? task = GetSelectedTask();
            if (task == null)
            {
                return;
            }

            if (!_repository.MarkCompleted(task.Id))
            {
                MessageBox.Show("Could not update the task: " + _repository.LastError, "Database error");
            }
            LoadTasks();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            TaskItem? task = GetSelectedTask();
            if (task == null)
            {
                return;
            }

            MessageBoxResult answer = MessageBox.Show($"Delete '{task.Title}'? This cannot be undone.",
                "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (answer != MessageBoxResult.Yes)
            {
                return;
            }

            if (!_repository.DeleteTask(task.Id))
            {
                MessageBox.Show("Could not delete the task: " + _repository.LastError, "Database error");
            }
            LoadTasks();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTasks();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}