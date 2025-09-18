using System;
using System.Collections.Generic;
using System.Globalization;

namespace Lab3GameManager
{
    /// <summary>
    /// Orchestrates the flow: welcome -> roll dice -> total -> operator explanations -> goodbye.
    /// </summary>
    public class GameManager
    {
        private readonly string _playerName;
        private readonly DieRoller roller;

        public GameManager(string playerName)
        {
            _playerName = playerName;
            roller = new DieRoller();
        }

        public void Play()
        {
            // Welcome line with name + date
            var today = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            Console.WriteLine($"Welcome, {_playerName}!");
            Console.WriteLine($"Date: {today}");
            Console.WriteLine();

            // Roll dice: d6, d8, d12, d20
            Console.WriteLine("Rolling four dice (d6, d8, d12, d20)...");
            (int total, List<(int sides, int roll)> rolls) = roller.Roll(6, 8, 12, 20);

            // Formatted per-die output
            Console.WriteLine();
            Console.WriteLine("Per-die results:");
            Console.WriteLine("Die      Roll");
            foreach (var (sides, roll) in rolls)
            {
                Console.WriteLine($"d{sides,-7}{roll,5}");
            }

            Console.WriteLine($"TOTAL    {total,5}");
            Console.WriteLine();

            // Operator explanations with short examples
            PrintOperatorExplanations();

            // Goodbye
            Console.WriteLine();
            Console.WriteLine("Thanks for playing!");
        }

        private static void PrintOperatorExplanations()
        {
            Console.WriteLine("Arithmetic Operators in C# (with examples)");
            Console.WriteLine();

            int a = 7, b = 5;

            Console.WriteLine("+  (addition): a + b = 7 + 5 = " + (a + b));
            Console.WriteLine("-  (subtraction): a - b = 7 - 5 = " + (a - b));
            Console.WriteLine("*  (multiplication): a * b = 7 * 5 = " + (a * b));

            int x = 7, y = 2;
            Console.WriteLine("/  (division): 7 / 2 = " + (x / y) + " (integer division, truncates)");
            Console.WriteLine("/  (division): 7 / 2.0 = " + (7 / 2.0) + " (double division, keeps decimals)");

            int c = 3;
            Console.WriteLine("++ (increment): c starts 3; c++ returns " + (c++) + ", now c = " + c);
            int d = 3;
            Console.WriteLine("++ (increment): ++d increments first: ++d = " + (++d) + " (now d = " + d + ")");

            int e = 3;
            Console.WriteLine("-- (decrement): e starts 3; e-- returns " + (e--) + ", now e = " + e);
            int f = 3;
            Console.WriteLine("-- (decrement): --f decrements first: --f = " + (--f) + " (now f = " + f + ")");

            Console.WriteLine("%  (modulo): 17 % 5 = " + (17 % 5) + " (remainder after division)");
        }
    }
}
