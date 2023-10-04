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
            ev.Target.Teleport(RoomType.Pocket);
        }
    }
}
