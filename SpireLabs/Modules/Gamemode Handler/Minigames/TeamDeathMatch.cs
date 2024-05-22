using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using MEC;
using ObscureLabs.API.Data;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ObscureLabs.Gamemode_Handler
{
    public static class TeamDeathMatch
    {
        public static bool IsGameModeEnabled => false;

        private static readonly Dictionary<int, MapData> _maps = new()
        {
            { 0, new MapData("pvpA1_2t", "\"Low Effort\"", new Vector3(8.48f, 1106.5f, 30.46f), new Vector3(-20.8f, 1107.5f, 51.66f)) },
            { 1, new MapData("pvpA2_2t", "\"Tilted Towers\"",new Vector3(9.7f, 1102f, 52.46f), new Vector3(-20.20f, 1102f, 35.37f)) },
            { 2, new MapData("pvpMZA1_2t", "\"The Maze\"", new Vector3(23.96f, 1126f, 29.14f), new Vector3(-30.74f, 1126f, -16.93f)) },
        };

        private static MapData _map;

        public static IEnumerator<float> runJbTDM()
        {
            var spawnCI = new Vector3(0f, 1000f, 0f);
            var spawnNTF = new Vector3(0f, 1000f, 0f);
            var range = Random.Range(0, 3);
            _map = _maps[range];

            int number;
            if (range == 2)
            {
                number = Random.Range(0, 3);
            }
            else
            {
                number = Random.Range(0, 4);
            }

            Log.Warn("Unloading all default maps");
            MapEditorReborn.API.Features.MapUtils.LoadMap("empty");
            Log.Warn("Loading Map: pvpA1_2t");
            MapEditorReborn.API.Features.MapUtils.LoadMap($"{_map.Name}");
            Log.Warn("Starting checks for players with wrong roles");

            Timing.WaitForSeconds(0.1f);
            if (Player.List.Count == 2)
            {
                var player = Player.List.ElementAt(0);
                player.Role.Set(RoleTypeId.ChaosConscript);
                player.ClearInventory(true);
                player.Teleport(spawnCI);
                EnableSomeEffects(player);

                yield return Timing.WaitForOneFrame;

                player.ChangeAppearance(RoleTypeId.ChaosConscript);

                var player1 = Player.List.ElementAt(1);
                player1.Role.Set(RoleTypeId.NtfSergeant);

                player1.ClearInventory(true);
                player1.Teleport(spawnNTF);
                EnableSomeEffects(player);

                yield return Timing.WaitForOneFrame;

                player1.ChangeAppearance(RoleTypeId.NtfSergeant);
            }
            else
            {
                for (int i = 0; i < (Math.Ceiling((double)Player.List.Count) / 2) + 1; i++)
                {
                    int playerid = Random.Range(0, Player.List.Count());
                    Player player = Player.List.ElementAt(playerid);
                    yield return Timing.WaitForSeconds(0.5f);
                    if (player.Role.Type is not RoleTypeId.ChaosConscript)
                    {
                        player.Role.Set(RoleTypeId.ChaosConscript);
                        player.ClearInventory(true);
                        player.Teleport(spawnCI);
                        EnableSomeEffects(player);
                        yield return Timing.WaitForOneFrame;
                        player.ChangeAppearance(RoleTypeId.ChaosConscript);

                    }
                    //Player.List.Remove(p);
                }
                foreach (Player player in Player.List)
                {
                    if (player.Role.Type is not RoleTypeId.ChaosConscript)
                    {
                        player.Role.Set(RoleTypeId.NtfSergeant);
                        EnableSomeEffects(player);
                        player.ClearInventory(true);
                        player.Teleport(spawnNTF);

                        yield return Timing.WaitForOneFrame;
                        player.ChangeAppearance(RoleTypeId.NtfSergeant);
                    }

                }
            }
            //Log.Info($"Setting player: {p.Nickname} to team: {team}");

            foreach (Player player in Player.List)
            {
                yield return Timing.WaitForSeconds(0.1f);

                player.Broadcast(10, $"<color=green><b>TEAM DEATHMATCH BEGINS IN 10S! \nThe current map is: {_map.DisplayName}");
                if (number == 0)
                {
                    Log.Warn("Giving Snipers");
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)1).Give(player);
                    player.AddAmmo(AmmoType.Ammo44Cal, 99);
                }
                else if (number == 1)
                {
                    Log.Warn("Giving Jailbirds");
                    var item = player.AddItem(ItemType.Jailbird);
                    player.CurrentItem = item;
                }
                else if (number == 2)
                {
                    Log.Warn("Giving Particle Disrupters");
                    Item item = player.AddItem(ItemType.ParticleDisruptor);
                    player.AddItem(ItemType.ParticleDisruptor);
                    player.CurrentItem = item;
                }
                else if (number == 3)
                {
                    Log.Warn("Giving ER16's");
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)5).Give(player);

                }
                else if (number == 4)
                {
                    Log.Warn("Giving Grenades and Balls");
                    var item = player.AddItem(ItemType.SCP018);
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)2).Give(player);
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)2).Give(player);
                    player.AddItem(ItemType.SCP018);
                    player.AddItem(ItemType.GrenadeHE);
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)4).Give(player);
                    player.AddItem(ItemType.SCP018);
                    player.AddItem(ItemType.GrenadeHE);
                    player.EnableEffect(EffectType.Scp207, 999, false);
                    player.CurrentItem = item;
                }

                player.Scale = new Vector3(1, 1, 1);

                yield return Timing.WaitForOneFrame;
            }

            Respawn.TimeUntilNextPhase = 3600f;
        }

        private static void EnableSomeEffects(Player player)
        {
            player.EnableEffect(EffectType.RainbowTaste, 999, false);
            player.EnableEffect(EffectType.Ensnared, 10, false);
            player.EnableEffect(EffectType.Flashed, 10, false);
            player.EnableEffect(EffectType.SoundtrackMute, 999, false);
            player.EnableEffect(EffectType.DamageReduction, 10, false);
            player.ChangeEffectIntensity(EffectType.DamageReduction, 255, 1f);
        }
    }
}
