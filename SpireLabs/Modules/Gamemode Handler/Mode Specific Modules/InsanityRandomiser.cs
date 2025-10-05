using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using ObscureLabs.API.Features;
using ObscureLabs.Items;
using System.Collections.Generic;
using System;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using InventorySystem.Items.Firearms.Ammo;
using LabApi.Events.Arguments.ServerEvents;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using MEC;
using InventorySystem.Items;
using System.Linq.Expressions;

namespace ObscureLabs.Modules.Gamemode_Handler.Mode_Specific_Modules
{
    internal class InsanityRandomiser : Module
    {
        public override string Name => "InsanityRandomiser";
        public override bool IsInitializeOnStart => false;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += RoundStarted;
            //LabApi.Events.Handlers.ServerEvents.MapGenerated += MapGenerated;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= RoundStarted;
            //LabApi.Events.Handlers.ServerEvents.MapGenerated -= MapGenerated;
            return base.Disable();
        }

        //private void MapGenerated(MapGeneratedEventArgs ev)
        //{
        //    var CustomItemList = new List<CustomItemSpawner.CustomItemSpawningData>();
        //    CustomItemList.AddRange(CustomItemSpawner.WeaponList);
        //    CustomItemList.AddRange(CustomItemSpawner.ItemList);

        //    foreach (LabApi.Features.Wrappers.Pickup p in LabApi.Features.Wrappers.Pickup.List)
        //    {
        //        if (p == null) continue;
        //        try
        //        {
        //            if (p.Type.IsAmmo())
        //            {
        //                continue;
        //            }
        //            if (UnityEngine.Random.Range(0, 101) < 50)
        //            {
        //                Log.Info("Replacing item " + p.Type.ToString() + " at position: " + p.Position.ToString() + " rotation: " + p.Rotation.ToString() + ".");
        //                LabApi.Features.Wrappers.Pickup.Create(Enum.GetValues(typeof(ItemType)).ToArray<ItemType>().ToList().RandomItem(), p.Position, p.Rotation).Spawn();
        //            }
        //            else
        //            {
        //                CustomItemList.GetRandomValue().item.Spawn(p.Position);
        //            }
        //            p.Destroy();
        //        }
        //        catch (Exception e)
        //        {
        //            Log.Error($"[InsanityRandomiser] Error while spawning item: {e}");
        //        }
        //    }
        //}

        private void RoundStarted()
        {
            Timing.RunCoroutine(Randomiseitems());
        }

        public IEnumerator<float> Randomiseitems()
        {
            var CustomItemList = new List<CustomItemSpawner.CustomItemSpawningData>();
            CustomItemList.AddRange(CustomItemSpawner.WeaponList);
            CustomItemList.AddRange(CustomItemSpawner.ItemList);
            List<Pickup> pickupList = new List<Pickup>();
            pickupList.AddRange(Pickup.List);
            foreach (Room r in Room.List)
            {
                yield return Timing.WaitForOneFrame;
                try
                {
                    Log.Info("[InsanityRandomiser] Randomising items in room: " + r.Type.ToString() + " " + r.Zone.ToString() + " " + r.Pickups.Count() + " pickups.");
                    foreach (Pickup p in r.Pickups)
                    {
                        if (p == null) continue;
                        try
                        {
                            if (p.Type.IsAmmo())
                            {
                                continue;
                            }
                            if (UnityEngine.Random.Range(0, 101) < 50)
                            {
                                var i = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>().RandomItem();
                                Log.Info($"[InsanityRandomiser] Replacing item {p.Type} with {i.GetName()}");
                                Pickup.CreateAndSpawn(i, p.Position, p.Rotation);
                            }
                            else
                            {
                                Log.Info($"[InsanityRandomiser] Replacing item {p.Type} with custom item");
                                CustomItemList.GetRandomValue().item.Spawn(p.Position);
                            }
                            p.Destroy();
                        }
                        catch (Exception e)
                        {
                            Log.Error($"[InsanityRandomiser] Error while spawning item: {e}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"[InsanityRandomiser] Error while spawning item: {e}");
                }
            }
            yield return Timing.WaitForOneFrame;
        }
    }
}
