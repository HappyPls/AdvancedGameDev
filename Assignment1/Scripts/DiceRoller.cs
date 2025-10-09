using System;

namespace Lab6Dungeon
{
    /// <summary>
    /// Helper class for dice operations.
    /// Supports tokens: d6, d8, d10, d12.
    /// </summary>
    public static class DiceRoller
    {
        // random number generator
        private static Random _rng = new Random();

        /// <summary>
        /// Parses txt like "d10" or "10" into a sides value. Returns 0 if invalid.
        /// </summary>
        /// <param name="input"> User text such as "d8" or "8" </param>
        /// <returns></returns>
        public static int ParseSides(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;
            string t = input.Trim().ToLower();
            if (t.StartsWith("d")) t = t.Substring(1);

            int sides;
            if (!int.TryParse(t, out sides)) return 0;
            if (sides < 2) return 0;
            return sides;
        }
        /// <summary>
        /// Rolls dice in range based in sides. Returns 0 if sides <= 2.
        /// </summary>
        /// <param name="sides"> Number of sides on die </param>
        /// <returns></returns>
        public static int Roll(int sides)
        {
            if (sides < 2) return 0;
            return _rng.Next(1, sides + 1);
        }
    }
}
