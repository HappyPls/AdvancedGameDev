using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Lab6Dungeon
{
    /// <summary>
    /// Poker-style dice game (Best-of-5).
    /// - Player can choose the dice set (e.g., "4 10 20") or press ENTER to use defaults.
    /// - Each round both choose TWO DISTINCT dice, roll both, rank: Pair > Straight > High.
    /// - Ties are draws. First to 3 wins (or 5 rounds max).
    /// </summary>
    public class GameManager
    {
        public Random Rng = new Random();

        // Players
        private Player _human = new Player("Player", false);
        private Player _ai = new Player("Computer", true);

        // -------- Adventure Grid + Rooms + Inventory --------

        /// <summary>
        /// Runs the Adventure with rooms, search encounters and inventory
        /// </summary>
        public void RunAdventure()
        {
            AdventureIntro();

            Console.WriteLine("What is your name, traveller?");
            string name = (Console.ReadLine() ?? "").Trim(); ;
            if (name == "") name = "Player";
            _human = new Player(name,false);

            var map = new Map(rows: 3, cols: 3, rng: Rng);

            Console.WriteLine();
            Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' to check your inventory, 'help' for help, 'exit' to leave");
            Console.WriteLine();

            bool running = true;

            while (running)
            {
                Room room = map.CurrentRoom();
                room.OnRoomEntered(this, _human);

                Console.WriteLine();
                Console.WriteLine("Your move: ");
                string cmd = (Console.ReadLine() ?? "").Trim().ToLower();

                if (cmd == "n" || cmd == "north" || cmd == "s" || cmd == "south" ||
                    cmd == "e" || cmd == "east" || cmd == "w" || cmd == "west")
                {
                    if (map.TryMove(cmd))
                    {
                        room.OnRoomExit(this, _human);
                        Console.WriteLine("You move " + cmd + "...");
                    }
                    else
                    {
                        Console.WriteLine("You can't go that way.");
                    }
                }
                else if (cmd == "search")
                {
                    room.OnRoomSearched(this, _human);
                }
                else if (cmd == "inv")
                {
                    PrintInventory();
                }
                else if (cmd == "help")
                {
                    Console.WriteLine("Commands: n/s/e/w to move, 'search' to look around, 'inv' to see items, 'exit' to leave.");
                }
                else if (cmd == "exit")
                {
                    Console.WriteLine("You decide to leave the area. Your journey continues elsewhere...");
                    running = false;
                }
                else
                {
                    Console.WriteLine("Unknown command. Try again.");
                }
                Console.WriteLine();
            }
            AdventureOutro();
        }
        private void AdventureIntro()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            Console.WriteLine("======================================");
            Console.WriteLine("        Lab 6 - Rooms & Inventory     ");
            Console.WriteLine("======================================");
            Console.WriteLine("            Date: " + today);
            Console.WriteLine("Explore rooms, search for treasure, and win dice encounters!");
            Console.WriteLine("======================================");
            Console.WriteLine();
        }

        private void AdventureOutro()
        {
            Console.WriteLine("Thanks for exploring! See you next time.");
        }

        /// <summary>
        /// Pretty inventory print with counts: d6 x 1, d8 x 4
        /// </summary>
        private void PrintInventory()
        {
            if (_human.InventorySides.Count == 0)
            {
                Console.WriteLine("Inventory is empty.");
                return;
            }

            var counts = CountBySides(_human.InventorySides);

            Console.WriteLine("Inventory:");
            foreach (var kv in Sorted(counts))
            {
                Console.WriteLine($"d{kv.Key} x {kv.Value}");
            }
        }

        /// <summary>
        /// Award a random bundle of dice to the player.
        /// </summary>
        public void GrantRandomLoot(Player player, int minCount, int maxCount)
        {
            if (minCount < 1) minCount = 1;
            if (maxCount < minCount) maxCount = minCount;

            int[] pool = new[] { 4, 6, 8, 10, 12, 14, 16, 24 };
            int grant = Rng.Next(minCount, maxCount + 1);

            int[] gained = new int[grant];
            for (int i = 0; i < grant; i++)
            {
                int idx = Rng.Next(0, pool.Length);
                int die = pool[idx];
                player.InventorySides.Add(die);
                gained[i] = die;
            }

            PrintDiceBundle(gained);
        }

        /// <summary>
        /// Helper: print a bundle like "d6 x 1", "d8 x 2".
        /// </summary>
        public void PrintDiceBundle(int[] gained)
        {
            var counts = CountBySides(gained);
            foreach (var kv in Sorted(counts))
            {
                Console.WriteLine($"  d{kv.Key} x {kv.Value}");
            }
        }

        /// <summary>
        /// Returns a dictionary of counts by sides
        /// </summary>
        private Dictionary<int, int> CountBySides(IEnumerable<int> sidesList)
        {
            var map = new Dictionary<int, int>();
            foreach (int s in sidesList)
            {
                if (s < 2) continue;
                if (!map.ContainsKey(s)) map[s] = 0;
                map[s] += 1;
            }
            return map;
        }

        /// <summary>
        /// Sorts counts by sides
        /// </summary>
        private List<KeyValuePair<int, int>> Sorted(Dictionary<int, int> counts)
        {
            var list = new List<KeyValuePair<int, int>>(counts);
            list.Sort((a, b) => a.Key.CompareTo(b.Key));
            return list;
        }

        /// <summary>
        /// Plays ONE poker-style round vs a computer opponent.
        /// Available dice = base set (permanent) + inventory (consumable).
        /// If the human uses an inventory die, one copy is removed after the round.
        /// Returns true if the human wins.
        /// </summary>
        public bool PlayPokerEncounter(Player human)
        {
            var cpu = new Player("CPU", true);

            // Base, permanent dice:
            var baseSet = new HashSet<int> { 6, 8, 12, 20 };

            // Merge base + inventory to offer choices
            var choiceSet = new HashSet<int>(baseSet);
            for (int i = 0; i < human.InventorySides.Count; i++) choiceSet.Add(human.InventorySides[i]);

            var available = new List<int>(choiceSet);
            available.Sort();

            Console.WriteLine();
            Console.WriteLine("Available dice for this fight: " + string.Join(", ", available.ConvertAll(s => "d" + s)));

            var (hA, hB) = human.ChooseTwoDice(available);
            var (cA, cB) = cpu.ChooseTwoDice(available);

            Console.WriteLine($"{human.Name} picked d{hA} & d{hB}.  {cpu.Name} picked d{cA} & d{cB}.");

            int h1 = DiceRoller.Roll(hA);
            int h2 = DiceRoller.Roll(hB);
            int c1 = DiceRoller.Roll(cA);
            int c2 = DiceRoller.Roll(cB);

            human.RecordRoll(h1); human.RecordRoll(h2);
            cpu.RecordRoll(c1); cpu.RecordRoll(c2);

            Console.WriteLine($"{human.Name} rolls {h1} and {h2}.");
            Console.WriteLine($"{cpu.Name} rolls {c1} and {c2}.");

            var hHand = EvaluateHand(h1, h2);
            var cHand = EvaluateHand(c1, c2);

            Console.WriteLine($"{human.Name} Hand: {hHand.Label}");
            Console.WriteLine($"{cpu.Name} Hand: {cHand.Label}");

            int cmp = CompareHands(hHand, cHand);

            // Consume selected dice from inventory if they exist there (base dice do not get consumed)
            ConsumeIfInInventory(human, hA);
            ConsumeIfInInventory(human, hB);

            if (cmp > 0)
            {
                human.Score += 1;
                Console.WriteLine($"{human.Name} wins the encounter! (+1 point)");
                return true;
            }
            else if (cmp < 0)
            {
                cpu.Score += 1;
                Console.WriteLine($"{cpu.Name} wins the encounter! (+1 point to the foe)");
                return false;
            }
            else
            {
                Console.WriteLine("Encounter is a draw. No points awarded.");
                return false;
            }
        }

        /// <summary>
        /// If 'sides' exists in player's inventory, remove ONE copy (consuming the die).
        /// Writes a note when consumed.
        /// </summary>
        private void ConsumeIfInInventory(Player human, int sides)
        {
            int idx = human.InventorySides.IndexOf(sides);
            if (idx >= 0)
            {
                human.InventorySides.RemoveAt(idx);
                Console.WriteLine($"(Consumed one d{sides} from your inventory.)");
            }
        }

        /// <summary>
        /// Central dice catalog: sides -> token (e.g., 12 -> "d12").
        /// Gets built from the player's input (or defaults if they skip).
        /// </summary>
        private Dictionary<int, string> _diceMap = new Dictionary<int, string>();

        /// <summary>
        /// Starts a Best-of-5 poker match with an optional custom dice set.
        /// </summary>
        public void RunBestOfFive()
        {
            Intro();

            // Name
            Console.Write("Enter your name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (name == "") name = "Player";
            _human = new Player(name, false);

            // Choose dice set
            SetupDiceCatalog();

            // Rules (now that we know the set)
            Console.WriteLine();
            Console.WriteLine("You & Computer can choose from: " + FormatDiceList());
            Console.WriteLine("Each round, pick TWO DIFFERENT dice (e.g., " + SuggestExamplePair() + ").");
            Console.WriteLine("Hands: Pair > Straight > High Card. Best-of-5 (first to 3 wins).");
            Console.WriteLine();

            // Best-of-5 loop
            _human.Score = 0;
            _ai.Score = 0;

            int maxRounds = 5;
            int winsToTakeMatch = 3;
            var availableSides = new List<int>(_diceMap.Keys);
            availableSides.Sort();

            for (int round = 1; round <= maxRounds; round++)
            {
                Console.WriteLine();
                Console.WriteLine("=== Round " + round + " ===");

                bool humanFirst = Rng.Next(0, 2) == 0;
                Console.WriteLine(humanFirst
                    ? _human.Name + " considers their choice first..."
                    : _ai.Name + " considers their choice first...");

                var playerPick = ChooseTwoTokens(availableSides);
                var aiPick = AutoPickTwoTokens(availableSides);

                Console.WriteLine(_human.Name + " picks " + playerPick.tokenA + " & " + playerPick.tokenB + ".");
                Console.WriteLine(_ai.Name + " picks " + aiPick.tokenA + " & " + aiPick.tokenB + ".");

                var playerRolls = RollTwo(playerPick.tokenA, playerPick.tokenB);
                var aiRolls = RollTwo(aiPick.tokenA, aiPick.tokenB);

                AnnounceTwo(_human, playerPick, playerRolls);
                AnnounceTwo(_ai, aiPick, aiRolls);

                var playerHand = EvaluateHand(playerRolls.a, playerRolls.b);
                var aiHand = EvaluateHand(aiRolls.a, aiRolls.b);

                Console.WriteLine(_human.Name + " Hand: " + playerHand.Label);
                Console.WriteLine(_ai.Name + " Hand: " + aiHand.Label);

                int result = CompareHands(playerHand, aiHand);
                if (result == 0)
                {
                    Console.WriteLine("Round is a draw. No points awarded.");
                }
                else
                {
                    Player winner = result > 0 ? _human : _ai;
                    Console.WriteLine("Winner: " + winner.Name + " (+1 point)");
                    winner.Score += 1;
                }

                Console.WriteLine("Score — " + _human.Name + ": " + _human.Score + " | " + _ai.Name + ": " + _ai.Score);

                if (_human.Score >= winsToTakeMatch || _ai.Score >= winsToTakeMatch)
                {
                    Console.WriteLine("Someone reached the required wins for Best-of-5!");
                    break;
                }
            }

            // Summary
            Console.WriteLine();
            Console.WriteLine("=== Match Summary ===");
            Console.WriteLine("Final Score — " + _human.Name + ": " + _human.Score + " | " + _ai.Name + ": " + _ai.Score);
            if (_human.Score > _ai.Score) Console.WriteLine(_human.Name + " wins the match!");
            else if (_ai.Score > _human.Score) Console.WriteLine(_ai.Name + " wins the match!");
            else Console.WriteLine("The match ends in a tie!");

            Outro();
        }

        /// <summary>
        /// Lets the player enter a custom list of dice (e.g., "4 10 20" or "d4 d10 d20").
        /// If they press ENTER or input is bad, we use a default set.
        /// </summary>
        private void SetupDiceCatalog()
        {
            Console.WriteLine();
            Console.WriteLine("Enter custom dice (e.g., 4 10 20 or d4 d10 d20).");
            Console.Write("Or press ENTER for default (6 8 12 20): ");

            string line = (Console.ReadLine() ?? "").Trim();
            if (line == "")
            {
                UseDefaultDice();
                Console.WriteLine("Using default dice: " + FormatDiceList());
                return;
            }

            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var unique = new HashSet<int>();

            for (int i = 0; i < parts.Length; i++)
            {
                int s = DiceRoller.ParseSides(parts[i]);
                if (s >= 2) unique.Add(s);
            }

            if (unique.Count < 2)
            {
                Console.WriteLine("Not enough valid dice. Using defaults.");
                UseDefaultDice();
                Console.WriteLine("Using default dice: " + FormatDiceList());
                return;
            }

            _diceMap.Clear();
            var list = new List<int>(unique);
            list.Sort();
            for (int i = 0; i < list.Count; i++)
            {
                _diceMap[list[i]] = "d" + list[i].ToString();
            }

            Console.WriteLine("Using dice: " + FormatDiceList());
        }

        /// <summary>
        /// Fills _diceMap with the default set.
        /// </summary>
        private void UseDefaultDice()
        {
            _diceMap.Clear();
            _diceMap[6] = "d6";
            _diceMap[8] = "d8";
            _diceMap[12] = "d12";
            _diceMap[20] = "d20";
        }

        /// <summary>
        /// Introduction banner and date.
        /// </summary>
        private static void Intro()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            Console.WriteLine("======================================");
            Console.WriteLine("          Lab 4 - Dice Poker          ");
            Console.WriteLine("======================================");
            Console.WriteLine();
            Console.WriteLine("           Date: " + today);
            Console.WriteLine();
            Console.WriteLine("  Hands: Pair > Straight > High Card.");
            Console.WriteLine("======================================");
        }

        /// <summary>
        /// Closing message.
        /// </summary>
        private static void Outro()
        {
            Console.WriteLine();
            Console.WriteLine("Thanks for playing!");
        }

        /// <summary>
        /// Prompts the human for two tokens; validates against _diceMap; re-prompts until valid.
        /// Accepts "d12 d10" or "12 10".
        /// </summary>
        private (string tokenA, string tokenB) ChooseTwoTokens(List<int> allowedSides)
        {
            while (true)
            {
                Console.Write("Choose two dice " + FormatDiceList() + ", separated by space: ");
                string line = (Console.ReadLine() ?? "").Trim().ToLower();
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                {
                    Console.WriteLine("You must enter exactly TWO choices, e.g. " + SuggestExamplePair() + ".");
                    continue;
                }

                int aSides = ParseTokenToSidesInSet(parts[0], allowedSides);
                int bSides = ParseTokenToSidesInSet(parts[1], allowedSides);

                if (aSides == 0 || bSides == 0)
                {
                    Console.WriteLine("Invalid choice. Allowed: " + FormatDiceList());
                    continue;
                }

                if (aSides == bSides)
                {
                    Console.WriteLine("They must be DIFFERENT.");
                    continue;
                }

                return (_diceMap[aSides], _diceMap[bSides]);
            }
        }

        /// <summary>
        /// Computer chooses two distinct tokens (slight bias toward larger dice).
        /// </summary>
        private (string tokenA, string tokenB) AutoPickTwoTokens(List<int> allowedSides)
        {
            var sorted = new List<int>(allowedSides);
            sorted.Sort();

            int roll = Rng.Next(0, 100);
            if (sorted.Count >= 2 && roll < 70)
            {
                int a = sorted[sorted.Count - 1];
                int b = sorted[sorted.Count - 2];
                return (_diceMap[a], _diceMap[b]);
            }

            int i1 = Rng.Next(sorted.Count);
            int i2 = i1;
            while (i2 == i1)
            {
                i2 = Rng.Next(sorted.Count);
            }

            return (_diceMap[sorted[i1]], _diceMap[sorted[i2]]);
        }

        /// <summary>
        /// Rolls two dice from token strings.
        /// </summary>
        private (int a, int b) RollTwo(string tokenA, string tokenB)
        {
            int sidesA = TokenToSidesOrZero(tokenA);
            int sidesB = TokenToSidesOrZero(tokenB);

            int rollA = DiceRoller.Roll(sidesA);
            int rollB = DiceRoller.Roll(sidesB);

            return (rollA, rollB);
        }

        /// <summary>
        /// Prints results for both dice (no callouts).
        /// </summary>
        private void AnnounceTwo(Player p, (string tokenA, string tokenB) pick, (int a, int b) rolls)
        {
            Console.WriteLine(p.Name + " rolls " + pick.tokenA + ": " + rolls.a +
                              " | " + pick.tokenB + ": " + rolls.b);
        }

        /// <summary>
        /// Hand type for two dice: Pair (3) > Straight (2) > High (1), plus tie-breaks and label.
        /// </summary>
        private struct Hand
        {
            public int Rank;
            public int Hi;
            public int Low;
            public string Label;
        }

        /// <summary>
        /// Evaluates two rolls into a Hand (pair, straight, or high card).
        /// </summary>
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

        /// <summary>
        /// Compares two hands. Returns +1 if a>b, -1 if a<b, 0 if equal.
        /// </summary>
        private static int CompareHands(Hand a, Hand b)
        {
            if (a.Rank != b.Rank) return a.Rank > b.Rank ? 1 : -1;
            if (a.Hi != b.Hi) return a.Hi > b.Hi ? 1 : -1;
            if (a.Low != b.Low) return a.Low > b.Low ? 1 : -1;
            return 0;
        }

        /// <summary>
        /// Formats dice list from _diceMap
        /// </summary>
        private string FormatDiceList()
        {
            var keys = new List<int>(_diceMap.Keys);
            keys.Sort();
            var tokens = new List<string>();
            for (int i = 0; i < keys.Count; i++)
            {
                tokens.Add(_diceMap[keys[i]]);
            }
            return "(" + string.Join(", ", tokens) + ")";
        }

        /// <summary>
        /// Suggests an example pair using the two largest dice in _diceMap.
        /// </summary>
        private string SuggestExamplePair()
        {
            var keys = new List<int>(_diceMap.Keys);
            keys.Sort();
            if (keys.Count >= 2)
            {
                int a = keys[keys.Count - 1];
                int b = keys[keys.Count - 2];
                return _diceMap[a] + " " + _diceMap[b];
            }
            return "d6 d8";
        }

        /// <summary>
        /// Parses token ("d10" or "10") to sides ONLY if it exists in allowedSides
        /// Otherwise returns 0.
        /// </summary>
        private int ParseTokenToSidesInSet(string token, List<int> allowedSides)
        {
            int s = DiceRoller.ParseSides(token);
            if (s < 2) return 0;
            return allowedSides.Contains(s) ? s : 0;
        }

        /// <summary>
        /// Converts a token to sides
        /// returns 0 if not recognized in _diceMap.
        /// </summary>
        private int TokenToSidesOrZero(string token)
        {
            int s = DiceRoller.ParseSides(token);
            if (s < 2) return 0;
            return _diceMap.ContainsKey(s) ? s : 0;
        }
    }
}
