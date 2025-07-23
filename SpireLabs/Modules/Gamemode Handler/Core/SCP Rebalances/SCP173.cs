using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp173;
using Exiled.Events.Handlers;
using MEC;
using ObscureLabs.API.Features;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Player = Exiled.API.Features.Player;
namespace ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances
{
    internal class Scp173 : Module
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
            if(ev.Scp173.BreakneckActive)
            {
                Timing.RunCoroutine(KillCoroutine(ev.Player));
            }
        }



        public IEnumerator<float> KillCoroutine(Player player)
        {
            yield return Timing.WaitForOneFrame;

            if (player.Health < 1000f)
            {

                foreach (Player p in Player.List)
                {
                    if (p.IsAlive && !p.IsScp)
                    {
                        float distance = Vector3.Distance(p.Transform.position, player.Transform.position);
                        if (distance <= 1.1f)
                        {
                            p.Hurt(1000f, DamageType.Crushed);
                            player.ShowHitMarker();

                        }
                    }
                }
            }

        }
    }
}
