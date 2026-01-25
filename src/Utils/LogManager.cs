using System;
using System.Diagnostics;
using MelonLoader;
using MelonLoader.Logging;
using static MelonLoader.Logging.ColorARGB;

namespace Firebot.Utils
{
    public static class LogManager
    {
        private static MelonLogger.Instance _melonLogger;

        public static void Initialize(MelonLogger.Instance loggerInstance)
        {
            _melonLogger = loggerInstance;
        }

        public static void Info(string message)
        {
            _melonLogger?.Msg(message);
        }

        public static void Warn(string message)
        {
            _melonLogger?.Warning(message);
        }

        public static void Error(string message)
        {
            _melonLogger?.Error(message);
        }

        public static void Success(string message)
        {
            _melonLogger?.MsgPastel(ConsoleColor.Green, message);
        }

        public static void Header(string title)
        {
            SubHeader(title);
            _melonLogger?.WriteLine(Green);
        }

        public static void SubHeader(string title)
        {
            _melonLogger?.MsgPastel(ConsoleColor.Cyan, $"[ {title.ToUpper()} ]");
        }

        [Conditional("DEBUG")]
        public static void Debug(string message)
        {
            _melonLogger?.MsgPastel(ConsoleColor.DarkGray, $"[DEBUG] {message}");
        }

        public static void WriteLine()
        {
            _melonLogger?.WriteLine(Green);
        }

        public static void WriteLine(ColorARGB color)
        {
            _melonLogger?.WriteLine(color);
        }
    }
}