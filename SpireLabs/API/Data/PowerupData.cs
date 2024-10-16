using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Data
{
    public abstract class Powerup
    {
        public abstract string Name { get; }
        public abstract bool UseInstantly { get; }
        public abstract float EffectLength { get; }

        public virtual void Use(Player p)
        {
            Log.Debug($"Powerup {Name} used by {p.Nickname}");
        }
    }
}
