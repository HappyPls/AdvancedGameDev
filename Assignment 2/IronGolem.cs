using System;

namespace Dungeon
{
    public class IronGolem : Enemy
    {
        public IronGolem(Random rng)
            : base(
                name: "Iron Golem",
                level: 4,
                maxHp: 200,
                armour: 22,
                damageMin: 12,
                damageMax: 22,
                critChance: 3,
                critMultiplier: 140,
                diceSides: new int[] { 8, 8, 10, 10, 12 },
                rng: rng
            )
        { }
    }
}
