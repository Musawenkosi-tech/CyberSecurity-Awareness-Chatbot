using CyberSecurity_Awareness_Chatbot.Models;
using MySqlConnector;

namespace CyberSecurity_Awareness_Chatbot.Services
{
    public class TaskRepository
    {
        private readonly string _connectionString;

        public string? LastError { get; private set; }

        public TaskRepository(DatabaseSettings settings)
        {
            _connectionString = settings.ConnectionString;
        }

        public bool TestConnection()
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(_connectionString);
                connection.Open();
                LastError = null;
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public int AddTask(string title, string description, DateTime? reminderDate)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(_connectionString);
                connection.Open();

                string sql = "INSERT INTO tasks (title, description, reminder_date) VALUES (@title, @description, @reminder)";
                using MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@reminder", (object?)reminderDate ?? DBNull.Value);
                command.ExecuteNonQuery();

                LastError = null;
                return (int)command.LastInsertedId;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return -1;
            }
        }

        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            try
            {
                using MySqlConnection connection = new MySqlConnection(_connectionString);
                connection.Open();

                string sql = "SELECT id, title, description, reminder_date, is_completed, created_at " +
                             "FROM tasks ORDER BY is_completed, created_at DESC";
                using MySqlCommand command = new MySqlCommand(sql, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    tasks.Add(new TaskItem
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.GetString(2),
                        ReminderDate = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                        IsCompleted = reader.GetBoolean(4),
                        CreatedAt = reader.GetDateTime(5)
                    });
                }

                LastError = null;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }

            return tasks;
        }

        public bool SetReminder(int id, DateTime reminderDate)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(_connectionString);
                connection.Open();

                string sql = "UPDATE tasks SET reminder_date = @reminder WHERE id = @id";
                using MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@reminder", reminderDate);
                command.Parameters.AddWithValue("@id", id);
                int rowsChanged = command.ExecuteNonQuery();

                LastError = null;
                return rowsChanged > 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public List<TaskItem> GetDueReminders()
        {
            List<TaskItem> due = new List<TaskItem>();

            try
            {
                using MySqlConnection connection = new MySqlConnection(_connectionString);
                connection.Open();

                string sql = "SELECT id, title, description, reminder_date, is_completed, created_at " +
                             "FROM tasks WHERE is_completed = 0 AND reminder_date IS NOT NULL AND reminder_date <= @now";
                using MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@now", DateTime.Now);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    due.Add(new TaskItem
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.GetString(2),
                        ReminderDate = reader.GetDateTime(3),
                        IsCompleted = reader.GetBoolean(4),
                        CreatedAt = reader.GetDateTime(5)
                    });
                }

                LastError = null;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }

            return due;
        }

        public bool MarkCompleted(int id)
        {
            return RunUpdate("UPDATE tasks SET is_completed = 1 WHERE id = @id", id);
        }

        public bool DeleteTask(int id)
        {
            return RunUpdate("DELETE FROM tasks WHERE id = @id", id);
        }

        private bool RunUpdate(string sql, int id)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection(_connectionString);
                connection.Open();

                using MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                int rowsChanged = command.ExecuteNonQuery();

                LastError = null;
                return rowsChanged > 0;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }
    }
}