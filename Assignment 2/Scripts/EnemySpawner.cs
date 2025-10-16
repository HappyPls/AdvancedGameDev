using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dungeon
{
    public class EnemySpawner
    {
        // Spawns a random enemy type from the list
        public static Enemy SpawnRandomEnemy(Random rng)
        {
            int roll = rng.Next(1, 11);

            if (roll == 1)
                return new IronGolem(rng);
            else if (roll <= 3)
                return new ArmouredOrc(rng);
            else if (roll <= 6)
                return new Goblin(rng);
            else
                return new CaveSlime(rng);
        }
    }
}
