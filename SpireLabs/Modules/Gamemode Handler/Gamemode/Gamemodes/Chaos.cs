using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using InventorySystem.Items.Usables;
using MEC;
using ObscureLabs.Items;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using UnityEngine;

//make sure they spawn out the facility after nuke
//modifiers show: how many people on each team and lifes
//three lives respawn in heavy
//first spawn in light randomly
//open their spawn room doors
//initial spawns have no guns
//
//initial kit: 1 coin, 1 guard card, 1 torch, 1 radio, 1 medkit
//respawn kit: 1 A7, 1 coin, 1 guard card, 1 torch, 1 radio, 1 medkit
//
//unlock all doors
//respawn waves have 5 seconds of invincibility

namespace ObscureLabs.Modules.Gamemode_Handler.Gamemode.Gamemodes
{
    internal class Chaos : API.Features.Gamemode
    {
        public override string Name => "Chaos";
        public override string HintText => "CHAOS ROUND";

        public override void Enable(bool force)
        {
            Exiled.Events.Handlers.Warhead.Detonated += NukeBoom;
            Exiled.Events.Handlers.Player.Escaping += Escaping;
            CreateTeams();
            Timing.RunCoroutine(threadception());
            base.Enable(force);
        }

        private IEnumerator<float> threadception()
        {
            yield return Timing.WaitForSeconds(0.1f);
            Thread t = new Thread(() =>
            {
                foreach (Room r in Room.List)
                {
                    Thread.Sleep(1);
                    foreach (Pickup p in r.Pickups)
                    {
                        Thread.Sleep(1);
                        if(p == null) continue;
                        if(p.Category == ItemCategory.Ammo) continue;

                        try
                        {
                            int c = UnityEngine.Random.Range(0, 101);
                            if (c >= 50 && c <= 55)
                            {
                                Vector3 pos = p.Position;
                                Quaternion ros = p.Rotation;

                                if (p.Category == ItemCategory.Firearm || p.Category == ItemCategory.MicroHID)
                                {
                                    p.Destroy();
                                    var i = CustomItemSpawner.WeaponList.GetRandomValue().item.Spawn(pos);
                                    i.Rotation = ros;
                                }
                                else
                                {
                                    p.Destroy();
                                    var i = CustomItemSpawner.ItemList.GetRandomValue().item.Spawn(pos);
                                    i.Rotation = ros;
                                }
                            }
                            else
                            {
                                Vector3 pos = p.Position;
                                Quaternion ros = p.Rotation;

                                var pickup = Pickup.Create((ItemType)UnityEngine.Random.Range(0, 54));
                                p.Destroy();
                                pickup.Spawn(pos, ros);

                            }
                        }
                        catch {}
                    }
                }
            }); t.Start();
        }
        
        public override void Start()
        {
            Exiled.Events.Handlers.Player.Died += OnPlayerDeath;
            AssignTeams();
            base.Start();
            Timing.RunCoroutine(TeamHandler.SpawnTeams(Teams, true));
            Manager.setModifier(0, $"<color=#ffcc40>Scientists</color><color=#fff>: {Teams.FirstOrDefault(x => x.Name == "Science Team").Players.Count}");
            Manager.setModifier(1, $"<color=#ff6626>D-Class</color><color=#fff>: {Teams.FirstOrDefault(x => x.Name == "D-Class").Players.Count}");
            Manager.setModifier(2, $"<color=#fff>Your Lives: 3");
            Manager.setModifier(3, "Current MVP: None");
        }

        private void OnPlayerDeath(DiedEventArgs ev)
        {
            try
            {
                Teams.FirstOrDefault(x => x.Players.Contains(ev.Player)).Lives[ev.Player]--;
            }
            catch
            {
            }
        }
        
        private void NukeBoom()
        {
            Teams.FirstOrDefault(x => x.Name == "Science Team").SpawnLocation = new Vector3(127.755f, 995.470f, -45.2f);
            Teams.FirstOrDefault(x => x.Name == "D-Class").SpawnLocation = new Vector3(8.536f, 991.649f, -42.759f);
        }

