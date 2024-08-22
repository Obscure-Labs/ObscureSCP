using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using PlayerRoles;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.NonAllocLINQ;
using static ObscureLabs.Modules.Gamemode_Handler.Minigames.TeamHandler;
using Random = UnityEngine.Random;

namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    public class TDM : Module
    {


        public override string Name => "TDM";

        public override bool IsInitializeOnStart => false;

        public List<CoroutineHandle> _coroutines = new List<CoroutineHandle>();


        private readonly Dictionary<int, MapData> _maps = new()
        {
            { 0, new MapData("pvpA1_2t", "\"Low Effort\"", new Vector3(7.531f, 1108.563f, 32.980f), new Vector3(-19.887f, 1107.903f, 52.691f)) },
            { 1, new MapData("pvpA2_2t", "\"Tilted Towers\"",new Vector3(9.7f, 1102f, 52.46f), new Vector3(-20.20f, 1102f, 35.37f)) },
            { 2, new MapData("pvpMZA1_2t", "\"The Maze\"", new Vector3(23.96f, 1126f, 29.14f), new Vector3(-30.74f, 1126f, -16.93f)) },
        };

        public int ScoreLimit = 20;


        private MapData _map;

        private bool die;

        private bool running = false;

        public override bool Enable()
        {
            die = false;
            if (!running)
            {
                running = true;
            _coroutines.Add(Timing.RunCoroutine(RoundSetup()));
            }

            Teams = new List<SerializableTeamData>();

            Exiled.Events.Handlers.Server.RoundStarted += RoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam += RespawnWave;
            Exiled.Events.Handlers.Player.Died += Died;
            return base.Enable();
        }
        public override bool Disable()
        {
            //foreach (CoroutineHandle h in _coroutines)
            //{
            //    Timing.KillCoroutines(h);
            //}
            running = false;
            die = true;

            Teams = new List<SerializableTeamData>();
            Exiled.Events.Handlers.Server.RoundStarted -= RoundStarted;
            Exiled.Events.Handlers.Server.RespawningTeam -= RespawnWave;
            Exiled.Events.Handlers.Player.Died -= Died;
            return base.Disable();
        }


        private IEnumerator<float> RoundSetup()
        {
            die = false;
            Round.IsLocked = true;
            var range = Random.Range(0, 3);
            _map = _maps[range];

            Log.Warn("Unloading all default maps");
            MapEditorReborn.API.Features.MapUtils.LoadMap("empty");
            Log.Warn($"Loading Map: {_map.DisplayName}");
            MapEditorReborn.API.Features.MapUtils.LoadMap($"{_map.Name}");
            Log.Warn($"Loaded map: {_map.DisplayName}");

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
                    new SerializableAmmoData{ItemType = Exiled.API.Enums.AmmoType.Nato556, Quantity = 250}
                },
                SpawnLocation = _map.SpawnCi
            });
            Log.Info($"{Teams.FirstOrDefault(x => x.Id == 0).Name} WAS ADDED");

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
                    new SerializableAmmoData{ItemType = Exiled.API.Enums.AmmoType.Nato556, Quantity = 250}
                },
                SpawnLocation = _map.SpawnNtf
            });
            Log.Info($"{Teams.FirstOrDefault(x => x.Id == 1).Name} WAS ADDED");
            yield break;
        }


        private IEnumerator<float> TeamAssignment()
        {
            Log.Warn("Running TeamAssignment");
            if (Player.List.Count == 2) 
            {
                Log.Info("Only 2 players, assigning to teams");
                ScoreLimit = 10;
                var p0 = Player.List.ElementAt(0);
                Teams.FirstOrDefault(x => x.Id == 0).Players.Add(p0);

                var p1 = Player.List.ElementAt(1);
                Teams.FirstOrDefault(x => x.Id == 1).Players.Add(p1);
            }
            else
            {
                Log.Info("Assigning players to teams");
                for (int i = 0; i < (Math.Ceiling((double)Player.List.Count) / 2) + 1; i++)
                {
                    Log.Info($"Pointer is at player id {i}");
                    int playerid = Random.Range(0, Player.List.Count());
                    Player player = Player.List.ElementAt(playerid);
                    Log.Info($"Player ({player.DisplayNickname}) has been retrieved from server");
                    yield return Timing.WaitForSeconds(0.1f);
                    if (Teams.FirstOrDefault(x => x.Id == 0).Players.Contains(player) || Teams.FirstOrDefault(x => x.Id == 1).Players.Contains(player)) { Log.Info($"Player assignment cancelled due to player already being in a team"); continue; }
                    else
                    {
                        Log.Info($"{player.DisplayNickname} is being set to Team {Teams.FirstOrDefault(x => x.Id == 0).Name}");
                        Teams.FirstOrDefault(x => x.Id == 0).Players.Add(player);
                        Log.Warn($"{player.DisplayNickname}, set to team 0");
                    }
                }


                foreach (Player player in Player.List)
                {
                    yield return Timing.WaitForSeconds(0.1f);
                    if (Teams.FirstOrDefault(x => x.Id == 0).Players.Contains(player) || Teams.FirstOrDefault(x => x.Id == 1).Players.Contains(player)) { continue; }
                    else
                    {
                        
                        Teams.FirstOrDefault(x => x.Id == 1).Players.Add(player);
                        Log.Warn($"{player.DisplayNickname}, set to team 1");
                    }
                }

            }

            _coroutines.Add(Timing.RunCoroutine(SpawnTeams()));
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
                if (die)
                {
                    yield break;
                }
            }

        }
        private void RespawnWave(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = false;
            _coroutines.Add(Timing.RunCoroutine(SpawnTeams()));
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
