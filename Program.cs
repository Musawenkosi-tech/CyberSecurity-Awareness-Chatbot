// See https://aka.ms/new-console-template for more information
using System;

namespace CyberSecurity_Awareness_Chatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatBot chatBot = new ChatBot();
            
        
            Welcome.recording();
            Welcome.DisplayLogo();

            string userName = Welcome.welcome();
            User user = new User(userName);

            chatBot.StartChat(user.userName);
        }
    }
}
