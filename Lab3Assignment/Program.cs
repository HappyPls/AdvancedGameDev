using System;

namespace Lab3GameManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var game = new GameManager(playerName: "Bryan");
            game.Play();
        }
    }
}
