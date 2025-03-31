using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp173;
using ObscureLabs.API.Features;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances
{
    internal class SCP173 : Module
    {
        public override string Name => "SCP173";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Scp173.Blinking += Attacking;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds += ActivatingBreackneck;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Scp173.Blinking -= Attacking;
            Exiled.Events.Handlers.Scp173.UsingBreakneckSpeeds -= ActivatingBreackneck;
            return base.Disable();
        }

        public void ActivatingBreackneck(UsingBreakneckSpeedsEventArgs ev)
        {
            if (ev.Scp173.BreakneckActive && ev.Player.Health < 1000f)
            {
                //Manager.setModifier(1, $"<color=red>Deadly Breakneck</color>");

            }

        }

        public void Attacking(BlinkingEventArgs ev)
        {
            if (ev.Scp173.BreakneckActive && ev.Player.Health < 1000f)
            {
                
                foreach (Player p in Player.List)
                {
                    if (p.IsAlive && !p.IsScp)
                    { 
                        float distance = Vector3.Distance(p.Transform.position, ev.Player.Transform.position);
                        if (distance <= 2f)
                        {
                            p.Hurt(1000f, DamageType.Custom.ToString("Cervical Fracture at the base of the skull"));
                            ev.Player.ShowHitMarker();

                        }
                    }
                }
            }
        }
    }
}
