using Exiled.API.Features;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using System.Collections.Generic;
using UnityEngine;


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
            public int Id { get; set; } = 0;
            public string Name { get; set; } = "PLACEHOLDER";
            public int Score { get; set; } = 0;
            public int Lives { get; set; } = 1;
            public List<Player> Players { get; set; } = new List<Player>();
            public List<Player> DeadPlayers { get; set; } = new List<Player>();
            public RoleTypeId RoleType { get; set; } = RoleTypeId.Tutorial;
            public List<SerializableItemData> LoadOut { get; set; } = new List<SerializableItemData>();
            public List<SerializableAmmoData> Ammo { get; set; } = new List<SerializableAmmoData>();
            public Vector3 SpawnLocation { get; set; } = new Vector3(0,0,0);
        }

        public static List<SerializableTeamData> Teams { get; set; } = new List<SerializableTeamData>();

        public override bool Enable()
        {
            Teams = new List<SerializableTeamData>();
            return base.Enable();
        }

        public override bool Disable()
        {
            Teams = new List<SerializableTeamData>();
            return base.Disable();
        }

        public static IEnumerator<float> SpawnTeams()
        {
            Log.Info("Running SpawnTeams");
            yield return Timing.WaitForSeconds(1);
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
                        foreach (SerializableAmmoData ammo in team.Ammo)
                        {
                            //p.Ammo.Add(ammo.ItemType, (ushort)ammo.Quantity);
                            //p.SetAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                            //p.AddAmmo(ammo.ItemType, (ushort)ammo.Quantity);
                            p.AddAmmo(ammo.ItemType, ammo.Quantity);
                        }
                        yield return Timing.WaitForSeconds(0.1f);
                        p.Teleport(team.SpawnLocation);
                        Log.Info("Spawned Teams");

                    }
                }
                }
            }
    }
}
