namespace SpireLabs
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

    internal static class doctor
    {
        internal static IEnumerator<float> scpShield(Player p)
        {
            for (int l = 0; l < 20; l++)
            {
                foreach (Player pp in Player.List)
                {
                    p = Player.Get(p.Id);
                    if (pp.DisplayNickname == null)
                    { }
                    else
                    {
                        if (pp != p)
                        {
                            RaycastHit hit;
                            Vector3 dir = pp.Transform.position - new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z);
                            dir = dir.normalized;
                            Ray r = new Ray(new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                            Physics.Raycast(r, out hit, maxDistance: 10);
                            Player ppp;
                            Player.TryGet(hit.collider, out ppp);
                            if (ppp != pp)
                            {
                                dir = pp.Transform.position - new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z);
                                r = new Ray(new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                                Physics.Raycast(r, out hit, maxDistance: 10);
                                Player.TryGet(hit.collider, out ppp);
                                if (ppp != pp)
                                {
                                    dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f);
                                    r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f), dir);
                                    Physics.Raycast(r, out hit, maxDistance: 10);
                                    Player.TryGet(hit.collider, out ppp);
                                    if (ppp != pp)
                                    {
                                        dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f);
                                        r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f), dir);
                                        Physics.Raycast(r, out hit, maxDistance: 10);
                                        Player.TryGet(hit.collider, out ppp);
                                        if (ppp != pp)
                                        {
                                            dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z);
                                            r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z), dir);
                                            Physics.Raycast(r, out hit, maxDistance: 10);
                                            Player.TryGet(hit.collider, out ppp);
                                        }
                                        else
                                        {
                                            if (!ppp.IsHuman)
                                            {
                                                ppp.ShowHint("You are recieving HS points from a nearby doctor!", 3);
                                                ppp.HumeShield += 4.5f;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!ppp.IsHuman)
                                        {
                                            ppp.ShowHint("You are recieving HS points from a nearby doctor!", 3);
                                            ppp.HumeShield += 4.5f;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!ppp.IsHuman)
                                    {
                                        ppp.ShowHint("You are recieving HS points from a nearby doctor!", 3);
                                        ppp.HumeShield += 4.5f;
                                    }
                                }
                            }
                            else
                            {
                                if (!ppp.IsHuman)
                                {
                                    ppp.ShowHint("You are recieving HS points from a nearby doctor!", 3);
                                    ppp.HumeShield += 4.5f;
                                }
                            }
                        }
                    }

                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        internal static IEnumerator<float> scpBoost(Player p)
        {
            for (int l = 0; l < 20; l++)
            {
                foreach (Player pp in Player.List)
                {
                    p = Player.Get(p.Id);
                    if (pp.DisplayNickname == null)
                    { }
                    else
                    {
                        if (pp != p)
                        {
                            RaycastHit hit;
                            Vector3 dir = pp.Transform.position - new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z);
                            dir = dir.normalized;
                            Ray r = new Ray(new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                            Physics.Raycast(r, out hit, maxDistance: 10);
                            Player ppp;
                            Player.TryGet(hit.collider, out ppp);
                            if (ppp != pp)
                            {
                                dir = pp.Transform.position - new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z);
                                r = new Ray(new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                                Physics.Raycast(r, out hit, maxDistance: 10);
                                Player.TryGet(hit.collider, out ppp);
                                if (ppp != pp)
                                {
                                    dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f);
                                    r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f), dir);
                                    Physics.Raycast(r, out hit, maxDistance: 10);
                                    Player.TryGet(hit.collider, out ppp);
                                    if (ppp != pp)
                                    {
                                        dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f);
                                        r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f), dir);
                                        Physics.Raycast(r, out hit, maxDistance: 10);
                                        Player.TryGet(hit.collider, out ppp);
                                        if (ppp != pp)
                                        {
                                            dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z);
                                            r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z), dir);
                                            Physics.Raycast(r, out hit, maxDistance: 10);
                                            Player.TryGet(hit.collider, out ppp);
                                        }
                                        else
                                        {
                                            if (!ppp.IsHuman)
                                            {
                                                ppp.ShowHint("You are recieving a speed boost from a nearby doctor!", 3);
                                                ppp.EnableEffect(EffectType.MovementBoost, 1.5f);
                                                ppp.ChangeEffectIntensity(EffectType.MovementBoost, 30, 1.5f);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!ppp.IsHuman)
                                        {
                                            ppp.EnableEffect(EffectType.MovementBoost, 1.5f);
                                            ppp.ChangeEffectIntensity(EffectType.MovementBoost, 30, 1.5f);
                                            ppp.ShowHint("You are recieving a speed boost from a nearby doctor!", 3);
                                        }
                                    }
                                }
                                else
                                {
                                    if (!ppp.IsHuman)
                                    {
                                        ppp.EnableEffect(EffectType.MovementBoost, 1.5f);
                                        ppp.ChangeEffectIntensity(EffectType.MovementBoost, 30, 1.5f);
                                        ppp.ShowHint("You are recieving a speed boost from a nearby doctor!", 3);
                                    }
                                }
                            }
                            else
                            {
                                if (!ppp.IsHuman)
                                {
                                    ppp.EnableEffect(EffectType.MovementBoost, 1.5f);
                                    ppp.ChangeEffectIntensity(EffectType.MovementBoost, 30, 1.5f);
                                    ppp.ShowHint("You are recieving a speed boost from a nearby doctor!", 3);
                                }
                            }
                        }
                    }

                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        internal static void doctorBoost(ActivatingSenseEventArgs ev)
        {
            if (ev.Target == null) return;
            ev.Player.EnableEffect(EffectType.MovementBoost, 10);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 10, 10);
            ev.Player.ShowHint("You provide speed to all SCP entities nearby!", 7);
            Timing.RunCoroutine(scpBoost(ev.Player));
        }

        internal static void call(SendingCallEventArgs ev)
        {
            if (ev.IsAllowed)
            {
                ev.Player.ShowHint("You are giving HS to all SCP entities nearby \n(this can overflow past natural max)", 7);
                Timing.RunCoroutine(scpShield(ev.Player));
            }
            else
                return;

        }
    }
}