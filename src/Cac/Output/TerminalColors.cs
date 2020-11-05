using System;
using System.Runtime.InteropServices;

namespace Cac.Output
{
    public static class TerminalColors
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const string _formatStringEnd = "\u001b[0m";

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        private static bool _enabled;

        static TerminalColors()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                GetConsoleMode(iStdOut, out var outConsoleMode);
                SetConsoleMode(iStdOut, outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }

            _enabled = Environment.GetEnvironmentVariable("NO_COLOR") == null;
        }

        public static string Foreground(this string input, ConsoleColor color)
        {
            return _enabled ? ColorFormat(input, AnsiCodes.Colors[color]) : input;
        }

        public static string Backgroud(this string input, ConsoleColor color)
        {
            return _enabled ? ColorFormat(input, AnsiCodes.BgColors[color]) : input;
        }

        private static string ColorFormat(string input, string color)
        {
            input = $"{color}{input}";
            if (!input.EndsWith(_formatStringEnd))
                input = $"{input}{_formatStringEnd}";

            return input;
        }
    }
}
