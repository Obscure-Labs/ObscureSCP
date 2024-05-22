using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp3114;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using System;
using System.Linq;
using UnityEngine;

namespace ObscureLabs
{
    public class Scp3114 : Module
    {
        public override string Name => "SCP3114";

        public override bool IsInitializeOnStart => false;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Scp3114.Disguised += OnDisguised;
            Exiled.Events.Handlers.Scp3114.Revealed += OnRevealed;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Scp3114.Disguised -= OnDisguised;
            Exiled.Events.Handlers.Scp3114.Revealed -= OnRevealed;

            return base.Disable();
        }

        private void OnDisguised(DisguisedEventArgs ev)
        {
            RoleData plData = null;
            try
            {
                plData = CustomRoles.RolesData.SingleOrDefault(x => x.Player.NetId == (ev.Ragdoll.Owner.NetId)) ?? null;
                Log.Warn($"Found {plData.Player}");
            }
            catch (Exception ex)
            {
                Log.Warn(ex);
            }

            if (plData is not null)
            {
                switch (plData.UcrId)
                {
                    case 2: ev.Player.Scale = Vector3.one * 0.7f; break;
                }
            }
        }

        private void OnRevealed(RevealedEventArgs ev)
        {
            ev.Player.Scale = Vector3.one;
        }
    }
}
