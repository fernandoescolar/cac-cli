using System;

namespace Cac.Output
{
    internal class EmptyOutput : IOutputWriter
    {
        public void Error(string message)
        {
        }

        public void Warning(string message)
        {
        }

        public void Write(string message)
        {
        }

        public void Write(string message, ConsoleColor color)
        {
        }

        public void Write(string message, ConsoleColor color, ConsoleColor backColor)
        {
        }

        public void WriteLine(string message)
        {
        }

        public void WriteLine(string message, ConsoleColor color)
        {
        }

        public void WriteLine(string message, ConsoleColor color, ConsoleColor backColor)
        {
        }
    }
}
