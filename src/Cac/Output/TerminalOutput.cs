using Cac.Options;
using System;
using System.Collections.Generic;

namespace Cac.Output
{
    public class TerminalOutput : IOutput
    {
        private readonly Terminal _terminal;
        private readonly Stack<Section> _sections = new Stack<Section>();

        public TerminalOutput(ICacOptions options)
        {
            _terminal = new Terminal();
            Verbose = options.Verbose ? (IOutputWriter)this : new EmptyOutput();
        }

        public IOutputWriter Verbose { get; }

        public void BeginSection(string title = default)
        {
            _sections.Push(new Section(title ?? string.Empty));
        }

        public void EndSection()
        {
            if (_sections.Count > 0)
            {
                _sections.Pop();
            }
        }

        public void ResetSections()
        {
            _sections.Clear();
        }

        public void Error(string message)
        {
            WriteLine(message, ConsoleColor.Red);
        }

        public void Warning(string message)
        {
            WriteLine(message, ConsoleColor.Yellow);
        }

        public void Write(string message)
        {
            Write(message, Console.ForegroundColor, Console.BackgroundColor);
        }

        public void Write(string message, ConsoleColor color)
        {
            Write(message, color, Console.BackgroundColor);
        }

        public void Write(string message, ConsoleColor color, ConsoleColor backColor)
        {
            var section = _sections.Count > 0 ? _sections.Peek() : default;
            if (section?.WriteTitle ?? false)
            {
                _terminal.Write(section.Title + "\n", Math.Max(_sections.Count - 1, 0), ConsoleColor.White, Console.BackgroundColor);
                section.WriteTitle = false;
            }

            _terminal.Write(message, _sections.Count, color, backColor);
        }

        public void WriteLine(string message)
        {
            WriteLine(message, Console.ForegroundColor, Console.BackgroundColor);
        }

        public void WriteLine(string message, ConsoleColor color)
        {
            WriteLine(message, color, Console.BackgroundColor);
        }

        public void WriteLine(string message, ConsoleColor color, ConsoleColor backColor)
        {
            Write(message, color, backColor);
            Write("\n");
        }

        private class Section
        {
            public Section(string title)
            {
                Title = title;
                WriteTitle = !string.IsNullOrWhiteSpace(Title);
            }

            public string Title { get; }

            public bool WriteTitle { get; set; }
        }
    }
}
