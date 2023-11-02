using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp3114;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpireLabs
{
    internal class boner
    {
        internal static void OnDisguise(DisguisedEventArgs ev)
        {
            customRoles.roleData plData = null;
            try
            {
                plData = customRoles.rd.SingleOrDefault(x => x.player.NetId == (ev.Ragdoll.Owner.NetId)) ?? null;
                Log.Warn($"Found {plData.player}");
            }
            catch(Exception ex)
            {
                Log.Warn(ex);
            }
            if (plData != null)
            {
                switch (plData.UCRID)
                {
                    case 2: ev.Player.Scale = Vector3.one * 0.7f; break;
                }
            }
        }

        internal static void OnReveal(RevealedEventArgs ev)
        {
            ev.Player.Scale = Vector3.one;
        }
    }
}
