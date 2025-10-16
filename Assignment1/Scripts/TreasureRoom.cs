using Lab6Dungeon;
using System;

namespace Lab6Dungeon
{
    /// <summary>
    /// Treasure Room. Searching can give new dices.
    /// </summary>
    public class TreasureRoom : Room
    {
        private bool _looted = false;

        //Pool to draw from when granting random dice
        private static int[] _possibleDice = new[] { 4, 6, 8, 10, 12, 14, 16, 24 };

        public override string RoomDescription()
        {
            return "A quiet chamber with dusty shelves and a faint glimmer in the corner";
        }

        public override void OnRoomEntered(GameManager gm, Player player)
        {
            VisitCount += 1;
            if (!Visited)
            {
                Console.WriteLine(RoomDescription());
                Console.WriteLine("It looks like there might be something hidden here...");
                Visited = true;
            }
            else
            {
                Console.WriteLine("You return to the treasure chamber.");
                Console.WriteLine($"You have been here {VisitCount} time(s)");
            }
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            if (_looted)
            {
                Console.WriteLine("You have already searched this room. There is nothing left.");
                return;
            }

            int count = gm.Rng.Next(1, 5);

            int[] gained = new int[count];
            for (int i = 0; i < count; i++)
            {
                int idx = gm.Rng.Next(0, _possibleDice.Length);
                int die = _possibleDice[idx];
                player.InventorySides.Add(die);
                gained[i] = die;
            }

            Console.WriteLine("You searched the room and found: ");
            gm.PrintDiceBundle(gained);

            _looted = true;
        }
        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You leave the treasure room...");
        }
    }
}
