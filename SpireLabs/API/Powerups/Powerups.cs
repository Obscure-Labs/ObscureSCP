using ObscureLabs.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Powerups
{
    //public enum Powerup
    //{
    //    SpeedBoost,
    //    InfiniteStamina,
    //    ShrinkRay,
    //    WalkThroughDoors,
    //    DamageBoost,
    //    HealthBoost,
    //    DamageReduction
    //}

    public static class Powerups
    {
        public static List<Powerup> PowerupList = new List<Powerup>();
        
        public static void PopulateList()
        {
            PowerupList.Add(new Speedboost());
        }
    }
}
