using AudioPlayer.Commands.SubCommands;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using MapEditorReborn;
using UnityEngine;
using Exiled.API.Enums;
using CustomItems.API;
using SpireLabs.Items;
using InventorySystem.Items;
using Exiled.API.Features.Items;
using PluginAPI.Events;
using System.Runtime.Remoting.Messaging;

namespace SpireLabs.Gamemode_Handler
{


    internal static class jailBirdTDM
    {






        static string mapName = "empty";
        public static string[] maps = { "pvpA1_2t", "pvpA2_2t", "pvpRA1_2t", "pvpMZA1_2t" };
        static bool gamemodeactive = false;
        static bool team = true;
        public static IEnumerator<float> startJbTDM()
        {



            bool gamemodeactive = false;
            var item = ItemType.Coin;
            var rnd = new System.Random();
            int num = rnd.Next(1, 100);
            Log.Info($"Minigame RNG was: {num} (should be between 30-60)");
            if (num >= 30 && num <= 60)
            {
                Timing.RunCoroutine(runJbTDM());

                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> lateJoin()
            {
            var loop = true;




            yield return Timing.WaitForSeconds(1.25f);
            int? count = 0;
                while (gamemodeactive = true)
                {
                    yield return Timing.WaitForSeconds(0.1f);
                    foreach (Player p in Player.List)
                {


                    if (count >= 0 && count <= 120)
                    {
                        if (p == null)
                        {
                        }
                        else
                        {
                            if (p.Role.Type != RoleTypeId.ChaosConscript && p.Role.Type != RoleTypeId.NtfSergeant && p.Role.Type != RoleTypeId.Spectator && p.Role.Type != RoleTypeId.None)
                            {
                                p.Role.Set(RoleTypeId.Spectator);
                                Log.Warn($"{p.DisplayNickname} joined late (or was assigned the wrong role somehow) and has been set to spectator");
                            }

                        }
                        yield return Timing.WaitForSeconds(0.25f);
                        count++;
                    }


                }
                }

            }

        public static IEnumerator<float> runJbTDM()
        {
            Vector3 spawnCI = new Vector3(0f, 1000f, 0f);
            Vector3 spawnNTF = new Vector3(0f, 1000f, 0f);
            Respawn.TimeUntilNextPhase = 86400;
            var rndMap = new System.Random();
            int numMap = rndMap.Next(0, 4);
            var rnd69 = new System.Random();
            int num69 = rnd69.Next(0, 4);
            switch (rndMap.Next(0, maps.Count()))
                {
                    case 0: mapName = maps[0]; spawnCI = new Vector3(8.48f, 1106.5f, 30.46f); spawnNTF = new Vector3(-20.8f, 1107.5f, 51.66f); break;
                    case 1: mapName = maps[1]; spawnCI = new Vector3(9.7f, 1102f, 52.46f); spawnNTF = new Vector3(-20.20f, 1102f, 35.37f); break;
                    case 2:
                        mapName = maps[2]; 
                        spawnCI = new Vector3(-53.97f, 1107f, 57.76f); 
                        spawnNTF = new Vector3(-53.97f, 1107f, 29.32f);
                        num69 = rnd69.Next(0, 3); // This is to prevent balls and grenades on this map due to it being smaller and easy for players to run out of items before anyone actually dies
                        break;
                    case 3:
                        mapName = maps[3];
                        spawnCI = new Vector3(23.96f, 1126f, 29.14f);
                        spawnNTF = new Vector3(-30.74f, 1126f, -16.93f);
                        num69 = rnd69.Next(0, 3); // This is to prevent balls and grenades on this map due to it being far too big and easy for players to run out of items before anyone actually dies
                    break;
            }

            gamemodeactive = true;
                Log.Warn("Unloading all default maps");
                MapEditorReborn.API.Features.MapUtils.LoadMap("empty");
                Log.Warn("Loading Map: pvpA1_2t");
                MapEditorReborn.API.Features.MapUtils.LoadMap($"{mapName}");
                Log.Warn("Starting checks for players with wrong roles");
                Timing.WaitForSeconds(0.6f);
                Timing.RunCoroutine(lateJoin());

            foreach (Player p in Player.List)
                {

                yield return Timing.WaitForSeconds(0.1f);

                if (team == true)
                {
                    Log.Warn($"{p.DisplayNickname} is CHAOS INSURGENCY TEAM");
                    team = false;
                    p.Role.Set(RoleTypeId.ChaosConscript);
                    p.ClearInventory(true);
                    p.Teleport(spawnCI);
                    p.EnableEffect(EffectType.RainbowTaste, 999, false);
                    p.EnableEffect(EffectType.Ensnared, 10, false);
                    p.EnableEffect(EffectType.Flashed, 10, false);
                    p.EnableEffect(EffectType.SoundtrackMute, 999, false);
                    p.EnableEffect(EffectType.DamageReduction, 10, false);
                    p.ChangeEffectIntensity(EffectType.DamageReduction, 255, 1f);
                }
                else
                {
                    Log.Warn($"{p.DisplayNickname} is NTF TEAM");
                    team = true;
                    p.Role.Set(RoleTypeId.NtfSergeant);
                    p.ClearInventory(true);
                    p.Teleport(spawnNTF);
                    p.EnableEffect(EffectType.RainbowTaste, 999, false);
                    p.EnableEffect(EffectType.Ensnared, 10, false);
                    p.EnableEffect(EffectType.Flashed, 10, false);
                    p.EnableEffect(EffectType.SoundtrackMute, 999, false);
                    p.EnableEffect(EffectType.DamageReduction, 10, false);
                    p.ChangeEffectIntensity(EffectType.DamageReduction, 255, 1f);
                }
                p.Broadcast(5, "<color=green><b>MINIGAME ROUND BEGINS IN 10S!");
                if (num69 == 0)
                {
                    Log.Warn("Giving Snipers");
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)1).Give(p);
                    p.AddAmmo(AmmoType.Ammo44Cal, 99);
                }
                if (num69 == 1)
                {
                    Log.Warn("Giving Jailbirds");
                    var item = p.AddItem(ItemType.Jailbird);
                    p.CurrentItem = item;
                }
                if (num69 == 2)
                {
                    Log.Warn("Giving Particle Disrupters");
                    Item item = p.AddItem(ItemType.ParticleDisruptor);
                    p.AddItem(ItemType.ParticleDisruptor);
                    p.CurrentItem = item;
                }
                if (num69 == 3)
                {
                    Log.Warn("Giving Grenades and Balls");
                    var item = p.AddItem(ItemType.SCP018);
                    p.AddItem(ItemType.GrenadeHE);
                    p.AddItem(ItemType.GrenadeHE);
                    p.AddItem(ItemType.SCP018);
                    p.AddItem(ItemType.GrenadeHE);
                    p.AddItem(ItemType.GrenadeHE);
                    p.AddItem(ItemType.SCP018);
                    p.AddItem(ItemType.GrenadeHE);
                    p.EnableEffect(EffectType.Scp207, 999, false);
                    p.CurrentItem = item;
                }
                p.Scale = new Vector3(1, 1, 1);

                yield return Timing.WaitForOneFrame;
            }
            }
        }
    }
