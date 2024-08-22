using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
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
            Exiled.Events.Handlers.Player.Hurting += Hurt;


            return base.Enable();
        }

        public override bool Disable()
        {
            cantShoot = new bool[60];
            Exiled.Events.Handlers.Player.Spawned -= Spawned;
            Exiled.Events.Handlers.Player.Hurting -= Hurt;
            return base.Disable();
        }

        private static void Spawned(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(SpawnThingCoroutine(ev));
        }



        private static void Hurt(HurtingEventArgs ev)
        {
            if (CorruptGuards[ev.Attacker.Id])
            {
                if (ev.Player.Role.Type == RoleTypeId.FacilityGuard && Round.ElapsedTime.TotalMinutes < 2)
                {
                    ev.IsAllowed = false;
                }
                else{ return; }
            }
            else if (!CorruptGuards[ev.Attacker.Id])
            {
                if (CorruptGuards[ev.Attacker.Id] && Round.ElapsedTime.TotalMinutes < 2)
                {
                    ev.IsAllowed = false;
                }
                else { return; }
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
        }

    }
}
