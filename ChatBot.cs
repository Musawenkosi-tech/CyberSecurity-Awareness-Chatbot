using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurity_Awareness_Chatbot
{
    public class ChatBot
    {
        public void StartChat(string userName)
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"{userName}: ");
                Console.ResetColor();

                string userInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(userInput))
                {
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
                }
                else if (userInput.Contains("how are you") || userInput.Contains("how are you doing"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: Im well, what cyber term would you like to learn, to help stay safe online today?");
                    Console.ResetColor();
                }
                else if (userInput.Contains("purpose") || userInput.Contains("define cyber security"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: My purpose is to teach you about cyber security and staying ssafe online.");
                    Console.ResetColor();
                }
                else if (userInput.Contains("what can i ask") || userInput.Contains("cyber security tips"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: anything about safe browsing and cybersecurity topics");
                    Console.ResetColor();
                }else if (userInput.Contains("password"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: Password safety means creating strong, unique passwords for each account and storing them securely, often with a password manager. It also involves enabling multi-factor authentication and regularly updating passwords to reduce the risk of unauthorized access.");
                    Console.ResetColor();
                }
                else if (userInput.Contains("phishing")) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: Phishing is a cyberattack where scammers impersonate trusted entities via email, messages, or fake websites to steal sensitive information like passwords or credit card details. Protect yourself by verifying sender addresses, avoiding suspicious links, and never sharing personal data in response to unsolicited requests.");
                    Console.ResetColor();

                }
                else if (userInput.Contains("safe browsing")) {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Bot: Safe browsing means visiting only trusted websites, checking for HTTPS encryption, and keeping your browser and security software up to date. It also includes avoiding downloads from unknown sources and using ad-blockers or anti-tracking tools to reduce exposure to malicious content.");
                    Console.ResetColor();

                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Bot: I'm sorry, I don't understand that. Please ask a different question or type 'exit' to quit.");
                    Console.ResetColor();



                }
            }
        }
    }
}
