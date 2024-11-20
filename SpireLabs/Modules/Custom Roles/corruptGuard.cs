using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UncomplicatedCustomRoles.Extensions;

namespace ObscureLabs
{
    public class CorruptGuard : Module
    {
        public static bool[] CorruptGuards { get; } = new bool[short.MaxValue];

        public override string Name => "CorruptGuard";

        public override bool IsInitializeOnStart => false;

        private static bool[] cantShoot = new bool[60];

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Spawned += Spawned;
            Exiled.Events.Handlers.Player.Shot += OnShot;
            for(int i = 0; i < cantShoot.Length; i++)
            {
                cantShoot[i] = false;
            }

            return base.Enable();
        }

        public override bool Disable()
        {
            cantShoot = new bool[60];
            Exiled.Events.Handlers.Player.Spawned -= Spawned;
            Exiled.Events.Handlers.Player.Shot -= OnShot;
            return base.Disable();
        }

        private static void Spawned(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(SpawnThingCoroutine(ev));
            Timing.CallDelayed(120, () =>
            {
                for (int i = 0; i < cantShoot.Length; i++)
                {
                    cantShoot[i] = false;
                }
            });
        }

        private static void OnShot(ShotEventArgs ev)
        {
            if (cantShoot[ev.Player.Id] && ev.Target.Role == RoleTypeId.FacilityGuard)
            {
                ev.CanHurt = false;
            }
            if (cantShoot[ev.Target.Id] && ev.Player.Role == RoleTypeId.FacilityGuard)
            {
                ev.CanHurt = false;
            }
        }
        
        private static IEnumerator<float> SpawnThingCoroutine(SpawnedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            if (!ev.Player.HasCustomRole())
            {
                yield break;
            }

            var customRoleID = ev.Player.GetCustomRole().Id;

            if (customRoleID != 3)
            {
                yield break;
            }

            yield return Timing.WaitForSeconds(0.5f);
            ev.Player.ChangeAppearance(RoleTypeId.FacilityGuard, false);
            CorruptGuards[ev.Player.Id] = true;
            cantShoot[ev.Player.Id] = true;
        }

    }
}
