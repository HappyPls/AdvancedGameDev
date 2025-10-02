using System;
using System.Collections.Generic;

namespace Lab4DiceThrowing
{
    /// <summary>
    /// Base player data and stats
    /// </summary>
    public class Player
    {
        public string Name;
        public bool IsComputer;
        public int Score;

        //Stats across game session
        public int RollSum;
        public int RollCount;
        public int EvenCount;
        public int OddCount;

        private Random _rng = new Random();

        public Player(string name, bool isComputer)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
            IsComputer = isComputer;
            ResetStats();
        }

        /// <summary>
        /// Resets Score and roll statistics
        /// Called at the start of each match
        /// </summary>
        public void ResetStats()
        {
            Score = 0;
            RollSum = 0;
            RollCount = 0;
            EvenCount = 0;
            OddCount = 0;
        }

        /// <summary>
        /// Records a single die roll into totals, average and even/odd counts
        /// </summary>
        /// <param name="value">Rolled Value by sides</param>
        public void RecordRoll(int value)
        {
            if (value <= 0) return;
            RollSum += value;
            RollCount++;
            if ((value % 2) == 0) EvenCount += 1; 
            else OddCount += 1;
        }

        public (int aSides, int bSides) ChooseTwoDice(List<int> availableSides)
        {
            if (!IsComputer)
            {
                while (true)
                {
                    Console.WriteLine($"{Name}, choose TWO different dice from: {string.Join(", ", availableSides)}");
                    Console.Write("Enter like: d12 d10 (you can also type 12 10): ");

                    string line = (Console.ReadLine() ?? "").Trim().ToLower();
                    string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length != 2)
                    {
                        Console.WriteLine("Please enter exactly two choices. \n");
                        continue;
                    }

                    int a = DiceRoller.ParseSides(parts[0]);
                    int b = DiceRoller.ParseSides(parts[1]);

                    if (a == b || !availableSides.Contains(a) || !availableSides.Contains(b))
                    {
                        Console.WriteLine("Invalid choice. Try again.\n");
                        continue;
                    }

                    return (a, b);
                }
            }
            else
            {
                if (availableSides.Count < 2) return (6, 8);
                int i1 = _rng.Next(availableSides.Count);
                int i2 = i1;
                while (i1 == i2)
                {
                    i2 = _rng.Next(availableSides.Count);
                }
                return (availableSides[i1], availableSides[i2]);
            }
        }
    }
}
