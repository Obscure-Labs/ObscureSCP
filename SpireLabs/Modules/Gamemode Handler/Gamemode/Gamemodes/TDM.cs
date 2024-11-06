using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using ObscureLabs.API.Data;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
using PlayerRoles;
using UnityEngine;
using static ObscureLabs.Modules.Gamemode_Handler.Minigames.TeamHandler;

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

        private void PlayerJoinInProgress(JoinedEventArgs ev)
        {
            Teams.FirstOrDefault(x => x.Players.Count == Teams.Min(x => x.Players.Count)).Players.Add(ev.Player);
            base.PlayerJoin(ev.Player);
        }

        public override void Start()
        {
            Exiled.Events.Handlers.Player.Joined -= PlayerJoin;
            SpawnTeams(Teams);
            Exiled.Events.Handlers.Player.Joined += PlayerJoinInProgress;
            base.Start();
        }

        public override void End()
        {
            Exiled.Events.Handlers.Player.Joined -= PlayerJoinInProgress;
            base.End();
        }
    }
}
