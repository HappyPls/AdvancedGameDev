using System;
namespace Lab6Dungeon
{
    /// <summary>
    /// Encounter Room. Entering triggers a poker round (enemy battle)
    /// </summary>
    public class EncounterRoom:Room
    {
        public override string RoomDescription()
        {
            return "A tense arena-like space. You feel watched";
        }

        public override void OnRoomEntered(GameManager gm, Player player)
        {
            VisitCount += 1;

            if (!Visited)
            {
                Console.WriteLine(RoomDescription());
                Visited = true;
            }
            else
            {
                Console.WriteLine("Back to the Arena");
                Console.WriteLine($"You have been here {VisitCount} time(s)");
            }

            Console.WriteLine("An opponent appears! Time for a duel! (Poker Rules)");

            bool playerWon = gm.PlayPokerEncounter(player);

            if (playerWon)
            {
                Console.WriteLine("Victory! You gain spoils from the battle.");
                gm.GrantRandomLoot(player, 1, 4);
            }
            else
            {
                Console.WriteLine("You retreat to recover from your loss...");
            }

            Console.WriteLine();
            Console.WriteLine("Type: north/south/east/west to move, 'search' to search, 'inv' to check your inventory, 'help' for help, 'exit' to leave");
        }

        public override void OnRoomSearched(GameManager gm, Player player)
        {
            Console.WriteLine("You scout the arena, but there is nothing to loot here.");
        }
        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You leave the treasure room...");
        }
    }
}
