using System;

namespace Lab6Dungeon
{
    internal class Program
    {
        static void Main()
        {
            var gm = new GameManager();
            gm.RunAdventure();

            Console.WriteLine();
            Console.WriteLine("Press Enter to Exit.");
            Console.ReadLine();
        }
    }
}
