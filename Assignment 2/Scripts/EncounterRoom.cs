// File: EncounterRoom.cs
using System;

namespace Dungeon
{
    public class EncounterRoom : Room
    {
        public override string RoomDescription()
        {
            return "A tense arena-like space. You feel watched.";
        }

        public override void OnRoomEntered(GameManager gm, Player player)
        {

            if (!Visited)
            {
                Console.WriteLine(RoomDescription());
                Visited = true;
            }
            else
            {
                Console.WriteLine("You return to the Arena.");
            }

            Console.WriteLine("An opponent appears! Time for a duel! (Poker Rules)");

            gm.StartEncounter();

            Console.WriteLine();
            Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' to check your inventory, 'exit' to end the game");
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            Console.WriteLine("You scout the arena, but there is nothing to loot here.");
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You leave the room...");
        }
    }
}
