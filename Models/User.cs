namespace CyberSecurity_Awareness_Chatbot.Models
{
    
    public class User
    {
        public string Name { get; set; }

        
        public string? FavouriteTopic { get; set; }

        
        public List<string> TopicsDiscussed { get; } = new List<string>();

        public User(string name)
        {
            Name = name;
        }

        
        public void RememberTopic(string topic)
        {
            if (!TopicsDiscussed.Contains(topic))
            {
                TopicsDiscussed.Add(topic);
            }
        }
    }
}