namespace CyberSecurity_Awareness_Chatbot.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }

        public string ReminderText => ReminderDate?.ToString("dd MMM yyyy HH:mm") ?? "None";
        public string StatusText => IsCompleted ? "Completed" : "Pending";
    }
}