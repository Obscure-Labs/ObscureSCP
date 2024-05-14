using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp0492;
using Exiled.Events.EventArgs.Scp106;
using MEC;
using PluginAPI.Core.Zones.Pocket;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs
{
    internal class larry : Plugin.Module
    {
        public override string name { get; set; } = "Larry";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Scp106.Attacking += larry.onLarryAttack;
                Exiled.Events.Handlers.Scp106.Attacking += larry.pdExits;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Scp106.Attacking -= larry.onLarryAttack;
                Exiled.Events.Handlers.Scp106.Attacking -= larry.pdExits;
                base.Disable();
                return true;
            }
            catch { return false; }
        }

        internal static void pdExits(AttackingEventArgs ev)
        {

        }

        internal static void onLarryAttack(AttackingEventArgs ev)
        {
            Timing.RunCoroutine(_1hit(ev));

        }

        private static IEnumerator<float> _1hit(AttackingEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.15f);
            ev.Target.EnableEffect(EffectType.PocketCorroding);
            ev.Target.ChangeEffectIntensity(EffectType.PocketCorroding, 1, 0);
            Manager.SendHint(ev.Player, $"You sent {ev.Target.Nickname} to the pocket dimension!", 5);
            Manager.SendHint(ev.Target, $"You were sent to the pocket dimension by: {ev.Player.Nickname} as SCP-106!", 5);
        }
    }
}
