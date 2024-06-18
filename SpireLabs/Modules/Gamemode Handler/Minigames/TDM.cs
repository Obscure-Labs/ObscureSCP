using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Loader.Models;
using InventorySystem.Items.Usables.Scp330;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using static ObscureLabs.Modules.Gamemode_Handler.Minigames.TeamHandler;
using Exiled.API.Features.Roles;
using PluginAPI.Roles;
using Utils.NonAllocLINQ;
using InventorySystem.Items.Usables.Scp244.Hypothermia;
using Exiled.Events.EventArgs.Player;
using SpireSCP.GUI.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.API.Features.Pickups;
using PluginAPI.Core.Zones;

namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    public class TDM : Module
    {


        public override string Name => "TDM";

        public override bool IsInitializeOnStart => false;

        public List<CoroutineHandle> _coroutines = new List<CoroutineHandle>();


        private readonly Dictionary<int, MapData> _maps = new()
        {
            { 0, new MapData("pvpA1_2t", "\"Low Effort\"", new Vector3(8.48f, 1106.5f, 30.46f), new Vector3(-20.8f, 1107.5f, 51.66f)) },
            { 1, new MapData("pvpA2_2t", "\"Tilted Towers\"",new Vector3(9.7f, 1102f, 52.46f), new Vector3(-20.20f, 1102f, 35.37f)) },
            { 2, new MapData("pvpMZA1_2t", "\"The Maze\"", new Vector3(23.96f, 1126f, 29.14f), new Vector3(-30.74f, 1126f, -16.93f)) },
        };

        public int ScoreLimit = 20;


        private MapData _map;


        public override bool Enable()
        {
            Round.IsLocked = true;
            var range = Random.Range(0, 3);
            _map = _maps[range];


            _coroutines.Add(Timing.RunCoroutine(RoundSetup()));
            Log.Warn("Unloading all default maps");
            MapEditorReborn.API.Features.MapUtils.LoadMap("empty");
            Log.Warn($"Loading Map: {_map.DisplayName}");
            MapEditorReborn.API.Features.MapUtils.LoadMap($"{_map.Name}");


            Exiled.Events.Handlers.Server.RoundStarted += RoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam += RespawnWave;
            Exiled.Events.Handlers.Player.Died += Died;
            return base.Enable();
        }
        public override bool Disable()
        {
            foreach (CoroutineHandle h in _coroutines)
            {
                Timing.KillCoroutines(h);
            }
            Exiled.Events.Handlers.Server.RoundStarted -= RoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam -= RespawnWave;
            Exiled.Events.Handlers.Player.Died -= Died;
            return base.Disable();
        }


        private IEnumerator<float> RoundSetup()
        {
            Teams.Add(new SerializableTeamData
            {
                Id = 0,
                Name = "Conscripts",
                RoleType = RoleTypeId.ChaosConscript,
                Score = 0,
                Players = new List<Player>(),
                LoadOut = new List<SerializableItemData> {
                    new SerializableItemData(false, 37), // ArmorCombat
                    new SerializableItemData(false, 20), // GunE11SR
                    new SerializableItemData(false, 33), // Adrenaline
                    new SerializableItemData(false, 25), // GrenadeHE
                    new SerializableItemData(false, 26), // GrenadeFlash
                    new SerializableItemData(false, 34), // Painkillers
                    new SerializableItemData(false, 34), // Painkillers
                },
                Ammo = new List<SerializableAmmoData>
                {
                    new SerializableAmmoData(AmmoType.Nato556, 250)
                },
                SpawnLocation = _map.SpawnCi
            });

            Teams.Add(new SerializableTeamData
            {
                Id = 1,
                Name = "PMC",
                RoleType = RoleTypeId.NtfSergeant,
                Score = 0,
                Players = new List<Player>(),
                LoadOut = new List<SerializableItemData> {
                    new SerializableItemData(false, 37), // ArmorCombat
                    new SerializableItemData(false, 20), // GunE11SR
                    new SerializableItemData(false, 33), // Adrenaline
                    new SerializableItemData(false, 25), // GrenadeHE
                    new SerializableItemData(false, 26), // GrenadeFlash
                    new SerializableItemData(false, 34), // Painkillers
                    new SerializableItemData(false, 34), // Painkillers
                },
                Ammo = new List<SerializableAmmoData>
                {
                    new SerializableAmmoData(AmmoType.Nato556, 250)
                },
                SpawnLocation = _map.SpawnNtf
            });
            yield break;
        }


        private IEnumerator<float> TeamAssignment()
        {
            Log.Warn("Running TeamAssignment");
            if (Player.List.Count == 2) 
            {
                ScoreLimit = 10;
                var p0 = Player.List.ElementAt(0);
                Teams.FirstOrDefault(x => x.Id == 0).Players.Add(p0);

                var p1 = Player.List.ElementAt(1);
                Teams.FirstOrDefault(x => x.Id == 1).Players.Add(p1);
            }
            else
            {
                for (int i = 0; i < (Math.Ceiling((double)Player.List.Count) / 2) + 1; i++)
                {
                    int playerid = Random.Range(0, Player.List.Count());
                    Player player = Player.List.ElementAt(playerid);
                    yield return Timing.WaitForSeconds(0.35f);
                    if (Teams.FirstOrDefault(x => x.Id == 0).Players.Contains(player) || Teams.FirstOrDefault(x => x.Id == 1).Players.Contains(player)) { continue; }
                    else
                    {
                        Teams.FirstOrDefault(x => x.Id == 0).Players.Add(player);
                        Log.Warn($"{player.DisplayNickname}, set to team 0");
                    }
                }


                foreach (Player player in Player.List)
                {
                    yield return Timing.WaitForSeconds(0.35f);
                    if (Teams.FirstOrDefault(x => x.Id == 0).Players.Contains(player) || Teams.FirstOrDefault(x => x.Id == 1).Players.Contains(player)) { continue; }
                    else
                    {
                        
                        Teams.FirstOrDefault(x => x.Id == 1).Players.Add(player);
                        Log.Warn($"{player.DisplayNickname}, set to team 1");
                    }
                }

            }

            _coroutines.Add(SpawnTeams().RunCoroutine());
            Manager.setModifier(0, $"<color=#A3334D>{Teams.FirstOrDefault(x => x.Id == 0).Name}:</color> {Teams.FirstOrDefault(x => x.Id == 0).Score} points");
            Manager.setModifier(1, $"<color=#292F9F>{Teams.FirstOrDefault(x => x.Id == 1).Name}:</color> {Teams.FirstOrDefault(x => x.Id == 1).Score} points");
            foreach (SerializableTeamData team in Teams)
            {

                Log.Warn($"{team.Name} has {team.Players.Count()} players");
                foreach (Player player in team.Players)
                {
                    Log.Warn($"Team: {team.Name} contains player: {player.DisplayNickname}");
                }

            }
        }

        private IEnumerator<float> RespawnTimer()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                if (Respawn.TimeUntilSpawnWave.Seconds >= 5f)
                {
                    Respawn.TimeUntilNextPhase = 5f;
                }
            }

        }
        private void RespawnWave(RespawningTeamEventArgs ev)
        {



            ev.IsAllowed = false;
            _coroutines.Add(SpawnTeams().RunCoroutine());
            Log.Info("Respawning Teams");

        }

        private void RoundStarted()
        {
            _coroutines.Add(Timing.RunCoroutine(RespawnTimer()));
            _coroutines.Add(Timing.RunCoroutine(TeamAssignment()));
            Respawn.TimeUntilNextPhase = 5f;

        }

        private IEnumerator<float> Win(string message)
        {
            foreach (Player p in Player.List)
            {
                yield return Timing.WaitForOneFrame;
                p.Broadcast(10, message);
            }
            yield return Timing.WaitForSeconds(10f);
            Round.Restart();
            ModulesManager.GetModule("TDM").Disable();
        }

        private void Died(DiedEventArgs ev)
        {
            foreach (Ragdoll ragdoll in Ragdoll.List.ToList())
            {
                ragdoll.Destroy();
            }
            foreach (Pickup pickup in Pickup.List.ToList())
            {
                pickup.Destroy();
            }

            if (Teams.FirstOrDefault(x => x.Id == 0).Players.Contains(ev.Attacker) && ev.Attacker != ev.Player)
            {
                Teams.FirstOrDefault(x => x.Id == 0).Score++;
            }
            else if (Teams.FirstOrDefault(x => x.Id == 1).Players.Contains(ev.Attacker) && ev.Attacker != ev.Player)
            {
                Teams.FirstOrDefault(x => x.Id == 1).Score++;
            }
            else
            {
                return;
            }


                Manager.setModifier(0, $"<color=#A3334D>{Teams.FirstOrDefault(x => x.Id == 0).Name}:</color> {Teams.FirstOrDefault(x => x.Id == 0).Score} points");
     
            Manager.setModifier(1, $"<color=#292F9F>{Teams.FirstOrDefault(x => x.Id == 1).Name}:</color> {Teams.FirstOrDefault(x => x.Id == 1).Score} points");

            

            if (Teams.FirstOrDefault(x => x.Id == 0).Score >= ScoreLimit)
            {
                _coroutines.Add(Timing.RunCoroutine(Win($"TEAM: <color=#A3334D>{Teams.FirstOrDefault(x => x.Id == 0).Name}</color> HAS WON THE GAME")));
            }
            if (Teams.FirstOrDefault(x => x.Id == 1).Score >= ScoreLimit)
            {
                _coroutines.Add(Timing.RunCoroutine(Win($"TEAM <color=#292F9F>{Teams.FirstOrDefault(x => x.Id == 1).Name}</color> HAS WON THE GAME")));
            }
        }
    }
}
