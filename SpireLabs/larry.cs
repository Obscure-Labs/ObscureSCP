using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp0492;
using Exiled.Events.EventArgs.Scp106;
using MEC;
using PluginAPI.Core.Zones.Pocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpireLabs
{
    internal class larry
    {
        internal static void pdExits(AttackingEventArgs ev)
        {

        }

        internal static void onLarryAttack(AttackingEventArgs ev)
        {
            Timing.RunCoroutine(larryHints(ev));
            

        }

        internal static IEnumerator<float> larryHints(AttackingEventArgs ev)
        {
            guiHandler.sendHint(ev.Player, $"You sent {ev.Target.Nickname} to the pocket dimension!", 5);
            yield return Timing.WaitForSeconds(5);
            guiHandler.sendHint(ev.Player, $"You were sent to the pocket dimension by: {ev.Player.Nickname} as SCP-106!", 5);
        }
    }
}
