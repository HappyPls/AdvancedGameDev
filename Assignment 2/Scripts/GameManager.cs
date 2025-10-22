using System;

namespace Dungeon
{
    public class GameManager
    {
        public Random Rng = new Random();

        private Map _map = null!;
        private Player _player = null!;
        public bool HasWon { get; private set; }

        public void RunAdventure()
        {
            Console.WriteLine("Welcome to Dicey Dungeon!");
            Console.Write("Enter your hero's name: ");
            string name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
                name = "Hero";

            _map = new Map(rows: 5, cols: 5, rng: Rng);
            _player = new Player(name: name, isComputer: false);
            EnemySpawner.ForceGolemSpawn();

            Console.WriteLine("Welcome, " + _player.Name + "!");
            PrintGameIntro();
            Console.WriteLine();
            PrintPokerRules();
            Console.WriteLine();
            Console.Write("Press Enter to continue...");
            Console.ReadLine();

            bool playing = true;

            while (playing && _player.HP > 0 && !HasWon)
            {
                Room room = _map.CurrentRoom();
                room.OnRoomEntered(this, _player);
                Console.WriteLine();
                Console.WriteLine("(Visited this room " + _map.CurrentRoom().VisitCount + " time(s))");
                Console.WriteLine();
                PrintHelp();

                Console.Write("> ");
                string cmd = (Console.ReadLine() ?? "").Trim().ToLower();

                if (cmd == "exit" || cmd == "quit")
                {
                    playing = false;
                }
                else if (cmd == "help" || cmd == "h" || cmd == "?")
                {
                    PrintHelp();
                }
                else if (cmd == "inv" || cmd == "inventory")
                {
                    InventoryMenu menu = new InventoryMenu();
                    menu.Open(_player);
                }
                else if (cmd == "search")
                {
                    room.OnRoomSearched(this, _player);
                }
                else if (cmd == "north" || cmd == "south" || cmd == "east" || cmd == "west" ||
                         cmd == "n" || cmd == "s" || cmd == "e" || cmd == "w")
                {
                    bool moved = _map.TryMove(cmd);
                    if (moved)
                        room.OnRoomExit(this, _player);
                    else
                        Console.WriteLine("You cannot go that way.");
                }
                else
                {
                    Console.WriteLine("Unknown command. Type 'help' for commands.");
                }

                if (_player.HP <= 0)
                {
                    Console.WriteLine("You collapse from your wounds...");
                    break;
                }
            }

            if (HasWon)
                Console.WriteLine("You defeated the Iron Golem and escape the dungeon!");

            Console.WriteLine("Thanks for playing!");
        }

        public void StartEncounter()
        {
            int prevRow = _map.Row;
            int prevCol = _map.Col;

            Enemy enemy = EnemySpawner.SpawnRandomEnemy(Rng);

            CombatMenu cm = new CombatMenu();
            cm.StartCombat(_player, enemy, _map, prevRow, prevCol);

            if (enemy.HP <= 0)
            {
                var enc = _map.CurrentRoom() as EncounterRoom;
                if (enc != null) enc.Cleared = true;

                var boss = _map.CurrentRoom() as BossEncounterRoom;
                if (boss != null) boss.Cleared = true;

                if (enemy is IronGolem)
                {
                    HasWon = true;
                    Console.WriteLine("You have slain the Iron Golem!");
                    return;
                }

                Console.WriteLine("You search the foe's remains.");
                GrantRandomLoot(_player, 1, 2, LootBias.ConsumablesLean);
            }
        }

        public void GiveItem(Player player, Item item)
        {
            if (item == null) return;

            if (item.Type == ItemType.Weapon)
            {
                player.AddWeapon((Weapon)item);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("You found a weapon: " + item.Name);
                Console.ResetColor();
            }
            else if (item.Type == ItemType.Armour)
            {
                player.AddArmour((ArmourItem)item);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("You found armour: " + item.Name);
                Console.ResetColor();
            }
            else
            {
                player.AddConsumable((Consumable)item);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("You found a consumable: " + item.Name);
                Console.ResetColor();
            }
        }

        public void GrantRandomLoot(Player player, int minCount, int maxCount, LootBias bias)
        {
            if (minCount < 1) minCount = 1;
            if (maxCount < minCount) maxCount = minCount;

            int count = Rng.Next(minCount, maxCount + 1);
            for (int i = 0; i < count; i++)
            {
                Item item = LootSystem.RandomItem(Rng, bias);
                GiveItem(player, item);
            }
        }

        public void PrintDiceBundle(int[] sidesBundle)
        {
            if (sidesBundle == null || sidesBundle.Length == 0)
            {
                Console.WriteLine("(no dice)");
                return;
            }

            Console.Write("Dice: ");
            for (int i = 0; i < sidesBundle.Length; i++)
            {
                Console.Write("d" + sidesBundle[i]);
                if (i < sidesBundle.Length - 1) Console.Write(" ");
            }
            Console.WriteLine();
        }

        private void PrintHelp()
        {
            Console.WriteLine("Commands for the game are:");
            Console.WriteLine("  north/south/east/west (or n/s/e/w)  - move");
            Console.WriteLine("  search                              - search the room");
            Console.WriteLine("  inv / inventory                     - open inventory menu");
            Console.WriteLine("  help                                - show this help");
            Console.WriteLine("  exit / quit                         - leave the game");
        }

        private void PrintGameIntro()
        {
            Console.WriteLine("You wake up in a dungeon with no memory of how you got here.");
            Console.WriteLine("You know that you have to kill an Iron Golem to get an exit stone to leave the place");
            Console.WriteLine("What do you do?");
        }
        private void PrintPokerRules()
        {
            Console.WriteLine();
            Console.WriteLine("== POKER HAND RANKINGS ==");
            Console.WriteLine("Combat in this game depends on the dice you roll!");
            Console.WriteLine("Your attack power depends on your dice combination. Here’s the ranking from strongest to weakest:");
            Console.WriteLine("   Five of a Kind   - All dice show the same number. (Highest damage multiplier)");
            Console.WriteLine("   Four of a Kind   - Four dice of the same number.");
            Console.WriteLine("   Full House       - Three of one number and two of another.");
            Console.WriteLine("   Straight         - Five dice in sequence (e.g., 2-3-4-5-6).");
            Console.WriteLine("   Three of a Kind  - Three dice showing the same number.");
            Console.WriteLine("   Two Pair         - Two different pairs (e.g., 3-3 and 5-5).");
            Console.WriteLine("   One Pair         - Two dice showing the same number.");
            Console.WriteLine("   High Card        - None of the above; the highest die decides base damage.");
            Console.WriteLine();
            Console.WriteLine("Higher hands give stronger multipliers — aim for combinations!");
        }
    }
}
