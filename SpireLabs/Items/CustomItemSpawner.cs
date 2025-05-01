using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using LabApi.Events.Arguments.ServerEvents;
using MEC;
using ObscureLabs.API.Features;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomItem = Exiled.CustomItems.API.Features.CustomItem;

namespace ObscureLabs.Items
{
    public class CustomItemSpawner : Module
    {
        public class CustomItemSpawningData
        {
            public CustomItemSpawningData(CustomItem item, int count, int limit)
            {
                this.item = item;
                this.count = count;
                this.limit = limit;
            }

            public CustomItem item { get; set; }
            public int count { get; set; }
            public int limit { get; set; }
        }

        public static CustomItemSpawningData[] WeaponList { get; set; } =
        {
            new(CustomItem.Get((uint)1), 0, 2), // Sniper
            new(CustomItem.Get((uint)5), 0, 3), // Lasergun
            new(CustomItem.Get((uint)6), 0, 2),  // Particle Collapser
            new(CustomItem.Get((uint)13), 0, 3) // Super Capybara
        };

        public static CustomItemSpawningData[] ItemList { get; set; } =
        {
            new(CustomItem.Get((uint)2), 0, 3), // ClusterHE
            new(CustomItem.Get((uint)4), 0, 3), // NovaGrenade
            new(CustomItem.Get((uint)12),0, 5), // S-NAV
            new(CustomItem.Get((uint)0), 0, 6)  // Essential Oils
        };

        private static readonly ItemType[] _blacklistedItems = new[]
        {
            ItemType.Ammo12gauge, ItemType.Ammo44cal, ItemType.Ammo556x45, ItemType.Ammo762x39, ItemType.Ammo9x19, ItemType.Coin
        };

        public override string Name => "itemSpawner";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            LabApi.Events.Handlers.ServerEvents.MapGenerated += OnMapGenerated;

            return base.Enable();
        }

        public override bool Disable()
        {
            LabApi.Events.Handlers.ServerEvents.MapGenerated -= OnMapGenerated;

            return base.Disable();
        }

        public static void OnMapGenerated(MapGeneratedEventArgs ev)
        {
            Log.Debug("Running custom item spawner for vanilla round");
            Timing.RunCoroutine(GunSpawnCoroutine());
        }

        public static IEnumerator<float> GunSpawnCoroutine()
        {
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
                    var spawn = UnityEngine.Random.Range(15, 85);
                    yield return Timing.WaitForOneFrame;

                    if (_blacklistedItems.Contains(targetItem.Type))
                    {
                        continue;
                    }

                    if (targetItem.Type.IsWeapon() && spawn > 30 && spawn < 65)
                    {

                        var weaponToSpawn = WeaponList.ElementAt(UnityEngine.Random.Range(0, WeaponList.Count()));
                        Pickup pickup = null;

                        if (weaponToSpawn.count < weaponToSpawn.limit)
                        {
                            pickup = weaponToSpawn.item.Spawn(targetItem.Transform.position);
                            pickup.Rotation = targetItem.Transform.rotation;
                            weaponToSpawn.count++;
                            Log.Debug($"Made new {weaponToSpawn.item.Id} in {room.Type}");
                            yield return Timing.WaitForOneFrame;
                            targetItem.Destroy();
                            Log.Debug($"Removed original item in {room.Type}");
                            break;
                        }
                        continue;
                    }

                    if (!targetItem.Type.IsWeapon() && !targetItem.Type.IsKeycard() && spawn > 30 && spawn < 65)
                    {
                        var itemToSpawn = ItemList.ElementAt(UnityEngine.Random.Range(0, ItemList.Count()));
                        Pickup pickup = null;

                        if (itemToSpawn.count < itemToSpawn.limit)
                        {
                            pickup = itemToSpawn.item.Spawn(targetItem.Transform.position);
                            pickup.Rotation = targetItem.Transform.rotation;
                            itemToSpawn.count++;
                            Log.Debug($"Made new {itemToSpawn.item.Id} in {room.Type}");
                            yield return Timing.WaitForOneFrame;
                            targetItem.Destroy();
                            Log.Debug($"Removed original item in {room.Type}");
                        }
                        continue;
                    }
                }
            }

            foreach (CustomItemSpawningData i in WeaponList)
            {
                Log.Info($"Spawned {i.count} of {CustomItem.Get(i.item.Id).Name}");
            }
            foreach (CustomItemSpawningData i in ItemList)
            {
                Log.Info($"Spawned {i.count} of {CustomItem.Get(i.item.Id).Name}");
            }
        }
    }
}
