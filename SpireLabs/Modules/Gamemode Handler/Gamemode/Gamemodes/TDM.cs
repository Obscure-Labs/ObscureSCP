using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using ObscureLabs.API.Data;
using PlayerRoles;
using UnityEngine;
using static ObscureLabs.Modules.Gamemode_Handler.Minigames.TeamHandler;
using MEC;
using SpireSCP.GUI.API.Features;

namespace ObscureLabs.Modules.Gamemode_Handler.Gamemode.Gamemodes
{
    internal class TDM : API.Features.Gamemode
    {
        public override string Name => "TDM";
        public override string HintText => "Team Deathmatch";

        private readonly List<MapData> _maps = new()
        {
            { new MapData("pvpA1_2t", "\"Low Effort\"", new Vector3(7.531f, 1108.563f, 32.980f), new Vector3(-19.887f, 1107.903f, 52.691f)) },
            { new MapData("pvpA2_2t", "\"Tilted Towers\"",new Vector3(9.7f, 1102f, 52.46f), new Vector3(-20.20f, 1102f, 35.37f)) },
            { new MapData("pvpMZA1_2t", "\"The Maze\"", new Vector3(23.96f, 1126f, 29.14f), new Vector3(-30.74f, 1126f, -16.93f)) },
        };

        private bool ModeRunning = false;
        private DateTime _roundStartTime;

        public override void Enable()
        {
            Round.IsLocked = true;
            Round.IsLobbyLocked = true;

            var _map = _maps.GetRandomValue();
            Teams.Add(new SerializableTeamData
            {
                Id = 0,
                Name = "Conscripts",
                RoleType = RoleTypeId.ChaosConscript,
                Score = 0,
                Players = new List<Player>(),
                LoadOut = new List<SerializableItemData> {
                    new(false, 37), // ArmorCombat
                    new(false, 20), // GunE11SR
                    new(false, 33), // Adrenaline
                    new(false, 25), // GrenadeHE
                    new(false, 26), // GrenadeFlash
                    new(false, 34), // Painkillers
                    new(false, 34), // Painkillers
                },
                Ammo = new List<SerializableAmmoData>
                {
                    new() {ItemType = Exiled.API.Enums.AmmoType.Nato556, Quantity = 250}
                },
                SpawnLocation = _map.SpawnCi
            });
            Teams.Add(new()
            {
                Id = 1,
                Name = "PMC",
                RoleType = RoleTypeId.NtfSergeant,
                Score = 0,
                Players = new List<Player>(),
                LoadOut = new List<SerializableItemData> {
                    new(false, 37), // ArmorCombat
                    new(false, 20), // GunE11SR
                    new(false, 33), // Adrenaline
                    new(false, 25), // GrenadeHE
                    new(false, 26), // GrenadeFlash
                    new(false, 34), // Painkillers
                    new(false, 34), // Painkillers
                },
                Ammo = new List<SerializableAmmoData>
                {
                    new() {ItemType = Exiled.API.Enums.AmmoType.Nato556, Quantity = 250}
                },
                SpawnLocation = _map.SpawnNtf
            });
            Exiled.Events.Handlers.Player.Joined += PlayerJoin;
            base.Enable();
        }

        private void PlayerJoin(JoinedEventArgs ev)
        {
            Teams.FirstOrDefault(x => x.Players.Count == Teams.Min(x => x.Players.Count)).Players.Add(ev.Player);
            base.PlayerJoin(ev.Player);
        }

#pragma warning disable CS0114
        private void PlayerJoinInProgress(JoinedEventArgs ev)
        {
            var selectedTeam = Teams.FirstOrDefault(x => x.Players.Count == Teams.Min(x => x.Players.Count));
            Teams.FirstOrDefault(x => x == selectedTeam).Players.Add(ev.Player);
#pragma warning  restore CS0114
            Timing.RunCoroutine(SpawnPlayer(ev.Player, selectedTeam));
            base.PlayerJoin(ev.Player);
        }

        public override void Start()
        {
            _roundStartTime = DateTime.UtcNow;
            Timing.RunCoroutine(EndTimer());
            Exiled.Events.Handlers.Player.Joined -= PlayerJoin;
            Timing.RunCoroutine(SpawnTeams(Teams));
            ModeRunning = true;
            Timing.RunCoroutine(PointsDisplay());
            Exiled.Events.Handlers.Player.Joined += PlayerJoinInProgress;
            base.Start();
        }

        public IEnumerator<float> PointsDisplay()
        {
            while (ModeRunning)
            {
                foreach (var team in Teams)
                {
                    Manager.setModifier(0, $"<color=white>Timer: {-(DateTime.UtcNow - _roundStartTime.AddMinutes(5))}</color>");
                    Manager.setModifier(1, $"<color=red>Conscripts: {Teams[0].Score}</color>");
                    Manager.setModifier(2, $"<color=blue>PMC: {Teams[1].Score}</color>");
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public IEnumerator<float> EndTimer()
        {
            while(ModeRunning)
            {
                if (_roundStartTime.AddMinutes(5) > DateTime.UtcNow)
                {
                    End();
                }
                yield return Timing.WaitForSeconds(1f);
            }
        }

        public override void End()
        {
            ModeRunning = false;
            Exiled.Events.Handlers.Player.Joined -= PlayerJoinInProgress;
            base.End();
        }
    }
}
