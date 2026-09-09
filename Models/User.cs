namespace CyberSecurity_Awareness_Chatbot.Models
{
    /// <summary>
    /// Holds information about the person chatting with the bot,
    /// including what the bot has "remembered" about them.
    /// </summary>
    public class User
    {
        public string Name { get; set; }

        /// <summary>The topic the user said they are interested in (null until they tell us).</summary>
        public string? FavouriteTopic { get; set; }

        /// <summary>Topics the user has asked about, in the order they came up.</summary>
        public List<string> TopicsDiscussed { get; } = new List<string>();

        public User(string name)
        {
            Name = name;
        }

        /// <summary>Remembers a topic (once only).</summary>
        public void RememberTopic(string topic)
        {
            if (!TopicsDiscussed.Contains(topic))
            {
                TopicsDiscussed.Add(topic);
            }
        }
    }
}