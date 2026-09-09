using System.Media;
using System.IO;


namespace CyberSecurity_Awareness_Chatbot.Services
{
    /// <summary>
    /// Plays the recorded voice greeting. Has no Console code, so the
    /// same class can be reused unchanged in the Part 2 GUI.
    /// </summary>
    public class AudioService
    {
        private const string FolderName = "Audio";
        private const string FileName = "welcome.wav";

        // <summary>
        /// Plays the greeting. Returns false if the file is missing or cannot be played.
        /// </summary>
        /// <param name="wait">
        /// true  = block until the audio finishes (fine for a console app).
        /// false = play in the background (required for a GUI so the window doesn't freeze).
        /// </param>
        public bool PlayGreeting(bool wait = false)
        {
            try
            {
                // AppContext.BaseDirectory = the folder the .exe runs from,
                // so this works on any computer, not just the developer's.
                string path = Path.Combine(AppContext.BaseDirectory, FolderName, FileName);

                if (!File.Exists(path))
                {
                    return false;
                }

                using SoundPlayer player = new SoundPlayer(path);
                if (wait)
                {
                    player.PlaySync();
                }
                else
                {
                    player.Play();
                }
                return true;
            }
            catch (Exception)
            {
                // A missing/corrupt audio file should never crash the chatbot.
                return false;
            }
        }
    }
}