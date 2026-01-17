using System;
using System.IO;
using MelonLoader;

namespace FireBot.Config
{
    public static class BotSettings
    {
        private static MelonPreferences_Category _category;

        public static MelonPreferences_Entry<bool> IsBotEnabled;
        public static MelonPreferences_Entry<float> ScanInterval;
        public static MelonPreferences_Entry<float> InteractionDelay;

        public static void Initialize()
        {
            _category = MelonPreferences.CreateCategory("FireBotSettings", " [ FIRE BOT CONFIGURATION ] ");

            _category.SetFilePath(Path.Combine("UserData", "fire-bot.cfg"));

            // --- CONFIGURATION ENTRIES ---

            IsBotEnabled = _category.CreateEntry(
                "IsBotEnabled",
                true,
                "Enable Bot",
                "Toggle the main logic of the bot. If false, all automated actions are suspended.");

            ScanInterval = _category.CreateEntry(
                "ScanInterval",
                5.0f,
                "Scan Interval (seconds)",
                "The frequency at which the bot searches for new targets. Lower values increase responsiveness but may affect CPU performance.");

            InteractionDelay = _category.CreateEntry(
                "InteractionDelay",
                1.0f,
                "Interaction Delay (seconds)",
                "Wait time between specific actions (clicks, key presses, etc.) to simulate human-like behavior.");

            _category.SaveToFile();

            // Usando a técnica de duas cores que aprendemos para o log
            MelonLogger.Msg(ConsoleColor.DarkCyan, "[FireBot] ", ConsoleColor.Gray,
                "Configuration initialized at UserData/fire-bot.cfg");
        }

        public static void Save()
        {
            _category.SaveToFile();
        }
    }
}