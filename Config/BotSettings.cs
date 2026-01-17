using System.IO;
using FireBot.Utils;
using MelonLoader;

namespace FireBot.Config
{
    public static class BotSettings
    {
        private static MelonPreferences_Category _category;

        public static MelonPreferences_Entry<bool> IsBotEnabled;
        public static MelonPreferences_Entry<float> ScanInterval;

        public static void Initialize()
        {
            _category = MelonPreferences.CreateCategory("FireBotSettings", "Fire Bot Configurations");

            _category.SetFilePath(Path.Combine("UserData", "fire-bot.cfg"));

            IsBotEnabled = _category.CreateEntry("IsBotEnabled", false, "Enable Bot");

            ScanInterval = _category.CreateEntry("ScanInterval", 5.0f, "Scan Interval (seconds)");

            _category.SaveToFile();

            LogManager.Info("Settings saved to UserData/fire-bot.cfg");
        }

        public static void Save()
        {
            _category.SaveToFile();
        }
    }
}