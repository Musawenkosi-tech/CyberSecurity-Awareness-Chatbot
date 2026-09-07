using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyberSecurity_Awareness_Chatbot
{
    class Welcome
    {
        static void welcome()
        {
            Console.WriteLine("Welcome to the Cyber Security Awareness Chatbot!");
            Console.WriteLine("Please enter your name: ");
            string userName = Console.ReadLine();
            Console.WriteLine($"Hello, {userName}! Let's get started with some cyber security awareness questions.");
        }

        static void DisplayLogo()
        {
            Console.WriteLine("  ____ _                 _   _                 _             _   _             ");
            Console.WriteLine(" / ___| | ___  _   _  __| | | |__   __ _ _ __ | |_ ___  __ _| |_(_) ___  _ __  ");
            Console.WriteLine("| |   | |/ _ \\| | | |/ _` | | '_ \\ / _` | '_ \\| __/ _ \\/ _` | __| |/ _ \\| '_ \\ ");
            Console.WriteLine("| |___| | (_) | |_| | (_| | | |_) | (_| | | | | ||  __/ (_| | |_| | (_) | | | |");
            Console.WriteLine(" \\____|_|\\___/ \\__,_|\\__,_| |_.__/ \\__,_|_| |_|\\__\\___|\\__,_|\\__|_|\\___/|_| |_|");
            Console.WriteLine();
        }

        static void recording()
        {
            
        }
}
