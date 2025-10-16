using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class Combatant
    {
        // Basic identity information
        public string Name { get; protected set; }
        public int Level { get; protected set; }

        // Vital stats for health and defense
        public int MaxHp { get; protected set; }
        public int HP { get; protected set; }
        public int Armour { get; protected set; }

        // Offensive stats for damage and criticals
        public int DamageMin { get; protected set; }
        public int DamageMax { get; protected set; }
        public int CritChance { get; protected set; }
        public int CritMultiplier { get; protected set; }

        // Dice sides used for special rolls or actions
        public int[] DiceSides { get; protected set; }
        protected Random Rng;

        // Constructor initializes the combatant’s stats and dice
        public Combatant(string name, int level, int maxHp, int hP, int armour, int damageMin, int damageMax, int critChance, int critMultiplier, int[] diceSides, Random rng)
        {
            Name = name;
            Level = level;
            MaxHp = maxHp;
            HP = MaxHp;
            Armour = armour;
            DamageMin = damageMin;
            DamageMax = damageMax;
            CritChance = critChance;
            CritMultiplier = critMultiplier;
            DiceSides = diceSides != null ? (int[])diceSides.Clone() : new int[] { 6, 8, 12, 20 };
            Rng = rng ?? new Random();
        }

        // Quick property to check if combatant is still alive
        public bool IsAlive => HP > 0;

        // Handles basic attack logic and color-coded output
        public virtual void Attack(Combatant target, int multiplierPercent)
        {
            int damage = RollDamage();
            if (multiplierPercent < 0) multiplierPercent = 0;
            damage = damage * multiplierPercent / 100;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Name);
            Console.ResetColor();
            Console.Write(" attacks ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(target.Name);
            Console.ResetColor();

            Console.Write(" for ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(damage);
            Console.ResetColor();

            Console.WriteLine(" damage!");
            target.TakeDamage(damage);
        }

        // Determines base and critical damage
        protected virtual int RollDamage()
        {
            int dmg = Rng.Next(DamageMin, DamageMax + 1);
            int roll = Rng.Next(1, 101);

            // Apply critical multiplier if chance succeeds
            if (roll <= CritChance)
            {
                dmg = dmg * CritMultiplier / 100;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Critical hit!");
                Console.ResetColor();
            }

            return dmg;
        }

        // Applies incoming damage after armor reduction
        public virtual void TakeDamage(int amount)
        {
            int reduced = amount - Armour;
            if (reduced < 0) reduced = 0;

            HP -= reduced;
            if (HP < 0) HP = 0;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(Name);
            Console.ResetColor();
            Console.Write(" takes ");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(reduced);
            Console.ResetColor();
            Console.WriteLine(" damage (" + HP + "/" + MaxHp + ")");
        }

        // Restores HP, capped at MaxHp
        public void Heal(int amount)
        {
            HP += amount;
            if (HP > MaxHp) HP = MaxHp;
        }
    }
}
