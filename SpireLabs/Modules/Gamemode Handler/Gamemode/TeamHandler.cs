using System;
using Exiled.API.Features;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using MapGeneration;
using UnityEngine;


namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    public class TeamHandler
    {
        public class SerializableItemData
        {
            public SerializableItemData(bool iscustomitem, int id)
            {
                IsCustomItem = iscustomitem;
                Id = id;
            }
            public bool IsCustomItem { get; set; }
            public int Id { get; set; }
        }

        public class SerializableAmmoData
        {
            public Exiled.API.Enums.AmmoType ItemType { get; set; }
            public ushort Quantity { get; set; }
        }

        public class SerializableTeamData
        {
            public int Id { get; set; } = 0;
            public string Name { get; set; } = "PLACEHOLDER";
            public int Score { get; set; } = 0;
            public Dictionary<Player, int> Lives { get; set; } = new Dictionary<Player, int>();
            public List<Player> Players { get; set; } = new List<Player>();
            public List<Player> DeadPlayers { get; set; } = new List<Player>();
            public RoleTypeId RoleType { get; set; } = RoleTypeId.Tutorial;
            public List<SerializableItemData> LoadOut { get; set; } = new List<SerializableItemData>();
            public List<SerializableAmmoData> Ammo { get; set; } = new List<SerializableAmmoData>();
            public Vector3 SpawnLocation { get; set; } = new Vector3(0,0,0);
        }

        public static IEnumerator<float> SpawnPlayer(Player p, SerializableTeamData team, bool chaos)
        {
            Log.Warn($"Passed to team handler");
            p.RoleManager.ServerSetRole(team.RoleType, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.All);
            Log.Warn($"Role Set");
            p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 5f, false);
            Log.Warn($"Effects Given");
            p.ChangeEffectIntensity(Exiled.API.Enums.EffectType.DamageReduction, 255, 5f);
            Log.Warn($"Effects Intensity Changed");
            p.ClearInventory();
            Log.Warn("Got to inventory assignment");
            foreach (SerializableItemData i in team.LoadOut)
            {
                if (!i.IsCustomItem)
                {
                    Exiled.API.Features.Items.Item.Create((ItemType)i.Id).Give(p);
                }
                else
                {
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)i.Id).Give(p);
                }
            }
            Log.Warn("Passed inventory assignment");
            foreach (SerializableAmmoData ammo in team.Ammo)
            {
                p.AddAmmo(ammo.ItemType, ammo.Quantity);
            }
            yield return Timing.WaitForSeconds(0.1f);
            Log.Warn("Teleporting Player");
            if (chaos && !Warhead.IsDetonated)
            {
                Room room = Room.List.GetRandomValue(x =>
                    x.Zone == ZoneType.HeavyContainment && x.RoomName != RoomName.HczCheckpointA && x.RoomName != RoomName.HczCheckpointB && x.RoomName != RoomName.Hcz079 && x.RoomName != RoomName.Hcz106);
                p.Teleport(room.Doors.FirstOrDefault().Position + new Vector3(0, 1.5f, 0));
            }
            else
            {
                p.Teleport(team.SpawnLocation);
            }

            Log.Warn("Spawn Complete");
        }

        public static IEnumerator<float> SpawnTeam(SerializableTeamData team)
        {
            foreach (Player p in team.Players)
            {
                if (p.Role.Type == team.RoleType && team.Players.Contains(p))
                {
                    continue;
                }
                else
                {

                    Log.Info("Player");
                    p.RoleManager.ServerSetRole(team.RoleType, RoleChangeReason.RoundStart, RoleSpawnFlags.None);
                    p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 5f, false);
                    p.ChangeEffectIntensity(Exiled.API.Enums.EffectType.DamageReduction, 255, 5f);
                    p.ClearInventory();
                    Log.Info("Set Player Role");
                    foreach (SerializableItemData i in team.LoadOut)
                    {
                        Log.Info("Giving Item");
                        if (!i.IsCustomItem)
                        {
                            Exiled.API.Features.Items.Item.Create((ItemType)i.Id).Give(p);
                        }
                        else
                        {
                            Exiled.CustomItems.API.Features.CustomItem.Get((uint)i.Id).Give(p);
                        }
                    }
                    foreach (SerializableAmmoData ammo in team.Ammo)
                    {
                        //p.Ammo.Add(ammo.ItemType, (ushort)ammo.Quantity);
                        //p.SetAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                        //p.AddAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                        p.AddAmmo(ammo.ItemType, ammo.Quantity);
                    }
                    yield return Timing.WaitForSeconds(0.1f);
                    p.Teleport(team.SpawnLocation);
                    Log.Info("Spawned Team");

                }
            }
        }

        public static IEnumerator<float> SpawnTeams(List<SerializableTeamData> Teams, bool chaos = false)
        {
            Log.Info("Running SpawnTeams");
            yield return Timing.WaitForSeconds(1);
            foreach(SerializableTeamData team in Teams)
            {
                foreach (Player p in team.Players)
                {
                    try
                    {
                        if (p.Role.Type == team.RoleType && team.Players.Contains(p))
                        {
                            continue;
                        }
                        else
                        {

                            Log.Info("Player");
                            p.RoleManager.ServerSetRole(team.RoleType, RoleChangeReason.RoundStart,
                                RoleSpawnFlags.None);
                            p.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 5f, false);
                            p.ChangeEffectIntensity(Exiled.API.Enums.EffectType.DamageReduction, 255, 5f);
                            p.ClearInventory();
                            Log.Info("Set Player Role");
                            foreach (SerializableItemData i in team.LoadOut)
                            {
                                Log.Info("Giving Item");
                                if (!i.IsCustomItem)
                                {
                                    Exiled.API.Features.Items.Item.Create((ItemType)i.Id).Give(p);
                                }
                                else
                                {
                                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)i.Id).Give(p);
                                }
                            }

                            foreach (SerializableAmmoData ammo in team.Ammo)
                            {
                                //p.Ammo.Add(ammo.ItemType, (ushort)ammo.Quantity);
                                //p.SetAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                                //p.AddAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                                p.AddAmmo(ammo.ItemType, ammo.Quantity);
                            }
                            if (chaos && !Exiled.API.Features.Warhead.IsDetonated)
                            {
                                Room room = Room.List.GetRandomValue(x =>
                                    x.Zone == ZoneType.LightContainment && x.RoomName != RoomName.LczCheckpointA &&
                                    x.RoomName != RoomName.HczCheckpointB);
                                p.Teleport(room.Doors.FirstOrDefault().Position + new Vector3(0, 1.5f, 0));
                            }
                            else
                            {
                                p.Teleport(team.SpawnLocation);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Info($"Spawning Player Error : {ex}");
                    }

                    Log.Info("Spawned Teams");

                }
            }
        }
    }
}
