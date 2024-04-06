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
    internal class larry
    {
        internal static void pdExits(AttackingEventArgs ev)
        {

        }

        internal static void onLarryAttack(AttackingEventArgs ev)
        {
            ev.Target.EnableEffect(EffectType.PocketCorroding);
            ev.Target.ChangeEffectIntensity(EffectType.PocketCorroding, 1, 0);
            Manager.SendHint(ev.Player, $"You sent {ev.Target.Nickname} to the pocket dimension!", 5);
            Manager.SendHint(ev.Target, $"You were sent to the pocket dimension by: {ev.Player.Nickname} as SCP-106!", 5);

        }
    }
}
