using System;

namespace Dungeon
{
    public class ArmouredOrc : Enemy
    {
        public ArmouredOrc(Random rng)
            : base(
                name: "Armoured Orc",
                level: 3,
                maxHp: 160,
                armour: 18,
                damageMin: 10,
                damageMax: 20,
                critChance: 5,
                critMultiplier: 150,
                diceSides: new int[] { 6, 8, 10, 10, 12 },
                rng: rng
            )
        { }
    }
}