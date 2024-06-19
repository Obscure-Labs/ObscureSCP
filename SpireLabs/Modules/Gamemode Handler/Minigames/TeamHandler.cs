using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Enums;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
using ObscureLabs.SpawnSystem;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using Exiled.Loader;
using Unity.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Collections.Generic;
using PlayerRoles;
using UnityEngine;
using static ObscureLabs.Modules.Gamemode_Handler.Minigames.TeamHandler;
using PluginAPI.Events;


namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    public class TeamHandler : Module
    {
        public override string Name => "TeamHandler";

        public override bool IsInitializeOnStart => false;

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
            public int Id { get; set; }
            public string Name { get; set; }
            public int Score { get; set; } = 0;
            public int Lives { get; set; }
            public List<Player> Players { get; set; } = new List<Player>();
            public List<Player> DeadPlayers { get; set; } = new List<Player>();
            public RoleTypeId RoleType { get; set; }
            public List<SerializableItemData> LoadOut { get; set; } = new List<SerializableItemData>();
            public List<SerializableAmmoData> Ammo { get; set; } = new List<SerializableAmmoData>();
            public Vector3 SpawnLocation { get; set; }
        }

        public static List<SerializableTeamData> Teams { get; set; } = new List<SerializableTeamData>();

        public override bool Enable()
        {
            //Teams.Add(new SerializableTeamData
            //{
            //    id = 0,
            //    name = "kevins balls",
            //    RoleType = RoleTypeId.Scp049,
            //    loadOut = new List<SerializableItemData> { new SerializableItemData(true, 3), new SerializableItemData(false, 69) },
            //    spawnLocation = new Vector3 { x = 0, y = 0, z = 0 }
            //});

            ////How to get things from an enumerable list
            //Teams.Any(x => x.id == 0);
            //Teams.Any(x => x.name == "kevins balls").players.Add(ev.Player);
            return base.Enable();
        }

        public override bool Disable()
        {
            return base.Disable();
        }

        public static IEnumerator<float> SpawnTeams()
        {
            Log.Info("Running SpawnTeams");
            Timing.WaitForSeconds(1);
            foreach(SerializableTeamData team in Teams)
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
                        yield return Timing.WaitForSeconds(0.2f);
                        foreach (SerializableAmmoData ammo in team.Ammo)
                        {
                            //p.Ammo.Add(ammo.ItemType, (ushort)ammo.Quantity);
                            //p.SetAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                            //p.AddAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                            p.AddAmmo(ammo.ItemType, ammo.Quantity);
                        }
                        yield return Timing.WaitForSeconds(0.2f);
                        p.Teleport(team.SpawnLocation);
                        Log.Info("Spawned Teams");

                    }
                }
                }
            }
    }
}
