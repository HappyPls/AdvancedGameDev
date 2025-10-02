using System;

namespace Lab4DiceThrowing
{
    internal class Program
    {
        static void Main()
        {
            var gm = new GameManager();
            gm.RunBestOfFive();

            Console.WriteLine();
            Console.WriteLine("Press Enter to Exit.");
            Console.ReadLine();
        }
    }
}
