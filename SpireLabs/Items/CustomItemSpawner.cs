using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using MEC;
using ObscureLabs.API.Features;
using System.Collections.Generic;
using System.Linq;
using CustomItem = Exiled.CustomItems.API.Features.CustomItem;

namespace ObscureLabs.Items
{
    public class CustomItemSpawner : Module
    {
        public static CustomItem[] WeaponList { get; } =
        {
            CustomItem.Get((uint)1), // sniper
            CustomItem.Get((uint)3), // grenade launcher
            CustomItem.Get((uint)5), // ER16
            CustomItem.Get((uint)6), // Particle Collapser
        };

        public static CustomItem[] ItemList { get; } =
        {
            CustomItem.Get((uint)2), // ClusterHE
            CustomItem.Get((uint)7), // ClusterFlash
            CustomItem.Get((uint)0), // EssentialOils
            CustomItem.Get((uint)4), // NovaGrenade
        };

        private static readonly ItemType[] _blacklistedItems = new[]
        {
            ItemType.Ammo12gauge, ItemType.Ammo44cal, ItemType.Ammo556x45, ItemType.Ammo762x39, ItemType.Ammo9x19, ItemType.Coin
        };

        public override string Name => "itemSpawner";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Map.Generated += OnMapGenerated;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Map.Generated -= OnMapGenerated;

            return base.Disable();
        }

        public static void OnMapGenerated()
        {
            Log.Info("Running custom item spawner for vanilla round");
            Timing.RunCoroutine(GunSpawnCoroutine());
        }

        public static IEnumerator<float> GunSpawnCoroutine() // Simple code to replace pickup item in a room with a custom item pickup
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

            yield return Timing.WaitForOneFrame;

            foreach (var room in Room.List)
            {
                yield return Timing.WaitForOneFrame;

                if (Room.Get(room.Type).Pickups.Count() == 0)
                {
                    continue;
                }

                if (room.Zone is ZoneType.Surface)
                {
                    continue;
                }

                foreach (var targetItem in room.Pickups.ToList())
                {
                    var spawn = UnityEngine.Random.Range(0, 85);

                    yield return Timing.WaitForOneFrame;

                    if (_blacklistedItems.Contains(targetItem.Type))
                    {
                        continue;
                    }

                    if (targetItem is null)
                    {
                        Log.Warn("item was for some reason null!?");
                    }
                    else
                    {
                        if (targetItem.Type.IsWeapon() && spawn > 30 && spawn < 65)
                        {
                            var weaponToSpawn = WeaponList.ElementAt(UnityEngine.Random.Range(0, WeaponList.Count()));
                            Pickup pickup = null;

                            if (weaponToSpawn == CustomItem.Get((uint)1)) // Sniper 
                            {
                                if (sniper < sniper_l)
                                {
                                    pickup = weaponToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    sniper++;
                                    Log.Info($"Made new item in {room.Type}");
                                    yield return Timing.WaitForOneFrame;
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            else if (weaponToSpawn == CustomItem.Get((uint)3)) // Grenade Launcher
                            {
                                if (glLauncher < glLauncher_l)
                                {
                                    pickup = weaponToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    glLauncher++;
                                    Log.Info($"Made new item in {room.Type}");
                                    yield return Timing.WaitForOneFrame;
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            else if (weaponToSpawn == CustomItem.Get((uint)5)) // ER16
                            {
                                if (ER16 < ER16_l)
                                {
                                    pickup = weaponToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    ER16++;
                                    Log.Info($"Made new item in {room.Type}");
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            else if (weaponToSpawn == CustomItem.Get((uint)6)) // Particle Collapser
                            {
                                if (particleCollapser < particleCollapser_l)
                                {
                                    pickup = weaponToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    particleCollapser++;
                                    Log.Info($"Made new item in {room.Type}");
                                    yield return Timing.WaitForOneFrame;
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                        }

                        if (!targetItem.Type.IsWeapon() && !targetItem.Type.IsKeycard() && spawn > 30 && spawn < 65)
                        {
                            var itemToSpawn = ItemList.ElementAt(UnityEngine.Random.Range(0, ItemList.Count()));
                            Pickup pickup = null;
                            if (itemToSpawn == CustomItem.Get((uint)2)) // ClusterHE 
                            {
                                if (clusterHE < clusterHE_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;

                                    clusterHE++;
                                    Log.Info($"Made new item in {room.Type}");
                                    yield return Timing.WaitForOneFrame;
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");
                                    break;
                                }
                            }
                            else if (itemToSpawn == CustomItem.Get((uint)7)) // ClusterFlash
                            {
                                if (clusterFlash < clusterFlash_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    clusterFlash++;
                                    Log.Info($"Made new item in {room.Type}");
                                    yield return Timing.WaitForOneFrame;
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");


                                    break;
                                }
                            }
                            else if (itemToSpawn == CustomItem.Get((uint)5)) // EsssentialOils
                            {
                                if (essentialOils < essentialOils_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    essentialOils++;
                                    Log.Info($"Made new item in {room.Type}");
                                    yield return Timing.WaitForOneFrame;
                                    targetItem.Destroy();
                                    Log.Warn($"Removed original item in {room.Type}");

                                    break;
                                }
                            }
                            else if (itemToSpawn == CustomItem.Get((uint)6)) // NovaGrenade
                            {
                                if (novaGrenade < novaGrenade_l)
                                {
                                    pickup = itemToSpawn.Spawn(targetItem.Transform.position);
                                    pickup.Rotation = targetItem.Transform.rotation;
                                    novaGrenade++;
                                    Log.Info($"Made new item in {room.Type}");
                                    yield return Timing.WaitForOneFrame;
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

