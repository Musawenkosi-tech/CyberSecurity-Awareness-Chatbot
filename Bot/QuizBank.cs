using CyberSecurity_Awareness_Chatbot.Models;

namespace CyberSecurity_Awareness_Chatbot.Bot
{
    public static class QuizBank
    {
        public static List<QuizQuestion> Create()
        {
            return new List<QuizQuestion>
            {
                MultipleChoice(
                    "What should you do if you receive an email asking for your password?",
                    2,
                    "Reporting phishing emails helps prevent scams. Legitimate companies never ask for your password by email.",
                    "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it"),

                MultipleChoice(
                    "Which of these is the strongest password?",
                    3,
                    "Long passphrases made of random words are hard to guess and easy to remember.",
                    "Summer2024", "qwerty123", "JohnSmith1990", "Blue-Tractor-Moon-Pancake!"),

                TrueFalse(
                    "Using the same password on all your accounts is safe as long as the password is long.",
                    false,
                    "If one website is breached, attackers try that password everywhere. Use a unique password for each account."),

                MultipleChoice(
                    "What does the padlock and 'https://' in the address bar tell you?",
                    1,
                    "HTTPS means your connection to the site is encrypted. It does not prove the site itself is honest, because scam sites can use it too.",
                    "The website is guaranteed to be safe", "The connection to the site is encrypted",
                    "The site is owned by the government", "The site has no adverts"),

                MultipleChoice(
                    "A caller says they are from your bank and asks for your PIN to 'secure your account'. What should you do?",
                    0,
                    "Banks never ask for your PIN. This is social engineering over the phone, so hang up and use the number on your bank card.",
                    "Hang up and call your bank on its official number", "Give the PIN so they can verify you",
                    "Give only the first two digits", "Ask them to call back tomorrow"),

                TrueFalse(
                    "Public Wi-Fi in a cafe is safe for online banking as long as it has a password.",
                    false,
                    "Other people on the same network may be able to snoop on your traffic. Use mobile data or a trusted VPN for banking."),

                MultipleChoice(
                    "What is ransomware?",
                    1,
                    "Ransomware locks or encrypts your files and demands payment. Regular offline backups let you recover without paying.",
                    "A program that speeds up your computer", "Malware that locks your files and demands payment",
                    "A type of firewall", "A secure backup tool"),

                TrueFalse(
                    "Multi-factor authentication still protects your account if your password is stolen.",
                    true,
                    "A second step, like a code on your phone, stops attackers who only have your password."),

                MultipleChoice(
                    "Which of these is a warning sign of a phishing message?",
                    3,
                    "Phishers create urgency and fear so you act before you think. Slow down and verify through an official channel.",
                    "It was sent during business hours", "It uses your correct first name",
                    "It contains a company logo", "It threatens to close your account unless you act now"),

                MultipleChoice(
                    "You find a USB drive in the car park. What should you do?",
                    2,
                    "Unknown USB drives can carry malware. Hand it to IT or security without plugging it in.",
                    "Plug it into your PC to find the owner", "Plug it into a friend's PC instead",
                    "Hand it to IT or security without plugging it in", "Format it and keep it"),

                TrueFalse(
                    "Keeping your software and apps updated helps protect you from known security weaknesses.",
                    true,
                    "Updates often include fixes for security holes that attackers already know about."),

                MultipleChoice(
                    "What is social engineering?",
                    1,
                    "Social engineering tricks people, not computers, into giving up information or access. Always verify unexpected requests.",
                    "Building social media apps", "Manipulating people into giving up information or access",
                    "Encrypting network traffic", "A way of writing secure code")
            };
        }

        private static QuizQuestion MultipleChoice(string text, int correctIndex, string explanation, params string[] options)
        {
            return new QuizQuestion(text, options.ToList(), correctIndex, explanation);
        }

        private static QuizQuestion TrueFalse(string text, bool answer, string explanation)
        {
            return new QuizQuestion(text, new List<string> { "True", "False" }, answer ? 0 : 1, explanation);
        }
    }
}