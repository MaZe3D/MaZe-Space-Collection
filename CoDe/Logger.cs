using System;
using System.Collections.Generic;

using System.Text;

namespace IngameScript
{
    internal static class Logger
    {
        static List<ILogListener> _logListeners = new List<ILogListener>();
        static Logger()
        {

        }

        public static void AttachLogger(ILogListener logListener)
        {
            _logListeners.Add(logListener);
        }

        public static void DetachLogger(ILogListener logListener)
        {
            _logListeners.Remove(logListener);
        }

        public static void Log(string message)
        {
            foreach (var logListener in _logListeners)
            {
                logListener.LogMessage(message);
            }
        }
    }

    interface ILogListener
    {
        void LogMessage(string message);
    }

    internal class FunctionalLogListener : ILogListener
    {
        private readonly Func<string, bool> logFunction;

        public FunctionalLogListener(Func<string, bool> func)
        {
            logFunction = func;
        }

        public void LogMessage(string message)
        {
            logFunction(message);
        }
    }

    internal class MemoryLogListener : ILogListener
    {
        StringBuilder _log;

        public MemoryLogListener()
        {
            _log = new StringBuilder();
        }

        public void LogMessage(string message)
        {
            _log.AppendLine(message);
        }

        public string Log => _log.ToString();
    }
}
