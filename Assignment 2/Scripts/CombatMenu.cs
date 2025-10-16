using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    public class CombatMenu
    {
        private Random _rng = new Random();

        // Main combat loop
        public void StartCombat(Player player, Enemy enemy, Map map, int prevRow, int prevCol)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"A {enemy.Name} appears!");
            Console.ResetColor();

            while (player.HP > 0 && enemy.HP > 0)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=== COMBAT ===");
                Console.ResetColor();

                Console.WriteLine($"Player HP: {player.HP}/{player.MaxHp}");
                Console.WriteLine($"{enemy.Name} HP: {enemy.HP}/{enemy.MaxHp}");
                Console.WriteLine();
                Console.WriteLine("1) Attack");
                Console.WriteLine("2) Use Consumable");
                Console.WriteLine("3) Flee");
                Console.Write("Choose an option: ");
                string choice = (Console.ReadLine() ?? "").Trim();

                if (choice == "1")
                {
                    // Show dice inventory clearly
                    PrintDiceInventory(player);

                    // Player chooses 5 dice for this attack
                    var diceSet = player.ChooseFiveDice(player.InventorySides);
                    int[] rolls = DiceRoller.RollDice(new int[] { diceSet.a, diceSet.b, diceSet.c, diceSet.d, diceSet.e });

                    // Show rolls and evaluate poker hand
                    DiceRoller.PrintRolls(rolls);
                    var result = DiceRoller.Evaluate5(rolls);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Hand: {result.Name}  (x{result.Multiplier / 100.0:F2} damage)");
                    Console.ResetColor();

                    // Perform attack
                    PlayerAttack(player, enemy, result.Multiplier);

                    // Enemy counterattack
                    if (enemy.HP > 0)
                        EnemyAttack(enemy, player);
                }
                else if (choice == "2")
                {
                    UseConsumable(player);
                    if (enemy.HP > 0 && player.HP > 0)
                        EnemyAttack(enemy, player);
                }
                else if (choice == "3")
                {
                    Console.WriteLine("You flee from the battle!");
                    map.SetPosition(prevRow, prevCol);
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid choice. Try again.");
                }
            }

            // End of combat summary
            if (player.HP <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You have been defeated...");
                Console.ResetColor();
            }
            else if (enemy.HP <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"You defeated the {enemy.Name}!");
                Console.ResetColor();
            }
        }

        // Prints player's dice inventory grouped by type
        private void PrintDiceInventory(Player player)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The dice you have are:");
            Console.ResetColor();

            var grouped = player.InventorySides
                .GroupBy(s => s)
                .OrderBy(g => g.Key)
                .Select(g => new { Sides = g.Key, Count = g.Count() });

            foreach (var g in grouped)
            {
                Console.WriteLine($"d{g.Sides} × {g.Count}");
            }

            Console.WriteLine();
        }

        // Player attacks with multiplier
        private void PlayerAttack(Player player, Enemy enemy, int multiplierPercent)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{player.Name} attacks!");
            Console.ResetColor();

            player.Attack(enemy, multiplierPercent);
        }

        // Enemy attack
        private void EnemyAttack(Enemy enemy, Player player)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{enemy.Name} attacks!");
            Console.ResetColor();

            enemy.Attack(player, 100);
        }

        // Player uses consumable
        private void UseConsumable(Player player)
        {
            if (player.Consumables.Count == 0)
            {
                Console.WriteLine("You have no consumables!");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Consumables:");
            for (int i = 0; i < player.Consumables.Count; i++)
            {
                Consumable c = player.Consumables[i];
                Console.WriteLine($"{i}) {c.Name} (heals {c.HealAmount})");
            }

            Console.Write("Choose a number to use: ");
            string input = Console.ReadLine();
            if (int.TryParse(input, out int index))
            {
                string name = player.Consumables[index].Name;
                bool used = player.UseConsumableByIndex(index);
                if (used)
                {
                    Console.WriteLine($"You used a {name}!");
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
            else
            {
                Console.WriteLine("Please enter a number.");
            }
        }
    }
}
