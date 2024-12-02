using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;

namespace ObscureLabs.API.Data
{
    public abstract class EffectBase
    {
        public abstract string Name { get; }
        public abstract float Duration { get; set; }
        public abstract bool IsPermanent { get; set; }
        public abstract Player player { get; set; }

        public virtual void Give()
        {
            Log.Debug("Given Player Effect");
            if (!IsPermanent)
            {
                Timing.CallDelayed(Duration, Remove);
            }
        }

        public virtual void Remove()
        {
            Log.Debug("Removed Player Effect");
        }
    }
}
