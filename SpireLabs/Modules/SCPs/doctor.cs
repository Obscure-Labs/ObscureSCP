namespace ObscureLabs
{
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Item;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using PlayerRoles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnityEngine;
    using System.IO;
    using Exiled.CustomItems.API.Features;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Doors;
    using System;
    using PlayerRoles.FirstPersonControl;
    using HarmonyLib;
    using System.Reflection.Emit;
    using NorthwoodLib.Pools;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Scp049;
    using SpireSCP.GUI.API.Features;
    using ObscureLabs.SpawnSystem;

    internal class doctor : Plugin.Module
    {
        public override string name { get; set; } = "Doctor";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Scp049.ActivatingSense += doctorBoost;
                Exiled.Events.Handlers.Scp049.SendingCall += call;
                Exiled.Events.Handlers.Player.Spawned += Player_Spawned;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Scp049.ActivatingSense -= doctorBoost;
                Exiled.Events.Handlers.Scp049.SendingCall -= call;
                Exiled.Events.Handlers.Player.Spawned -= Player_Spawned;
                base.Disable();
                return true;
            }
            catch { return false; }
        }

        internal static void doctorBoost(ActivatingSenseEventArgs ev)
        {
            if (ev.Target == null) return;
            ev.Player.EnableEffect(EffectType.MovementBoost, 10);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 10, 10);
            Manager.SendHint(ev.Player, "You provide speed to all SCP entities nearby!", 7);
            Timing.RunCoroutine(fixedScpBoost(ev.Player));
        }

        internal static void call(SendingCallEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                Manager.SendHint(ev.Player, "You are giving HS to all SCP entities nearby \n(this can overflow past natural max)", 7);
                Timing.RunCoroutine(fixedScpShield(ev.Player));
            }
            else
                return;

        }
        internal static IEnumerator<float> fixedScpBoost(Player p)
        {
            p.EnableEffect(EffectType.MovementBoost, 15f);
            p.EnableEffect(EffectType.MovementBoost, 30, 15f);

            Log.Info("Running fixedScpBoost");
            for (int j = 0; j < 60; j++)
            {
                Manager.SendHint(p, "You provide speed to all SCP entities nearby!", 0.75f);
                foreach (Player pp in Player.List)
                {
                    if (pp == p) continue;
                    Player nP = Player.Get(p.Id);
                    int loopCntr = 0;
                    RaycastHit h = new RaycastHit();
                    Player ppp = null;
                    do
                    {
                        Vector3 dir = pp.Position - new Vector3(nP.Position.x, nP.Position.y + 0.1f, nP.Position.z);
                        Physics.Raycast(nP.Position, dir, out h);
                        loopCntr++;
                    } while (!Player.TryGet(h.collider, out ppp) && loopCntr != 5);
                    if (ppp == null) continue;
                    if (Math.Sqrt((Math.Pow((nP.Position.x - ppp.Position.x), 2)) + (Math.Pow((nP.Position.y - ppp.Position.y), 2))) > 10) continue;
                    if (!ppp.IsHuman && ppp != p)
                    {
                        Manager.SendHint(ppp, "You are recieving a speed boost from a nearby doctor!", 0.75f);
                        ppp.EnableEffect(EffectType.MovementBoost, 1.5f);
                        ppp.ChangeEffectIntensity(EffectType.MovementBoost, 30, 1.5f);
                    }
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
            Log.Info("Stopped fixedScpBoost");
        }
        internal static IEnumerator<float> fixedScpShield(Player p)
        {
            Log.Info("Running fixedScpShield");
            for (int j = 0; j < 120; j++)
            {
                Manager.SendHint(p, "You provide protection to all SCP entities nearby!", 0.75f);
                foreach (Player pp in Player.List)
                {
                    if (pp == p) continue;
                    Player nP = Player.Get(p.Id);
                    int loopCntr = 0;
                    RaycastHit h = new RaycastHit();
                    Player ppp = null;
                    do
                    {
                        Vector3 dir = pp.Position - new Vector3(nP.Position.x, nP.Position.y + 0.1f, nP.Position.z);
                        Physics.Raycast(nP.Position, dir, out h);
                        loopCntr++;
                    } while (!Player.TryGet(h.collider, out ppp) && loopCntr != 5);
                    if (ppp == null) continue;
                    if (Math.Sqrt((Math.Pow((nP.Position.x - ppp.Position.x), 2)) + (Math.Pow((nP.Position.y - ppp.Position.y), 2))) > 10) continue;
                    if (!ppp.IsHuman && ppp != p)
                    {
                        Manager.SendHint(ppp, "You are recieving HS points from a nearby doctor!", 0.75f);
                        ppp.HumeShield += 2.7f;
                    }
                }
                yield return Timing.WaitForSeconds(0.5f);
            }
            Log.Info("Stopped fixedScpShield");

        }

        public static void Player_Spawned(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(Uppies(ev));
            if (ev.Player.Role == RoleTypeId.Scp0492)
            {
                var plData = customRoles.rd.SingleOrDefault(x => x.player.NetId == ev.Player.NetId) ?? null;
                if (plData != null)
                {
                    switch (plData.UCRID)
                    {
                        case 2: ev.Player.Scale = Vector3.one * 0.7f; break;
                    }
                }
            }

            Timing.RunCoroutine(HealthOverride.OverrideHealth(ev));
            Timing.RunCoroutine(customRoles.CheckRoles(ev.Player));
        }

        private static IEnumerator<float> Uppies(SpawnedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            ev.Player.Position = new Vector3(ev.Player.Position.x, ev.Player.Position.y + 0.3f, ev.Player.Position.z);
        }
    }
}
    