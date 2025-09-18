using System;
using System.Collections.Generic;

namespace Lab3GameManager
{
    public class DieRoller
    {
        private readonly Random rng;

        public DieRoller()
        {
            rng = new Random();
        }

        public (int total, List<(int sides, int roll)> rolls) Roll(params int[] sides)
        {
            var results = new List<(int sides, int roll)>();
            int total = 0;

            foreach (int s in sides)
            {
                int roll = rng.Next(1, s + 1);
                results.Add((s, roll));
                total += roll;
            }

            return (total, results);
        }
    }
}
