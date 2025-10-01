using Service.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    public static class EventPublishers
    {
        public static FileWriter FileWriter { get; set; }

        public static void LogInformation(object sender, EventArgsWithMessage e)
        {
            FileWriter.WriteText($"{DateTime.Now}: {e.EventMessage}");
            Console.WriteLine(e.EventMessage);
        }

        public static void LogException(object sender, EventArgsWithMessage e)
        {
            FileWriter.WriteText($"{DateTime.Now}: {e.EventMessage}");

            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e.EventMessage);
            Console.ForegroundColor = defaultColor;
        }

        public static void LogWarning(object sender, EventArgsWithMessage e)
        {
            FileWriter.WriteText($"{DateTime.Now}: {e.EventMessage}");

            var defaultColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(e.EventMessage);
            Console.ForegroundColor = defaultColor;
        }
    }
}
