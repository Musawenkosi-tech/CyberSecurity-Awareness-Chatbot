// See https://aka.ms/new-console-template for more information
using System;

namespace CyberSecurity_Awareness_Chatbot
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatBot chatbot = new ChatBot();
            Welcome welcome = new Welcome();

            welcome.recording();
            welcome.DisplayLogo();
            welcome.welcome();

            chatbot.StartChat();
        }
    }
}
