﻿using System;
using System.Collections.Generic;
using System.Text;
using NDesk.Options;
using SiteServer.Utils;

namespace SiteServer.Cli.Core
{
    public static class CliUtils
    {
        public const int PageSize = 500;

        public static readonly string PhysicalApplicationPath = Environment.CurrentDirectory;

        private const int ConsoleTableWidth = 77;

        private static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            return string.IsNullOrEmpty(text)
                ? new string(' ', width)
                : text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
        }

        // https://stackoverflow.com/questions/491595/best-way-to-parse-command-line-arguments-in-c
        public static bool ParseArgs(OptionSet options, string[] args)
        {
            try
            {
                options.Parse(args);
                return true;
            }
            catch (OptionException ex)
            {
                PrintError(ex.Message);
                return false;
            }
        }

        public static void PrintLine()
        {
            Console.WriteLine(new string('-', ConsoleTableWidth));
        }

        public static void PrintRow(params string[] columns)
        {
            int width = (ConsoleTableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        public static void PrintProgressBar(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(Convert.ToDouble(progress / (double)total).ToString("0%") + "    "); //blanks at the end remove any excess
        }

        public static void PrintError(string errorMessage)
        {
            Console.WriteLine();
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(errorMessage);
            Console.ForegroundColor = color;
        }

        public static void LogErrors(string commandName, List<TextLogInfo> logs)
        {
            var builder = new StringBuilder();
            if (logs != null && logs.Count > 0)
            {
                foreach (var log in logs)
                {
                    builder.AppendLine();
                    builder.Append(log);
                    builder.AppendLine();
                }
            }
            FileUtils.WriteText(PathUtils.Combine(PhysicalApplicationPath, $"{commandName}.error.log"), Encoding.UTF8, builder.ToString());
        }
    }
}
