using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using Log = Exiled.API.Features.Log;
using UnityEngine;
using Exiled.API.Features.Pickups;
using Exiled.API.Extensions;
using SpireSCP.GUI.API.Features;
using CustomItem = Exiled.CustomItems.API.Features.CustomItem;
using System.Security.Policy;
namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    public class Chaos : Module
    {
        public override string Name => "ChaosRound";

        public override bool IsInitializeOnStart => false;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;
            Timing.RunCoroutine(GunSpawnCoroutine());
            Server.FriendlyFire = true;
            return base.Enable();

        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;

            Server.FriendlyFire = false;
            return base.Disable();
        }

        private static List<CustomItem> _customitemlist = new()
        {
            CustomItem.Get((uint)2), // ClusterHE
            CustomItem.Get((uint)7), // ClusterFlash
            CustomItem.Get((uint)0), // EssentialOils
            CustomItem.Get((uint)4), // NovaGrenade
            CustomItem.Get((uint)1), // sniper
            CustomItem.Get((uint)3), // grenade launcher
            CustomItem.Get((uint)5), // ER16
            CustomItem.Get((uint)6), // Particle Collapser
        };



        public static IEnumerator<float> GunSpawnCoroutine()
        {
            Log.Info("Running custom item spawner for vanilla round");
            foreach (Room room in Room.List.ToList())
            {
                yield return Timing.WaitForOneFrame;

                foreach (Pickup pickup in room.Pickups.ToList())
                {
                    if (pickup == null)
                    {
                        continue;
                    }
                    var customItemChance = UnityEngine.Random.Range(0, 101);
                    var isCustomItem = customItemChance >= 50 && customItemChance <= 55;

                    yield return Timing.WaitForOneFrame;

                    if (isCustomItem)
                    {
                        try
                        {
                            _customitemlist.RandomItem().Spawn(pickup.Position);

                            Log.Info($"CHAOS: Made new CUSTOM item in {room.Type}");

                            pickup.Destroy();
                            Log.Warn($"CHAOS: Removed original item in {room.Type}");
                        }
                        catch (Exception e)
                        {
                            Log.Warn(e);

                        }
                    }
                    else
                    {
                        try
                        {
                            item:
                            var randomItemType = Enum.GetValues(typeof(ItemType)).ToArray<ItemType>().GetRandomValue();
                            if (randomItemType == ItemType.SCP244a || randomItemType == ItemType.SCP244b)
                            {
                                goto item;
                            }
                            Pickup.CreateAndSpawn(randomItemType, pickup.Position, pickup.Rotation);
                            Log.Info($"CHAOS: Made new item in {room.Type}");

                            pickup.Destroy();
                            Log.Warn($"CHAOS: Removed original item in {room.Type}");
                        }
                        catch (Exception e) 
                        {
                            Log.Warn(e);
                        }

                    }
                }
            }
        }


        public static void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
                Log.Info(Plugin.EventRoundType);
                ev.IsAllowed = false;
                Log.Info("Run ChaosMode Spawn wave");

                if (ev.NextKnownTeam is Faction.FoundationEnemy)
                {
                    foreach (Player player in ev.Players)
                    {
                        player.Role.Set(RoleTypeId.ChaosRepressor, RoleSpawnFlags.UseSpawnpoint);
                        player.ClearInventory();
                        player.AddItem(ItemType.Coin, 1);
                    player.AddItem(ItemType.KeycardMTFCaptain, 1);
                    player.EnableEffect(EffectType.DamageReduction, intensity: 255, duration: 30, false);
                    player.AddItem(ItemType.Flashlight, 1);
                    player.AddItem(ItemType.Radio, 1);
                    RandomSpawn(player, ZoneType.HeavyContainment);
                    Respawn.SetTokens(SpawnableFaction.NtfWave, 100);
#warning Need to figure out how to set Respawn wave time
                    //Respawn.TimeUntilNextPhase = 45;
                    }
                }
                else
                {
                    foreach (Player player in ev.Players)
                    {
                        player.Role.Set(RoleTypeId.NtfCaptain, RoleSpawnFlags.UseSpawnpoint);
                        player.ClearInventory();
                        player.AddItem(ItemType.Coin, 1);
                    player.AddItem(ItemType.KeycardMTFCaptain, 1);
                    player.EnableEffect(EffectType.DamageReduction, intensity: 255, duration: 30, false);
                    player.AddItem(ItemType.Flashlight, 1);
                    player.AddItem(ItemType.Radio, 1);
                    RandomSpawn(player, ZoneType.HeavyContainment);
                    Respawn.SetTokens(SpawnableFaction.ChaosWave, 100);
#warning Need to figure out how to set Respawn wave time
                        //Respawn.TimeUntilNextPhase = 45;
                    }
                }
            }

        public static void RandomSpawn(Player player, ZoneType zone)
        {
            var number = UnityEngine.Random.Range(0, 2);
            var goodRoom = false;
            //what the magic number
            var room = Room.List.ElementAt(4);
            var door = Room.List.ElementAt(4).Doors.FirstOrDefault();

            while (!goodRoom)
            {
                var roomNd = new System.Random();
                var roomNum = roomNd.Next(0, Room.List.Count());

                var room2 = Room.List.ElementAt(roomNum);

                if (Map.IsLczDecontaminated)
                {
                    if (room2.Type is not RoomType.HczTesla or RoomType.HczElevatorA or RoomType.HczElevatorB && Room.List.ElementAt(roomNum).Zone == zone)
                    {
                        goodRoom = true;
                        door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                    }
                }
                else
                {
                    if (room2.Type != RoomType.HczTesla && Room.List.ElementAt(roomNum).Zone == zone)
                    {
                        goodRoom = true;

                        door = Room.List.ElementAt(roomNum).Doors.FirstOrDefault();
                    }
                }

            }

            player.Teleport(new UnityEngine.Vector3(door.Position.x, door.Position.y + 1.5f, door.Position.z));
        }

        public static IEnumerator<float> RunChaosCoroutine()
        {
            Log.Warn("Running Chaos Round");

            Timing.WaitForSeconds(0.1f);

            List<Player> PList = Player.List.ToList();

            for (int i = 0; i < (Math.Ceiling((double)PList.Count) / 2) + 1; i++)
            {
                var playerid = UnityEngine.Random.Range(0, PList.Count());
                var player = PList.ElementAt(playerid);

                yield return Timing.WaitForSeconds(0.5f);

                if (player.Role != RoleTypeId.Overwatch)
                {
                    player.Broadcast(5, "<color=green><b>CHAOS ROUND!");
                    player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
                    player.ClearInventory();
                    player.AddItem(ItemType.KeycardMTFOperative, 1);
                    player.AddItem(ItemType.Flashlight, 1);
                    player.AddItem(ItemType.Radio, 1);

                    RandomSpawn(player, ZoneType.LightContainment);
                }
                PList.Remove(player);
                //Player.List.Remove(p);
            }
            foreach (var player in PList)
            {
                if (player.Role != RoleTypeId.Overwatch)
                {
                    player.Broadcast(5, "<color=green><b>CHAOS ROUND!");
                    player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.UseSpawnpoint);
                    player.ClearInventory();
                    player.AddItem(ItemType.KeycardMTFOperative, 1);
                    player.AddItem(ItemType.Flashlight, 1);
                    player.AddItem(ItemType.Radio, 1);

                    RandomSpawn(player, ZoneType.LightContainment);
                }
            }
#warning Need to figure out how to set Respawn wave time
            //Respawn.TimeUntilNextPhase = 45;
        }
    }
}
