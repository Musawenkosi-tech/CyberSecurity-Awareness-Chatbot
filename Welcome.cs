using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace CyberSecurity_Awareness_Chatbot
{
    class Welcome
    {
        public static string welcome()
        {
            Console.WriteLine("Welcome to the Cyber Security Awareness Chatbot!");

            string userName = "";
            bool isValid = false;

            while (!isValid)
            {
                Console.WriteLine("Please enter your name: ");
                userName = Console.ReadLine();

                if (string.IsNullOrEmpty(userName))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Please enter a valid name");
                    Console.ResetColor();
                }
                else {
                    isValid = true;
                }

            }
                Console.WriteLine($"Hello, {userName}! Let's get started or enter 'exit' to leave the program. Feel free to ask me about cyber terms like  password safety, phishing or safe browsing. ");
                return userName;
            }

        public static void DisplayLogo()
        {
            Console.WriteLine("  ____ _                 _   _                 _             _   _             ");
            Console.WriteLine(" / ___| | ___  _   _  __| | | |__   __ _ _ __ | |_ ___  __ _| |_(_) ___  _ __  ");
            Console.WriteLine("| |   | |/ _ \\| | | |/ _` | | '_ \\ / _` | '_ \\| __/ _ \\/ _` | __| |/ _ \\| '_ \\ ");
            Console.WriteLine("| |___| | (_) | |_| | (_| | | |_) | (_| | | | | ||  __/ (_| | |_| | (_) | | | |");
            Console.WriteLine(" \\____|_|\\___/ \\__,_|\\__,_| |_.__/ \\__,_|_| |_|\\__\\___|\\__,_|\\__|_|\\___/|_| |_|");
            Console.WriteLine();
        }

        public static void recording()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("C:\\Users\\Musa_\\source\\repos\\Programming_ part1\\CyberSecurity Awareness Chatbot\\Audio\\welcome.wav");
                player.PlaySync();
            }
            catch (Exception )
            {
                Console.WriteLine("Check file ");
            }
        }
    }
}
