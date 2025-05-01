using CustomPlayerEffects;
using PlayerStatsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.StatusEffects
{
    public class Burning : TickingEffectBase
    {
        private float _tickDamage = 5f;
        public override void Enabled()
        {
            base.Enabled();
        }

        public override void OnTick()
        {
            base.Hub.playerStats.DealDamage(new UniversalDamageHandler(this._tickDamage, new DeathTranslation(9, 14, 15, "Burned to death."), null));
        }

        public override void Disabled()
        {
            base.Disabled();
        }


    }
}
