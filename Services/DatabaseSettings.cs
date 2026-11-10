using System;
using System.IO;
using System.Text.Json;
using MySqlConnector;

namespace CyberSecurity_Awareness_Chatbot.Services
{
    public class DatabaseSettings
    {
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 3306;
        public string Database { get; set; } = "cyber_chatbot";
        public string User { get; set; } = "root";
        public string Password { get; set; } = "";

        public string ConnectionString
        {
            get
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
                {
                    Server = Server,
                    Port = (uint)Port,
                    Database = Database,
                    UserID = User,
                    Password = Password
                };
                return builder.ConnectionString;
            }
        }

        public static DatabaseSettings? Load()
        {
            string path = Path.Combine(AppContext.BaseDirectory, "dbsettings.json");

            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<DatabaseSettings>(File.ReadAllText(path), options);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}