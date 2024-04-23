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
    using Mirror;
    using Exiled.API.Features.Roles;
    using static UnityEngine.GraphicsBuffer;
    using InventorySystem.Items.ThrowableProjectiles;
    using Exiled.API.Features.Items;
    using Hazards;
    using Exiled.API.Features.Hazards;
    using Utf8Json.Resolvers.Internal;
    using Exiled.API.Features.Toys;
    using InventorySystem.Items.Usables.Scp330;
    using Exiled.API.Extensions;
    using SpireSCP.GUI.API.Features;

    internal static class CoinFlip
    {
        public static string[] good = { "You gained 50HP!", "You gained a 5 second speed boost!", "You found a keycard!", "You are invisible for 5 seconds!", "You are healed!", "GRENADE FOUNTAIN!", "Ammo pile!!", "FREE CANDY!", "You can't die for the next 25s!", "You bring health to those around you!", "Nice hat..", "You have such radiant skin!", "You got an item!", "Brought a random player to you!" };
        public static string[] bad = { "You now have 50HP!", "You dropped all of your items, How clumsy...", "You have heavy feet for 5 seconds...", "Pocket Sand!", "You got lost and found yourself in a random room!", "Don't gamble kids!", "Beep!", "Portal to hell!!!", "Others percieve you as upside down!", "You caused a blackout in your zone!", "Door stuck! DOOR STUCK!", "Your coin melted :(", "You have been detained!", "You have been brought to a random player!", "The facility is having some technical difficulties" };
    
        private static IEnumerator<float> grenadeFountain(Player p)
        {
            
            int bombs = 0;
            while (bombs != 2)
            {
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                yield return Timing.WaitForSeconds(0.1f);
                bombs++;
            }
        }

        private static IEnumerator<float> ammoFountain(Player p)
        {
            var itemTotal = 0;
            var rnd = new System.Random();
            int num = rnd.Next(0, 100);
            if (num >= 95)
            {
                itemTotal = 350;
            }
            else
            {
                itemTotal = 50;
            }

            Log.Info("Running ammo fountain on " + p);
            int items = 0;
            while (items != itemTotal)
            {
                Pickup.CreateAndSpawn(ItemType.Ammo9x19, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo762x39, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo12gauge, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo556x45, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo44cal, p.Position, p.Rotation);


                yield return Timing.WaitForOneFrame;
                items++;
            }
        }
        private static IEnumerator<float> candyFountain(Player p)
        {
            var rnd = new System.Random();
            Log.Info("Running candy fountain on " + p);
            int items = 0;
            while (items != 50)
            {
                Exiled.API.Features.Pickups.Scp330Pickup Scp330 = (Exiled.API.Features.Pickups.Scp330Pickup)Pickup.Create(ItemType.SCP330);
                int num = rnd.Next(0, 6);
                if (num == 0)
                {
                    Scp330.Candies.Add(CandyKindID.Red);
                }
                else if (num == 1)
                {
                    Scp330.Candies.Add(CandyKindID.Blue);
                }
                else if (num == 2)
                {
                    Scp330.Candies.Add(CandyKindID.Yellow);
                }
                else if (num == 3)
                {
                    Scp330.Candies.Add(CandyKindID.Green);
                }
                else if (num == 4)
                {
                    Scp330.Candies.Add(CandyKindID.Purple);
                }
                else if (num == 5)
                {
                    Scp330.Candies.Add(CandyKindID.Rainbow);
                }
                else if (num == 6)
                {
                    Scp330.Candies.Add(CandyKindID.Pink);
                }
                Scp330.Spawn(new Vector3(p.Position.x, p.Position.y, p.Position.z), p.Rotation);

                yield return Timing.WaitForOneFrame;

                items++;
            }
        }

        private static IEnumerator<float> scl(Player p)
        {
            p.Scale = new Vector3(-1, -1, 1);
            yield return Timing.WaitForSeconds(30);
            p.Scale = Vector3.one;

        }

        private static void pp(Player pl)
        {


            Primitive p = Primitive.Create(pl.Transform.position + (pl.Transform.forward * 1.25f) + (pl.Transform.up * 0.75f), Vector3.zero, Vector3.one, false);

            p.Color = new Color(0, 255, 0);
            p.Scale = Vector3.one * 0.25f;
            p.MovementSmoothing = 125;
            p.Base.gameObject.tag = "Cube";
            p.Spawn();
            p.Base.gameObject.transform.SetParent(pl.CameraTransform);

        }

        private static IEnumerator<float> raycastHeal(Player p)
        {
            yield return Timing.WaitForSeconds(1.5f);
            for (int j = 0; j < 20; j++)
            {
                Manager.SendHint(p, "You are providing health to those around you!", 0.75f);
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
                    if (ppp.IsHuman)
                    {
                        //Log.Info($"{ppp.DisplayNickname} is {ppp.Role.Name} this role is {ppp.IsHuman}");
                        Manager.SendHint(ppp, "Someone's coinflip is giving you health!", 0.75f);
                        ppp.HumeShield += 4.75f;
                    }
                }
            }
                yield return Timing.WaitForSeconds(0.5f);
                #region oldrayCastlol
                //for (int l = 0; l < 10; l++)
                //{
                //    foreach (Player pp in Player.List)
                //    {
                //        if (pp.DisplayNickname == null)
                //        { }
                //        else
                //        {
                //            if (pp != p)
                //            {
                //                RaycastHit hit;
                //                Vector3 dir = pp.Transform.position - new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z);
                //                dir = dir.normalized;
                //                Ray r = new Ray(new Vector3(p.Transform.position.x - 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                //                Physics.Raycast(r, out hit, maxDistance: 5);
                //                Player ppp;
                //                Player.TryGet(hit.collider, out ppp);
                //                if (ppp != pp)
                //                {
                //                    dir = pp.Transform.position - new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z);
                //                    r = new Ray(new Vector3(p.Transform.position.x + 0.1f, p.Transform.position.y, p.Transform.position.z), dir);
                //                    Physics.Raycast(r, out hit, maxDistance: 5);
                //                    Player.TryGet(hit.collider, out ppp);
                //                    if (ppp != pp)
                //                    {
                //                        dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f);
                //                        r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z + 0.1f), dir);
                //                        Physics.Raycast(r, out hit, maxDistance: 5);
                //                        Player.TryGet(hit.collider, out ppp);
                //                        if (ppp != pp)
                //                        {
                //                            dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f);
                //                            r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y, p.Transform.position.z - 0.1f), dir);
                //                            Physics.Raycast(r, out hit, maxDistance: 5);
                //                            Player.TryGet(hit.collider, out ppp);
                //                            if (ppp != pp)
                //                            {
                //                                dir = pp.Transform.position - new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z);
                //                                r = new Ray(new Vector3(p.Transform.position.x, p.Transform.position.y + 0.1f, p.Transform.position.z), dir);
                //                                Physics.Raycast(r, out hit, maxDistance: 5);
                //                                Player.TryGet(hit.collider, out ppp);
                //                            }
                //                            else
                //                            {
                //                                if (ppp.IsHuman)
                //                                {
                //                                    ppp.Heal(7.5f, false);
                //                                }
                //                            }
                //                        }
                //                        else
                //                        {
                //                            if (ppp.IsHuman)
                //                            {
                //                                ppp.Heal(7.5f, false);
                //                            }
                //                        }
                //                    }
                //                    else
                //                    {
                //                        if (ppp.IsHuman)
                //                        {
                //                            ppp.Heal(7.5f, false);
                //                        }
                //                    }
                //                }
                //                else
                //                {
                //                    if (ppp.IsHuman)
                //                    {
                //                        ppp.Heal(7.5f, false);
                //                    }
                //                }
                //            }
                //        }

                //    }
                //    yield return Timing.WaitForSeconds(0.5f);
                //}
                //yield return Timing.WaitForSeconds(1f);
                #endregion
        }

        internal static void Player_FlippingCoin(FlippingCoinEventArgs ev)
        {
            var itemList = new List<ItemType> {
                ItemType.GunA7,
                ItemType.GunFSP9,
                ItemType.GunCom45,
                ItemType.GunE11SR,
                ItemType.GunRevolver,
                ItemType.GunLogicer,
                ItemType.GunFRMG0,
                ItemType.GunAK,
                ItemType.Adrenaline,
                ItemType.Painkillers,
                ItemType.ArmorLight,
                ItemType.ArmorHeavy,
                ItemType.ArmorCombat,
                ItemType.Medkit,
                ItemType.MicroHID,
                ItemType.ParticleDisruptor,
                ItemType.SCP500,
                ItemType.Radio,
                ItemType.Jailbird,
                ItemType.GunShotgun,
                ItemType.KeycardZoneManager,
                ItemType.KeycardScientist,
                ItemType.KeycardResearchCoordinator,
                ItemType.KeycardGuard,
                ItemType.KeycardJanitor,
                ItemType.KeycardContainmentEngineer,
                ItemType.KeycardFacilityManager,
                ItemType.GunCrossvec,
                ItemType.KeycardMTFCaptain,
                ItemType.KeycardMTFOperative,
                ItemType.KeycardMTFPrivate,
                ItemType.KeycardChaosInsurgency,
                ItemType.KeycardO5,
                ItemType.Flashlight,
                ItemType.SCP1576,
                ItemType.SCP244b,
                ItemType.SCP244a,
                ItemType.SCP207,
                ItemType.Coin
            };

            //pp(ev.Player);
            var rnd = new System.Random();
            int num = rnd.Next(0, 100);
            int result = 0;
            if (num > 20 && num < 45) result = 1;
            if (num > 45 && num < 100) result = 2;
            if (result == 1)
            {
                Log.Debug($"{ev.Player.Nickname} flipped a coin and got a good result!");
                switch (rnd.Next(0, good.Count()))
                {
                    case 0:
                        //ev.Player.ShowHint(good[0], 3);
                        Manager.SendHint(ev.Player, good[0], 3);
                        ev.Player.Heal(50, true);
                        if (ev.Player.Role == RoleTypeId.NtfCaptain)
                        {
                            ev.Player.MaxHealth = 150;
                        }
                        else
                        {
                            ev.Player.MaxHealth = 100;
                        }
                        break;
                    case 1:
                        Manager.SendHint(ev.Player, good[1], 3);
                        ev.Player.EnableEffect(EffectType.MovementBoost, 5);
                        ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 205, 5);
                        break;
                    case 2:
                        bool todrop = false;
                        Manager.SendHint(ev.Player, good[2], 3);
                        if (ev.Player.IsInventoryFull)
                        {
                            todrop = true;

                        }
                        else
                        {
                            todrop = false;
                        }

                        var rnd2 = new System.Random();
                        int card = rnd2.Next(1, 3);
                        if (card == 1)
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardZoneManager, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardZoneManager);
                            }
                        }
                        else if (card == 2)
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardMTFOperative, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardMTFOperative);
                            }
                        }
                        else
                        {
                            if (todrop)
                            {
                                Pickup.CreateAndSpawn(ItemType.KeycardResearchCoordinator, ev.Player.Position, ev.Player.Rotation);
                            }
                            else
                            {
                                ev.Player.AddItem(ItemType.KeycardResearchCoordinator);
                            }
                        }
                        break;
                    case 3:
                        Manager.SendHint(ev.Player, good[3], 3);
                        ev.Player.EnableEffect(EffectType.Invisible, 5);
                        ev.Player.ChangeEffectIntensity(EffectType.Invisible, 1, 5);
                        break;
                    case 4:
                        ev.Player.Heal(150, false);
                        Manager.SendHint(ev.Player, good[4], 3);
                        break;
                    case 5:
                        Manager.SendHint(ev.Player, good[5], 3);
                        Timing.RunCoroutine(grenadeFountain(ev.Player));
                        break;
                    case 6:
                        Manager.SendHint(ev.Player, good[6], 3);
                        Timing.RunCoroutine(ammoFountain(ev.Player));
                        break;
                    case 7:
                        Manager.SendHint(ev.Player, good[7], 3);
                        Timing.RunCoroutine(candyFountain(ev.Player));
                        break;
                    case 8:
                        Manager.SendHint(ev.Player, good[8], 3);
                        ev.Player.EnableEffect(EffectType.DamageReduction, 25);
                        ev.Player.ChangeEffectIntensity(EffectType.DamageReduction, 255, 25);
                        break;
                    case 9:
                        Manager.SendHint(ev.Player, good[9], 1.5f);
                        Timing.RunCoroutine(raycastHeal(ev.Player));
                        break;
                    case 10:
                        Manager.SendHint(ev.Player, good[10], 3);
                        bool tp = false;
                        if (ev.Player.IsInventoryFull) tp = true;
                        else tp = false;
                        if (tp)
                        {
                            Pickup.CreateAndSpawn(ItemType.SCP268, ev.Player.Position, ev.Player.Rotation);
                        }
                        else
                        {
                            ev.Player.AddItem(ItemType.SCP268);
                        }
                        break;
                    case 11:
                        Manager.SendHint(ev.Player, good[11], 3);
                        Timing.RunCoroutine(glow(ev.Player));
                        break;
                    case 12:
                        Manager.SendHint(ev.Player, good[12], 3);
                        var randomitem = new System.Random();
                        Pickup.CreateAndSpawn(itemList.ElementAt(randomitem.Next(0, itemList.Count() + 1)), ev.Player.Position, ev.Player.Rotation);
                        break;
                    case 13:

                    randomplayertptome:
                        var random = new System.Random();
                        var target = Plugin.PlayerList.ElementAt(random.Next(Plugin.PlayerList.Count));
                        Log.Info(target.Nickname);
                        if (Plugin.PlayerList.Count == 1) { ev.Player.Vaporize(ev.Player); Manager.SendHint(ev.Player, "LOL", 5); break; }
                        else
                        {
                            if (!target.IsAlive || target.Role.Type == RoleTypeId.Overwatch || target.Role.Type == RoleTypeId.Spectator || target == ev.Player) { goto randomplayertptome; }
                            else
                            {
                                target.Position = ev.Player.Position;
                                Manager.SendHint(ev.Player, good[13], 3);
                            }
                        }
                        break;

                }

            }
            else if (result == 2)
            {
                Log.Debug($"{ev.Player.Nickname} flipped a coin and got a bad result!");
                switch (rnd.Next(0, bad.Count()))
                {
                    case 0:
                        Manager.SendHint(ev.Player, bad[0], 3);
                        ev.Player.Health = 50;
                        break;
                    case 1:
                        Manager.SendHint(ev.Player, bad[1], 3);
                        ev.Player.DropItems();
                        break;
                    case 2:
                        Manager.SendHint(ev.Player, bad[2], 3);
                        ev.Player.EnableEffect(EffectType.SinkHole, 5, true);
                        ev.Player.ChangeEffectIntensity(EffectType.SinkHole, 1, 5);
                        break;
                    case 3:
                        Manager.SendHint(ev.Player, bad[3], 3);
                        ev.Player.EnableEffect(EffectType.Flashed, 5, true);
                        ev.Player.ChangeEffectIntensity(EffectType.Flashed, 1, 5);
                        break;
                    case 4:
                        if (Warhead.IsDetonated)
                            break;
                        Manager.SendHint(ev.Player, bad[4], 3);
                        var r = new System.Random();
                        var n = r.Next(0, 2);
                        bool goodRoom = false;
                        Room room = Room.List.ElementAt(4);
                        Door door = Room.List.ElementAt(4).Doors.FirstOrDefault();
                        while (goodRoom == false)
                        {
                            var roomNd = new System.Random();
                            int roomNum = roomNd.Next(0, Room.List.Count());
                            if (Map.IsLczDecontaminated)
                            {
                                if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla && Room.List.ElementAt(roomNum).Zone != ZoneType.LightContainment && Room.List.ElementAt(roomNum).Type != RoomType.HczElevatorA && Room.List.ElementAt(roomNum).Type != RoomType.HczElevatorB)
                                {
                                    goodRoom = true;
                                    door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                                }
                            }
                            else
                            {
                                if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla)
                                {
                                    goodRoom = true;
                                    door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                                }
                            }

                        }
                        ev.Player.Teleport(new Vector3(door.Position.x, door.Position.y + 1.5f, door.Position.z));

                        break;
                    case 5:

                        var randomdeath = new System.Random();
                        int randomdeathpick = randomdeath.Next(0, 4);
                        if (randomdeathpick == 0)
                        {
                            ev.Player.EnableEffect(EffectType.CardiacArrest, 999);
                            ev.Player.ChangeEffectIntensity(EffectType.CardiacArrest, 25);
                            ev.Player.EnableEffect(EffectType.SeveredHands, 999);
                            Manager.SendHint(ev.Player, "Look ma' no hands!", 3);
                        }
                        if (randomdeathpick == 1) { ev.Player.Vaporize(ev.Player); Manager.SendHint(ev.Player, "Voop", 3); }
                        if (randomdeathpick == 2) { ev.Player.Explode(); Manager.SendHint(ev.Player, "Bang!", 3); }
                        if (randomdeathpick == 3) { Timing.RunCoroutine(shitonthefloor(ev.Player)); Manager.SendHint(ev.Player, "Shidded", 3);  }
                        break;
                    case 6:
                        Timing.RunCoroutine(beep(ev.Player));
                        Manager.SendHint(ev.Player, bad[6], 3);
                        break;
                    case 7:
                        Manager.SendHint(ev.Player, bad[7], 3);
                        ZoneType zt = ev.Player.CurrentRoom.Zone;
                        ev.Player.Teleport(Room.List.FirstOrDefault(x => x.Type == RoomType.Pocket));
                        ev.Player.EnableEffect(EffectType.PocketCorroding, 999);
                        ev.Player.EnableEffect(EffectType.Corroding, 999);
                        Timing.RunCoroutine(enterPD(ev.Player, zt));
                        break;
                    case 8:
                        Manager.SendHint(ev.Player, bad[8], 3);

                        ev.Player.Scale = Vector3.one * -1;
                        Timing.RunCoroutine(scl(ev.Player));
                        break;
                    case 9:
                        Manager.SendHint(ev.Player, bad[9], 3);
                        var zone = ev.Player.CurrentRoom.Zone;
                        Map.TurnOffAllLights(30f, zone);
                        break;
                    case 10:
                        Manager.SendHint(ev.Player, bad[10], 3);
                        foreach (Room roomSel in Room.List)
                        {
                            if (roomSel.Zone == ev.Player.Zone)
                            {

                                Timing.RunCoroutine(roomRGB(roomSel));

                            }
                        }
                        break;
                    case 11:
                        Manager.SendHint(ev.Player, bad[11], 3);
                        ev.Player.RemoveHeldItem(true);
                        break;
                    case 12:
                        Manager.SendHint(ev.Player, bad[12], 3);
                        Timing.RunCoroutine(disarm(ev.Player));
                        break;
                    case 13:

                        randomplayertp:
                        var random = new System.Random();
                        var target = Plugin.PlayerList.ElementAt(random.Next(Plugin.PlayerList.Count));
                        Log.Info(target.Nickname);
                        if (Plugin.PlayerList.Count == 1) { ev.Player.Vaporize(ev.Player); Manager.SendHint(ev.Player, "LOL", 5); break; }
                        else
                        {
                            if (!target.IsAlive || target.Role.Type == RoleTypeId.Overwatch || target.Role.Type == RoleTypeId.Spectator || target == ev.Player) { goto randomplayertp; }
                            else
                            {
                                ev.Player.Position = target.Position;
                            }
                            Manager.SendHint(ev.Player, bad[13], 3);
                        }
                        break;
                    case 14:
                        Manager.SendHint(ev.Player, bad[14], 3);
                        foreach (Room _r in Room.List)
                        {
                            _r.TurnOffLights(15);
                            foreach (Door _d in _r.Doors)
                            {
                                _d.IsOpen = false;
                            }
                        }
                        break;
                }
            }
            else
            {
                Log.Debug($"{ev.Player.Nickname} flipped a coin and got nothing!");
                //ev.Player.ShowHint("No consequences, this time...", 3);
                Manager.SendHint(ev.Player, "No consequences, this time...", 5);
            }
        }
        private static IEnumerator<float> disarm(Player ev)
        {
            ev.Handcuff();
            yield return Timing.WaitForSeconds(10);
            ev.RemoveHandcuffs();
        }

        private static IEnumerator<float> shitonthefloor(Player ev)
        {
            for (int i = 0; i < 30; i++)
            {
                yield return Timing.WaitForSeconds(0.5f);
                ev.PlaceTantrum(true);
            }
            ev.Explode();
        }

        private static IEnumerator<float> roomRGB(Room roomSel)
        {

            Color[] colors = {
            Color.red, //this is red you fucking twat
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.yellow,
            };
            var rnd = new System.Random();
            Color color = colors[rnd.Next(0, colors.Count())];
            roomSel.Color = color * 9;
            roomSel.LockDown(10, DoorLockType.AdminCommand);
            yield return Timing.WaitForSeconds(30);
            roomSel.ResetColor();
        }
        private static IEnumerator<float> glow(Player p)
        {
            Exiled.API.Features.Toys.Light li = Exiled.API.Features.Toys.Light.Create(new Vector3(p.Transform.position.x, p.Transform.position.y + 1.2f, p.Transform.position.z), Vector3.zero, Vector3.one, false);
            li.Range = 45f;
            li.Intensity = 9999f;
            li.Color = Color.yellow;
            li.Spawn();
            li.Base.gameObject.transform.SetParent(p.GameObject.transform);
            yield return Timing.WaitForSeconds(30f);
            li.Destroy();
        }
        private static IEnumerator<float> beep(Player p)
        {
            p.PlayBeepSound();
            p.PlayShieldBreakSound();
            yield return Timing.WaitForSeconds(0.5f);
            p.PlayBeepSound();
            p.PlayShieldBreakSound();
            yield return Timing.WaitForSeconds(0.5f);
            p.PlayBeepSound();
            p.PlayShieldBreakSound();
            yield return Timing.WaitForSeconds(0.5f);
            p.PlayBeepSound();
            p.PlayShieldBreakSound();
            yield return Timing.WaitForSeconds(0.1f);
        }
        private static IEnumerator<float> enterPD(Player p, ZoneType zt)
        {
            Door door = Room.List.FirstOrDefault().Doors.FirstOrDefault();
            yield return Timing.WaitForOneFrame;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += ExitVoid;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += FixThing;
            void ExitVoid(EscapingPocketDimensionEventArgs ev)
            {
                bool goodRoom = false;
                while (goodRoom == false)
                {
                    var roomNd = new System.Random();
                    int roomNum = roomNd.Next(0, Room.List.Count());
                    if (Room.List.ElementAt(roomNum).Type != RoomType.HczTesla && Room.List.ElementAt(roomNum).Zone == zt)
                    {
                        goodRoom = true;
                        door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                    }
                }
                ev.TeleportPosition = new Vector3(door.Position.x, door.Position.y + 1.5f, door.Position.z);
                p.DisableEffect(EffectType.PocketCorroding);
                p.DisableEffect(EffectType.Corroding);
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }
            void FixThing(FailingEscapePocketDimensionEventArgs e)
            {
                Exiled.Events.Handlers.Player.EscapingPocketDimension -= ExitVoid;
                Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= FixThing;
            }
            yield return Timing.WaitForOneFrame;
        }
    }
}