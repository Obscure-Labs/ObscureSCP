using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp3114;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ObscureLabs
{
    internal class scp3114 : Plugin.Module
    {
        public override string name { get; set; } = "SCP3114";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Scp3114.Disguised += OnDisguise;
                Exiled.Events.Handlers.Scp3114.Revealed += OnReveal;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Scp3114.Disguised -= OnDisguise;
                Exiled.Events.Handlers.Scp3114.Revealed -= OnReveal;
                base.Disable();
                return true;
            }
            catch { return false; }
        }



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