        public override void End()
        {
            Exiled.Events.Handlers.Player.Escaping -= Escaping;
            Exiled.Events.Handlers.Warhead.Detonated -= NukeBoom;
        }

        public override void PlayerJoinInProgress(JoinedEventArgs ev)
        {
            var selectedTeam = Teams.FirstOrDefault(x => x.Players.Count == Teams.Min(x => x.Players.Count));
            Teams.FirstOrDefault(x => x == selectedTeam).Players.Add(ev.Player);
            Teams.FirstOrDefault(x => x == selectedTeam).Lives.Add(ev.Player, 2);
            base.PlayerJoin(ev.Player);
        }

        public override void RespawnWave(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
            foreach (Player p in Players)
            {
                if(p.IsAlive) continue;
                if (Teams.FirstOrDefault(x => x.Players.Contains(p)).Lives[p] != 0)
                {
                    SpawnPlayer(p, Teams.FirstOrDefault(x => x.Players.Contains(p)));
                }
            }
        }

        private void Escaping(EscapingEventArgs ev)
        {
            ev.IsAllowed = false;
            Manager.SendHint(ev.Player, "There is no escape", 2f);
        }

        private void CreateTeams()
        {
            TeamHandler.SerializableTeamData dClass = new TeamHandler.SerializableTeamData()
            {
                Id = 0,
                Name = "D-Class",
                RoleType = RoleTypeId.ClassD,
                Score = 0,
                Players = new List<Player>(),
                LoadOut = new List<TeamHandler.SerializableItemData>
                {
                    new(false, (int)ItemType.Coin), // Coin
                    new(false, (int)ItemType.KeycardGuard), // GuardCard
                    new(false, (int)ItemType.Flashlight), // Flashlight
                    new(false, (int)ItemType.Radio), // Radio
                    new(false, (int)ItemType.Medkit), // Medkit
                },
                Ammo = new List<TeamHandler.SerializableAmmoData>(),
                SpawnLocation = new Vector3(8.536f, 992.649f, -42.759f)
                
            };

            TeamHandler.SerializableTeamData scienceTeam = new TeamHandler.SerializableTeamData()
            {
                Id = 1,
                Name = "Science Team",
                RoleType = RoleTypeId.Scientist,
                Score = 0,
                Players = new List<Player>(),
                LoadOut = new List<TeamHandler.SerializableItemData>
                {
                    new(false, (int)ItemType.Coin), // Coin
                    new(false, (int)ItemType.KeycardGuard), // GuardCard
                    new(false, (int)ItemType.Flashlight), // Flashlight
                    new(false, (int)ItemType.Radio), // Radio
                    new(false, (int)ItemType.Medkit), // Medkit
                },
                Ammo = new List<TeamHandler.SerializableAmmoData>(),
                SpawnLocation = new Vector3(127.755f, 996.470f, -45.2f)
            };

            Teams.Add(dClass);
            Teams.Add(scienceTeam);
        }

        private void AssignTeams()
        {
            List<Player> realPlayerlist = new List<Player>();
            foreach (Player p in Player.List)
            {
                if (p.AuthenticationType == AuthenticationType.DedicatedServer) continue;
                realPlayerlist.Add(p);
            }
            realPlayerlist.ShuffleList();
            for (int i = 0; i < realPlayerlist.Count; i++)
            {
                if (i % 2 == 0)
                {
                    Teams[0].Players.Add(realPlayerlist[i]);
                    Teams[0].Lives.Add(realPlayerlist[i], 3);
                    Log.Warn($"{realPlayerlist[i].DisplayNickname} is now part of {Teams[0].Name}");
                }
                else
                {
                    Teams[1].Players.Add(realPlayerlist[i]);
                    Teams[1].Lives.Add(realPlayerlist[i], 3);
                    Log.Warn($"{realPlayerlist[i].DisplayNickname} is now part of {Teams[1].Name}");
                }
            }
        }
    }
}
