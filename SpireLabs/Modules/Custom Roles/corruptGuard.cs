using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCRAPI = UncomplicatedCustomRoles.API.Features.Manager;

namespace ObscureLabs
{
    internal class corruptGuard : Plugin.Module
    {
        public override string name { get; set; } = "CorruptGuard";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Player.Spawned += corruptGuard.spawned;
                Exiled.Events.Handlers.Player.Shot += corruptGuard.shot;
                Timing.RunCoroutine(initcantShoot());
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                cantShoot = new bool[60];
                Exiled.Events.Handlers.Player.Spawned -= corruptGuard.spawned;
                Exiled.Events.Handlers.Player.Shot -= corruptGuard.shot;
                base.Disable();
                return true;
            }
            catch { return false; }
        }

        private static bool[] cantShoot = new bool[60];

        internal static IEnumerator<float> initcantShoot()
        {
            yield return Timing.WaitForOneFrame;
            for (int i = 0; i < cantShoot.Length; i++)
            {
                cantShoot[i] = false;
            }
        }

        internal static void spawned(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(spawnThing(ev));
        }

        private static IEnumerator<float> spawnThing(SpawnedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            int customRoleID;
            if (UCRAPI.HasCustomRole(ev.Player))
            {
                customRoleID = UCRAPI.Get(ev.Player).Id;
                if (customRoleID == 3)
                {
                    yield return Timing.WaitForSeconds(0.5f);
                    ev.Player.ChangeAppearance(RoleTypeId.FacilityGuard, false);
                    cantShoot[ev.Player.Id] = true;
                    Timing.RunCoroutine(thing(ev.Player.Id));
                    Plugin.corruptGuards[ev.Player.Id] = true;
                }
            }
        }

        private static IEnumerator<float> thing(int PlayerID)
        {
            yield return Timing.WaitForSeconds(60f);
            cantShoot[PlayerID] = false;
        }

        internal static void shot(ShotEventArgs ev)
        {
            try
            {
                if (cantShoot[ev.Player.Id] == true && ev.Target.Role == RoleTypeId.FacilityGuard)
                {
                    ev.CanHurt = false;
                }
                if (cantShoot[ev.Target.Id] == true && ev.Player.Role == RoleTypeId.FacilityGuard)
                {
                    ev.CanHurt = false;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
        }
    }
}
