using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurity_Awareness_Chatbot
{
    public class ChatBot
    {
        public StartChat(string userName)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{userName}: ");
                Console.ResetColor();

                string userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput)) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("How can I help you today?");
                    Console.ResetColor();
                    continue;
                }
                
                userInput = userInput.ToLower();
                if (userInput.Contains("exit") || userInput.Contains("quit"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Bot: Thank you {userName} for using the Cyber Security Awareness Chatbot. Goodbye!");
                    Console.ResetColor();
                    break;
                } else if (userInput.Contains("how are you") || userInput.Contains("how are you doing")) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: Im well, what cyber term would you like to learn, to help stay safe online today?");
                    Console.ResetColor();
                } else if (userInput.Contains("purpose") || userInput.Contains("define cyber security")) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: My purpose is to teach you about cyber security and staying ssafe online.");
                    Console.ResetColor();
                } else if (userInput.Contains("whay can i ask") || userInput.Contains("cyber security tips")) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: anything about safe browsing and cybersecurity topics");
                    Console.ResetColor();
                } else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bot: I'm sorry, I don't understand that. Please ask a different question or type 'exit' to quit.");
                    Console.ResetColor();



                }
    }
}
