using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cac.Output
{
    public static class AnsiCodes
    {

        public static readonly ReadOnlyDictionary<ConsoleColor, string> Colors = new ReadOnlyDictionary<ConsoleColor, string>(new Dictionary<ConsoleColor, string> {
            { ConsoleColor.DarkBlue,    "\u001b[38;5;18m" },
            { ConsoleColor.DarkGreen,   "\u001b[38;5;22m" },
            { ConsoleColor.DarkCyan,    "\u001b[38;5;36m" },
            { ConsoleColor.DarkRed,     "\u001b[38;5;52m" },
            { ConsoleColor.DarkMagenta, "\u001b[38;5;90m" },
            { ConsoleColor.DarkYellow,  "\u001b[38;5;138m" },
            { ConsoleColor.DarkGray,    "\u001b[38;5;242m" },
            { ConsoleColor.Gray,        "\u001b[38;5;252m" },
            { ConsoleColor.Black,       "\u001b[30m" },
            { ConsoleColor.Red,         "\u001b[31m" },
            { ConsoleColor.Green,       "\u001b[32m" },
            { ConsoleColor.Yellow,      "\u001b[33m" },
            { ConsoleColor.Blue,        "\u001b[34m" },
            { ConsoleColor.Magenta,     "\u001b[35m" },
            { ConsoleColor.Cyan,        "\u001b[36m" },
            { ConsoleColor.White,       "\u001b[37m" }
        });

        public static readonly ReadOnlyDictionary<ConsoleColor, string> BgColors = new ReadOnlyDictionary<ConsoleColor, string>(new Dictionary<ConsoleColor, string> {
            { ConsoleColor.DarkBlue,    "\u001b[48;5;18m" },
            { ConsoleColor.DarkGreen,   "\u001b[48;5;22m" },
            { ConsoleColor.DarkCyan,    "\u001b[48;5;36m" },
            { ConsoleColor.DarkRed,     "\u001b[48;5;52m" },
            { ConsoleColor.DarkMagenta, "\u001b[48;5;90m" },
            { ConsoleColor.DarkYellow,  "\u001b[48;5;138m" },
            { ConsoleColor.DarkGray,    "\u001b[48;5;242m" },
            { ConsoleColor.Gray,        "\u001b[48;5;252m" },
            { ConsoleColor.Black,       "\u001b[40m" },
            { ConsoleColor.Red,         "\u001b[41m" },
            { ConsoleColor.Green,       "\u001b[42m" },
            { ConsoleColor.Yellow,      "\u001b[43m" },
            { ConsoleColor.Blue,        "\u001b[44m" },
            { ConsoleColor.Magenta,     "\u001b[45m" },
            { ConsoleColor.Cyan,        "\u001b[46m" },
            { ConsoleColor.White,       "\u001b[47m" }
        });
    }
}
