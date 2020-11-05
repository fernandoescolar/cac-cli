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
            _lastChar = message.LastOrDefault();

            if (Console.BackgroundColor != backColor)
                message = message.Backgroud(backColor);
            if (Console.ForegroundColor != color)
                message = message.Foreground(color);

            Console.Write(message);
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
