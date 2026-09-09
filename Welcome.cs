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
            string logo = @"
               ___  _  _  ____  ____  ____  ____  ____  ___  _  _  ____  __  ____  _  _    ____   __  ____       
 / __)( \/ )(  _ \(  __)(  _ \/ ___)(  __)/ __)/ )( \(  _ \(  )(_  _)( \/ )  (  _ \ /  \(_  _)      
( (__  )  /  ) _ ( ) _)  )   /\___ \ ) _)( (__ ) \/ ( )   / )(   )(   )  /    ) _ ((  O ) )(        
 \___)(__/  (____/(____)(__\_)(____/(____)\___)\____/(__\_)(__) (__) (__/    (____/ \__/ (__)       ";       
            Console.ResetColor();
            Console.WriteLine(logo);

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
