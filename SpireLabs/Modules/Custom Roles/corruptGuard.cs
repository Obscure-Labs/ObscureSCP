﻿using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using System.Collections.Generic;
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
            Exiled.Events.Handlers.Player.Shot += Shot;
            Player.Hurting += Hurt;

            for (int i = 0; i < cantShoot.Length; i++)
            {
                cantShoot[i] = false;
            }

            return base.Enable();
        }

        public override bool Disable()
        {
            cantShoot = new bool[60];
            Exiled.Events.Handlers.Player.Spawned -= Spawned;
            Exiled.Events.Handlers.Player.Shot -= Shot;

            return base.Disable();
        }

        private static void Spawned(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(SpawnThingCoroutine(ev));
        }

        private static void Shot(ShotEventArgs ev)
        {
            if (CustomRoles.RolesData.FirstOrDefault(x => x.Player == ev.Target).UcrId == 3)
            {
                if (cantShoot[ev.Target.Id] == true)
                {
                    ev.CanHurt = false;
                }
            }
            //ev.CanHurt = element is true && ev.Player.Role == RoleTypeId.FacilityGuard;
        }

        private static void Hurt(HurtingEventArgs ev)
        {
            if(CustomRoles.RolesData.FirstOrDefault(x => x.Player == ev.Attacker).UcrId == 3)
            {
                if (cantShoot[ev.Player.Id] == true)
                {
                    ev.IsAllowed = false;
                }
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
            cantShoot[ev.Player.Id] = true;
            Timing.RunCoroutine(SetCantShootCoroutine(ev.Player.Id));
            CorruptGuards[ev.Player.Id] = true;
        }

        private static IEnumerator<float> SetCantShootCoroutine(int PlayerID)
        {
            yield return Timing.WaitForSeconds(60f);
            cantShoot[PlayerID] = false;
        }
    }
}
