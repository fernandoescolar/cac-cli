using System;
using System.Linq;

namespace Cac.Output
{
    public class Terminal
    {
        private char _lastChar = ' ';

        public void Write(string message, int sectionCounter, ConsoleColor color, ConsoleColor backColor)
        {
            var spaces = new string(' ', sectionCounter * 2);
            if (_lastChar == '\n') Console.Write(spaces);

            var formattedMessage = CreateSpacedMessage(message, spaces);
            Write(formattedMessage, color, backColor);
        }

        private void Write(string message, ConsoleColor color, ConsoleColor backColor)
        {
            var cColor = Console.ForegroundColor;
            var cBackColor = Console.BackgroundColor;
            Console.ForegroundColor = color;
            Console.BackgroundColor = backColor;
            Console.Write(message);
            Console.ForegroundColor = cColor;
            Console.BackgroundColor = cBackColor;

            _lastChar = message.LastOrDefault();
        }

        private static string CreateSpacedMessage(string message, string spaces)
        {
            var spacedMessage = message.Replace("\n", $"\n{spaces}");
            if (spacedMessage.EndsWith("\n" + spaces))
            {
                spacedMessage = spacedMessage.Substring(0, spacedMessage.Length - spaces.Length);
            }

            return spacedMessage;
        }
    }
}
