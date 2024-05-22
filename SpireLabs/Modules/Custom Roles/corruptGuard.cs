using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using UCRAPI = UncomplicatedCustomRoles.API.Features.Manager;

namespace ObscureLabs
{
    public class CorruptGuard : Module
    {
        public static bool[] CorruptGuards { get; } = new bool[short.MaxValue];

        public override string Name => "CorruptGuard";

        public override bool IsInitializeOnStart => true;

        private static bool[] cantShoot = new bool[60];

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Spawned += Spawned;
            Exiled.Events.Handlers.Player.Shot += Shot;

            Timing.CallDelayed(Timing.WaitForOneFrame, () =>
            {
                for (int i = 0; i < cantShoot.Length; i++)
                {
                    cantShoot[i] = false;
                }
            });

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
            if (!cantShoot.TryGet(ev.Player.Id, out var element))
            {
                return;
            }

            ev.CanHurt = element is true && ev.Player.Role == RoleTypeId.FacilityGuard;
        }

        private static IEnumerator<float> SpawnThingCoroutine(SpawnedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            if (!UCRAPI.HasCustomRole(ev.Player))
            {
                yield break;
            }

            var customRoleID = UCRAPI.Get(ev.Player).Id;

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
