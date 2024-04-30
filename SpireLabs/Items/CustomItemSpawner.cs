using CommandSystem.Commands.Console;
using CustomItems.API;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using InventorySystem.Items.MicroHID;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.TypeResolvers;
using CustomItem = Exiled.CustomItems.API.Features.CustomItem;

namespace ObscureLabs.Items
{
    internal class CustomItemSpawner
    {

        

        public static void roundStartCustomItemHandler()
        {


            Log.Info("Running custom item spawner for vanilla round");
            Timing.RunCoroutine(gunSpawn());


        }

        public static CustomItem[] weaponlist =
        {
            CustomItem.Get((uint)1), // sniper
            CustomItem.Get((uint)3), // grenade launcher
            CustomItem.Get((uint)5), // ER16
            CustomItem.Get((uint)6), // Particle Collapser
        };

        public static CustomItem[] itemlist =
        {
            CustomItem.Get((uint)2), // ClusterHE
            CustomItem.Get((uint)7), // ClusterFlash
            CustomItem.Get((uint)0), // EssentialOils
            CustomItem.Get((uint)4), // NovaGrenade
        };

        public static IEnumerator<float> gunSpawn() // Simple code to replace pickup item in a room with a custom item pickup
        {
            var ER16 = 0;
            var glLauncher = 0;
            var particleCollapser = 0;
            var sniper = 0;
            var essentialOils = 0;
            var clusterFlash = 0;
            var clusterHE = 0;
            var novaGrenade = 0;

            // limits

            var ER16_l = 3;
            var glLauncher_l = 1;
            var particleCollapser_l = 1;
            var sniper_l = 1;
            var essentialOils_l = 10;
            var clusterFlash_l = 5;
            var clusterHE_l = 3;
            var novaGrenade_l = 10;

            ItemType[] blacklistedItems =
            [
                ItemType.Ammo12gauge, ItemType.Ammo44cal, ItemType.Ammo556x45, ItemType.Ammo762x39, ItemType.Ammo9x19,
                ItemType.Coin
            ];

            yield return Timing.WaitForOneFrame;
            foreach (Room room in Room.List)
            {
                yield return Timing.WaitForOneFrame;
                if (Room.Get(room.Type).Pickups.Count() == 0) { continue; }
                if (room.Zone == ZoneType.Surface) { continue; }

                foreach (Pickup targetItem in room.Pickups.ToList())
                {
                    var rnd = new Random();
                    var spawn = rnd.Next(0, 85);
                    yield return Timing.WaitForOneFrame;

                    if(blacklistedItems.Contains(targetItem.Type)) continue;

                    if (targetItem == null) { Log.Warn("item was for some reason null!?"); }
                    else
                    {
                        if (targetItem.Type.IsWeapon() && spawn > 30 && spawn < 65)
                        {

                            var weapontospawn = weaponlist.ElementAt(rnd.Next(0, weaponlist.Count()));
                            Pickup pickup = null;
                            if (weapontospawn == CustomItem.Get((uint)1)) // Sniper 
                            {
                                if (sniper < sniper_l)
                                {
                                    pickup = weapontospawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    sniper++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            if (weapontospawn == CustomItem.Get((uint)3)) // Grenade Launcher
                            {
                                if (glLauncher < glLauncher_l)
                                {
                                    pickup = weapontospawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    glLauncher++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            if (weapontospawn == CustomItem.Get((uint)5)) // ER16
                            {
                                if (ER16 < ER16_l)
                                {
                                    pickup = weapontospawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    ER16++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            if (weapontospawn == CustomItem.Get((uint)6)) // Particle Collapser
                            {
                                if (particleCollapser < particleCollapser_l)
                                {
                                    pickup = weapontospawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    particleCollapser++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }


                            weapontospawn.Spawn(targetItem.Transform.position);
                        }
                        if (!targetItem.Type.IsWeapon() && !targetItem.Type.IsKeycard() && spawn > 30 && spawn < 65)
                        {
                            var itemToSpawn = itemlist.ElementAt(rnd.Next(0, itemlist.Count()));
                            Pickup pickup = null;
                            if (itemToSpawn == CustomItem.Get((uint)2)) // ClusterHE 
                            {
                                if (clusterHE < clusterHE_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    
                                    clusterHE++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            if (itemToSpawn == CustomItem.Get((uint)7)) // ClusterFlash
                            {
                                if (clusterFlash < clusterFlash_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    clusterFlash++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            if (itemToSpawn == CustomItem.Get((uint)5)) // EsssentialOils
                            {
                                if (essentialOils < essentialOils_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    essentialOils++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            if (itemToSpawn == CustomItem.Get((uint)6)) // NovaGrenade
                            {
                                if (novaGrenade < novaGrenade_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    novaGrenade++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                        }


                    }
                }

               

            }

        }
    }


}

