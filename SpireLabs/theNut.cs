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
using Exiled.Events.EventArgs.Scp173;

namespace SpireLabs
{
    internal class theNut
    {
        internal static void scp173DMG(HurtingEventArgs ev)
        {
            if(ev.Player.Role == RoleTypeId.Scp173)
            {
                var rnd = new System.Random();
                int num = rnd.Next(1, 100);
                if(num < 20 && num > 13)
                {
                    ev.Amount = 0;
                    ev.Player.ShowHint("You just ignored some damage!");
                }
            }
        }

        //internal static void scp173TP(BlinkingEventArgs ev)
        //{
        //    if (ev.Scp173.BreakneckActive && ev.Player.Health < 1000)
        //    {
        //        Player p;
        //        foreach (Player pp in Player.List)
        //        {
        //            p = Player.Get(ev.Player.Id);
        //            if (pp.DisplayNickname == null)
        //            { }
        //            else
        //            {
        //                if (pp != p)
        //                {
        //                    RaycastHit hit;
        //                    Vector3 dir = pp.Transform.position - new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z);
        //                    dir = dir.normalized;
        //                    Ray r = new Ray(new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
        //                    Physics.Raycast(r, out hit, maxDistance: 1.5f);
        //                    Player ppp;
        //                    try
        //                    {
        //                        Player.TryGet(hit.collider, out ppp);
        //                    }
        //                    catch{ ppp = null; }
        //                    if (ppp != pp)
        //                    {
        //                        dir = pp.Transform.position - new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z);
        //                        r = new Ray(new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
        //                        Physics.Raycast(r, out hit, maxDistance: 1.5f);
        //                        try
        //                        {
        //                            Player.TryGet(hit.collider, out ppp);
        //                        }
        //                        catch { ppp = null; }
        //                        if (ppp != pp)
        //                        {
        //                            dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f);
        //                            r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f), dir);
        //                            Physics.Raycast(r, out hit, maxDistance: 1.5f);
        //                            try
        //                            {
        //                                Player.TryGet(hit.collider, out ppp);
        //                            }
        //                            catch { ppp = null; }
        //                            if (ppp != pp)
        //                            {
        //                                dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f);
        //                                r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f), dir);
        //                                Physics.Raycast(r, out hit, maxDistance: 1.5f);
        //                                try
        //                                {
        //                                    Player.TryGet(hit.collider, out ppp);
        //                                }
        //                                catch { ppp = null; }
        //                                if (ppp != pp)
        //                                {
        //                                    dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z);
        //                                    r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z), dir);
        //                                    Physics.Raycast(r, out hit, maxDistance: 1.5f);
        //                                    try
        //                                    {
        //                                        Player.TryGet(hit.collider, out ppp);
        //                                    }
        //                                    catch { ppp = null; }
        //                                }
        //                                else
        //                                {
        //                                    if (ppp.IsHuman)
        //                                    {
        //                                        ppp.Hurt(200, DamageType.Scp173);
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (ppp.IsHuman)
        //                                {
        //                                    ppp.Hurt(200, DamageType.Scp173);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (ppp.IsHuman)
        //                            {
        //                                ppp.Hurt(200, DamageType.Scp173);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (ppp.IsHuman)
        //                        {
        //                            ppp.Hurt(200, DamageType.Scp173);
        //                        }
        //                    }
        //                }
        //            }

        //        }
        //    }

        //}

            internal static void scp173ZOOM(UsingBreakneckSpeedsEventArgs ev)
        {
            //ev.Scp173.
        }
    }
}
