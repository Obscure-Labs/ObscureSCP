using Exiled.API.Features;
using ObscureLabs.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Powerups
{
    public class Speedboost : Powerup
    {
        public override string Name => "Speedboost";
        public override bool UseInstantly => true;
        public override float EffectLength => 0f;

        public override void Use(Player p)
        {
            p.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 10f);
            p.ChangeEffectIntensity(Exiled.API.Enums.EffectType.MovementBoost, 100, 10f);
            base.Use(p);
        }
    }
}
