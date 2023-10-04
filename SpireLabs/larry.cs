using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp0492;
using Exiled.Events.EventArgs.Scp106;
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
            ev.Player.ShowHint($"You sent {ev.Target.Nickname} to the pocket dimension!");
            ev.Target.Teleport(Room.List.FirstOrDefault(x => x.Type == RoomType.Pocket));
            ev.Target.EnableEffect(EffectType.PocketCorroding, 60);
            ev.Target.ShowHint($"You were sent to the pocket dimension by: {ev.Player.Nickname} as SCP-106!");
        }
    }
}
