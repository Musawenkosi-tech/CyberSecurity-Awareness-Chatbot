using System.Media;
using System.IO;


namespace CyberSecurity_Awareness_Chatbot.Services
{
    
    public class AudioService
    {
        private const string FolderName = "Audio";
        private const string FileName = "welcome.wav";

       
        public bool PlayGreeting(bool wait = false)
        {
            try
            {
                
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
                
                return false;
            }
        }
    }
}