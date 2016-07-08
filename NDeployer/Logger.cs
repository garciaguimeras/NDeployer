using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDeployer
{

    enum LogType
    {
        INFO, 
        WARNING, 
        ERROR
    }

    class Logger
    {

        private static void log(LogType type, int level, string text, params object[] args)
        {
            string formattedText = string.Format(text, args);
            for (int i = 0; i < level - 1; i++)
                formattedText = "  " + formattedText;  
            Console.WriteLine("{0} - [{1}] - {2}", DateTime.Now, type, formattedText);
        }

        public static void info(string text, params object[] args)
        {
            log(LogType.INFO, 1, text, args);
        }

        public static void info(int level, string text, params object[] args)
        {
            log(LogType.INFO, level, text, args);
        }

        public static void error(string text, params object[] args)
        {
            log(LogType.ERROR, 1, text, args);
        }

        public static void warning(string text, params object[] args)
        {
            log(LogType.WARNING, 1, text, args);
        }

    }
}
