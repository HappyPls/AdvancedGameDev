using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace Lab4DiceThrowing
{
    /// <summary>
    /// Orchestrates one round of dice rolling.
    /// intro -> name -> both choose TWO DISTINCT dice from {d6,d8,d10,d12} -> roll -> rank like poker -> winner or tie reroll -> summary.
    /// </summary>
    public class GameManager
    {
        //Random number generator for coin flips and computer choices
        private Random _rng = new Random();

        private Player? _player; //Human Player
        private Player _ai = new Player("Computer"); //AI

        //Set of dices that each player owns
        private string[] _allowedSet = new string[] { "d6", "d8", "d10", "d12" };
        public void RunOneRound()
        {
            //------ Start: Introduction to game ------
            Intro();


            //Ask Player for their name
            Console.Write("Enter your name: ");
            _player = new Player(Console.ReadLine() ?? "");

            //Show Game Rules
            Console.WriteLine();
            Console.WriteLine("You & Computer each have: d6, d8, d10, d12");
            Console.WriteLine("Pick TWO DIFFERENT dice (e.g., d12 d10).");


            // Player chooses two dices
            var playerPick = ChooseTwoDices(_player);

            // AI chooses two distinct tokens
            var aiPick = AutoPickTwoDices();
            Console.WriteLine();
            Console.WriteLine("Computer chooses: " + aiPick.tokenA + " & " + aiPick.tokenB + ".");

            // Roll for both players
            var playerRolls = RollTwo(playerPick.tokenA, playerPick.tokenB);
            var aiRolls = RollTwo(aiPick.tokenA, aiPick.tokenB);

            //Show Results of the rolls
            AnnounceTwo(_player, playerPick, playerRolls);
            AnnounceTwo(_ai, aiPick, aiRolls);


            //Evaluate the hands to find out what kind of hand each player has
            var humanHand = EvaluateHand(playerRolls.a, playerRolls.b);
            var aiHand = EvaluateHand(aiRolls.a, aiRolls.b);

            //Compare Results
            int result = CompareHands(humanHand, aiHand);

            Console.WriteLine();
            if (result == 0)
            {
                Console.WriteLine("It's a draw! No points awarded.");
            }
            else
            {
                Player winner = result > 0 ? _player : _ai;
                Player loser = Object.ReferenceEquals(winner, _player) ? _ai : _player;

                Console.WriteLine("Winner: " + winner.Name + "(+1 Point)");
                winner.Score += 1;
            }

            Outro();
        }

        //------ Displays the Intro Message ------
        private static void Intro()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            Console.WriteLine("======================================");
            Console.WriteLine("          Lab 4 - Dice Poker          ");
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("           Date: " + today            );
            Console.WriteLine();
            Console.WriteLine("Rules: Each side picks TWO dices from \n" +
                              "         {d6, d8, d10, d12} \n" +
                              "              and rolls.");
            Console.WriteLine();
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("  Hands: Pair > Straight > High Card.");
            Console.WriteLine();
            Console.WriteLine("======================================");
        }

        //------ Displays the Outro Message ------
        private static void Outro()

        {
            Console.WriteLine();
            Console.WriteLine("Thanks for playing one round!");
        }

        //------ Player chooses 2 dices ------
        private (string tokenA, string tokenB) ChooseTwoDices(Player p)
        {
            while (true)
            {
                Console.Write("Choose two dice (tokens) from (d6, d8, d10, d12), separated by space: ");
                string line = Console.ReadLine() ?? "";
                line = line.ToLower();

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                {
                    Console.WriteLine("You must enter exactly TWO tokens, e.g. d12 d10.");
                    continue;
                }

                string a = parts[0];
                string b = parts[1];

                // validate both dices properly
                if (!DiceRoller.IsAllowedToken(a) || !DiceRoller.IsAllowedToken(b))
                {
                    Console.WriteLine("Invalid choice. Allowed: d6, d8, d10, d12.");
                    continue;
                }

                if (a == b)
                {
                    Console.WriteLine("They must be DIFFERENT (you only own one of each).");
                    continue;
                }

                Console.WriteLine("You chose: " + a + " and " + b + ".");
                return (a, b);
            }
        }
        //------ Computer Auto picks 2 dices ------
        private (string tokenA, string tokenB) AutoPickTwoDices()
        {
            int roll = _rng.Next(0, 100);
            if (roll < 60) return ("d12", "d10");
            if (roll < 80) return ("d12", "d8");
            if (roll < 90) return ("d10", "d8");
            return ("d12", "d6");
        }


        //------ Roll Two Dices ------
        private (int a, int b) RollTwo(string tokenA, string tokenB)
        {
            int sidesA = DiceRoller.TokenToSides(tokenA);
            int sidesB = DiceRoller.TokenToSides(tokenB);
            int rollA = DiceRoller.Roll(sidesA);
            int rollB = DiceRoller.Roll(sidesB);
            return (rollA, rollB);
        }

        //------ Show Results for Both Dices ------
        private void AnnounceTwo(Player p, (string tokenA, string tokenB) pick, (int a, int b) rolls) 
        { 
            string calloutA = Callout(pick.tokenA, rolls.a); 
            string calloutB = Callout(pick.tokenB, rolls.b); 
            Console.WriteLine(p.Name + " rolls " + pick.tokenA + ": " + rolls.a + calloutA + " | " + pick.tokenB + ": " + rolls.b + calloutB); } 

        //------ Callout results ------
        private static string Callout(string token, int roll) 
        { 
            int sides = DiceRoller.TokenToSides(token); 
            double avg = (sides + 1) / 2.0; if (roll == sides) return " — Maximum!"; 
            if (roll == 1) return " — Oh No!"; 
            if (roll > avg) return " — Above Avg!"; return ""; }

        //------ Evaluate Hand (pair/straight/high card) ------
        private struct Hand
        {
            public int Rank;     // Pair=3, Straight=2, High=1
            public int Hi;       // highest die
            public int Low;      // lowest die
            public string Label; // text label for printing
        }
        private static Hand EvaluateHand(int x, int y)
        {
            int hi = x >= y ? x : y;
            int low = x >= y ? y : x;

            if (x == y)
                return new Hand { Rank = 3, Hi = hi, Low = low, Label = "Pair (" + hi + "s)" };

            if (hi - low == 1)
                return new Hand { Rank = 2, Hi = hi, Low = low, Label = "Straight (" + low + "-" + hi + ")" };

                return new Hand { Rank = 1, Hi = hi, Low = low, Label = "High (" + hi + "," + low + ")" };
        }

        //------ Compare both hands ------
        private static int CompareHands(Hand a, Hand b)
        {
            if (a.Rank != b.Rank) return a.Rank > b.Rank ? 1 : -1;
            if (a.Hi != b.Hi) return a.Hi > b.Hi ? 1 : -1;
            if (a.Low != b.Low) return a.Low > b.Low ? 1 : -1;
            return 0;
        }


        //------ Player Class ------
        private class Player
        {
            public string Name;
            public int Score;

            public Player(string name)
            {
                if (String.IsNullOrWhiteSpace(name)) Name = "Player";
                else Name = name.Trim();
                Score = 0;
            }
        }
    }
}
