using System;

namespace Lab6Dungeon
{
    public class BasicRoom : Room
    {
        public override string RoomDescription()
        {
            return "A quiet, empty space. You sense no danger here.";
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
                Console.WriteLine("You return to the familiar empty room.");
            }
        }
        public override void OnRoomSearched(GameManager gm, Player player)
        {
            Console.WriteLine("You search the room but find nothing of interest.");
        }

        public override void OnRoomExit(GameManager gm, Player player)
        {
            Console.WriteLine("You quietly leave the empty room behind.");
        }
    }
}
