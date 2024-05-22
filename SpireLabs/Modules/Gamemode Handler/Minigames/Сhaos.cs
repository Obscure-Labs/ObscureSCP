using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using Log = Exiled.API.Features.Log;

namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    public class Chaos : Module
    {
        public override string Name => "ChaosRound";

        public override bool IsInitializeOnStart => false;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeam;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeam;

            return base.Disable();
        }

        public static void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (Plugin.IsActiveEventround && Plugin.EventRoundType is not API.Enums.EventRoundType.Chaos)
            {
                return;
            }

            if (!Plugin.IsActiveEventround)
            {
                return;
            }

            else
            {
                Log.Info(Plugin.EventRoundType);
                ev.IsAllowed = false;
                Log.Info("Run ChaosMode Spawn wave");

                if (ev.NextKnownTeam is SpawnableTeamType.ChaosInsurgency)
                {
                    foreach (Player player in ev.Players)
                    {
                        player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
                        player.ClearInventory();
                        player.AddItem(ItemType.Coin, 2);
                        player.AddItem(ItemType.KeycardMTFCaptain, 1);
                        player.AddItem(ItemType.Flashlight, 1);
                        player.Teleport(RoomType.EzDownstairsPcs);
                        Respawn.NtfTickets = 100;
                        Respawn.TimeUntilNextPhase = 45;
                    }
                }
                else
                {
                    foreach (Player player in ev.Players)
                    {
                        player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.UseSpawnpoint);
                        player.ClearInventory();
                        player.AddItem(ItemType.Coin, 2);
                        player.AddItem(ItemType.KeycardMTFCaptain, 1);
                        player.AddItem(ItemType.Flashlight, 1);
                        player.Teleport(RoomType.EzDownstairsPcs);
                        Respawn.ChaosTickets = 100;
                        Respawn.TimeUntilNextPhase = 45;
                    }
                }
            }
        }

        public static IEnumerator<float> RunChaosCoroutine()
        {
            Log.Warn("Running Chaos Round");

            Timing.WaitForSeconds(0.1f);

            for (int i = 0; i < (Math.Ceiling((double)Player.List.Count) / 2) + 1; i++)
            {
                var playerid = UnityEngine.Random.Range(0, Player.List.Count());
                var player = Player.List.ElementAt(playerid);

                yield return Timing.WaitForSeconds(0.5f);

                if (player.Role != RoleTypeId.Overwatch)
                {
                    player.Broadcast(5, "<color=green><b>CHAOS ROUND!");
                    player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
                    player.ClearInventory();
                    player.AddItem(ItemType.Coin, 2);
                    player.AddItem(ItemType.KeycardScientist, 1);
                    player.AddItem(ItemType.Flashlight, 1);
                    player.Teleport(RoomType.LczClassDSpawn);
                }
                //Player.List.Remove(p);
            }

            foreach (var player in Player.List)
            {
                if (player.Role != RoleTypeId.Overwatch)
                {
                    player.Broadcast(5, "<color=green><b>CHAOS ROUND!");
                    player.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.UseSpawnpoint);
                    player.ClearInventory();
                    player.AddItem(ItemType.Coin, 2);
                    player.AddItem(ItemType.KeycardScientist, 1);
                    player.AddItem(ItemType.Flashlight, 1);
                    player.Teleport(RoomType.LczClassDSpawn);
                }
            }

            Respawn.TimeUntilNextPhase = 45;
        }
    }
}
