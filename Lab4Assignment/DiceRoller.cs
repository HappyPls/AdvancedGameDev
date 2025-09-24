using System;

namespace Lab4DiceThrowing
{
    /// <summary>
    /// Helper class for dice operations.
    /// Supports tokens: d6, d8, d10, d12.
    /// </summary>
    public static class DiceRoller
    {
        // random number generator
        private static Random _rng = new Random();

        // list of allowed dice tokens
        private static string[] _allowed = { "d6", "d8", "d10", "d12" };

        // check if token is valid
        public static bool IsAllowedToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return false;
            token = token.Trim().ToLower();
            foreach (string d in _allowed)
            {
                if (token == d) return true;
            }
            return false;
        }

        // convert token (like "d10") to number of sides
        // returns 0 if not valid
        public static int TokenToSides(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return 0;

            token = token.Trim().ToLower();
            if (token == "d6") return 6;
            if (token == "d8") return 8;
            if (token == "d10") return 10;
            if (token == "d12") return 12;

            return 0; // not valid
        }

        // roll a die with given number of sides
        // if sides < 2, just return 0
        public static int Roll(int sides)
        {
            if (sides < 2) return 0;
            return _rng.Next(1, sides + 1);
        }
    }
}
