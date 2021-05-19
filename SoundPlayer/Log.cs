using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoundPlayer
{
    public static class Log {
        private static readonly ManualLogSource _logger;

        static Log() {
            _logger = new ManualLogSource("SoundPlayer");
            Logger.Sources.Add(_logger);
        }

        public static void Info(object m) => _logger.LogMessage(m);
        public static void Debug(object m) => _logger.LogDebug(m);
        public static void Error(object m) => _logger.LogError(m);
    }
}
