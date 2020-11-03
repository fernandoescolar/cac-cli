using Cac.Exceptions;
using Cac.Output;
using System;
using System.IO;

namespace Cac.ErrorHandling
{
    public static class ExceptionHandler
    {
        public static void Handle(Exception ex, IOutput output, string filePath)
        {
            Environment.ExitCode = HandleException(ex, output, filePath);

            output.Verbose.WriteLine("Exception details:");
            output.Verbose.WriteLine(ex.ToString());
        }

        private static int HandleException(Exception ex, IOutput output, string filePath) {
            var result = ex switch
            {
                ExpressionEvaluationException e => WriteError(output, filePath, $"Error evaluating expression: {e.Message}", e.Line, e.Column),
                PropertyNotFoundException e => WriteError(output, filePath, e.Message, e.Line, e.Column),
                UnexpectedBeginOfLineException e => WriteError(output, filePath, e.Message, e.Line),
                UnexpectedCharacterException e => WriteError(output, filePath, e.Message, e.Line, e.Column),
                UnexpectedEndOfLineException e => WriteError(output, filePath, e.Message, e.Line),
                UnexpectedTypeException e => WriteError(output, filePath, e.Message, e.Line, e.Column),
                UnexpectedYamlObjectException e => WriteError(output, filePath, e.Message, e.Line, e.Column),
                CanNotDownloadPackageException => WriteError(output, filePath, ex.Message),
                _ => HandleUnknownException(ex, output, filePath)
            };

            //if (ex.InnerException != null && result != 2)
            //{
            //    HandleException(ex.InnerException, output, filePath);
            //}

            return result;
        }

        private static int HandleUnknownException(Exception ex, IOutput output, string filePath)
        {
            if (ex.InnerException != null)
            {
                Handle(ex.InnerException, output, filePath);
            }
            else
            {
                WriteError(output, filePath, $"Unexpected exception: {ex.Message}");
            }

            return 2;
        }

        private static int WriteError(IOutput output, string filePath, string message, int? line = null, int? column = null)
        {
            if (output == null)
            {
                Console.WriteLine("Error: can not initialize output object!");
                return -1;
            }

            output.ResetSections();
            output.WriteLine(string.Empty);
            output.WriteLine("-------------------------------------------------------------------", ConsoleColor.Red);

            if (!File.Exists(filePath))
            {
                output.Error($"File does not exist: {filePath}");
                return -1;
            }
           
            output.Write("There was an ");
            output.Write("error", ConsoleColor.Red);
            output.Write(" processing ");
            output.Write(filePath, ConsoleColor.Cyan);
            output.Write(" file");
            output.WriteLine(string.Empty);


            if (!string.IsNullOrWhiteSpace(message))
            {
                output.Error(message);
                output.WriteLine(string.Empty);
            }

            if (line.HasValue)
            {
                output.Write("In line ");
                output.Write((line.Value + 1).ToString(), ConsoleColor.Cyan);
                if (column.HasValue)
                {
                    output.Write(" and column ");
                    output.Write(column.Value.ToString(), ConsoleColor.Cyan);
                }

                output.WriteLine(string.Empty);

                var content = File.ReadAllText(filePath);
                var lines = content.Split("\n");

                if (line.Value > 1)
                {
                    output.WriteLine(lines[line.Value - 2], ConsoleColor.White);
                }

                if (line.Value > 0)
                {
                    output.WriteLine(lines[line.Value - 1], ConsoleColor.White);
                }

                output.WriteLine(lines[line.Value], ConsoleColor.White);
                if (column.HasValue && column.Value > 0)
                {
                    output.WriteLine(new string('_', column.Value) + "^", ConsoleColor.DarkCyan);
                    output.WriteLine(string.Empty);
                }
            }

            return 1;
        }
    }
}
