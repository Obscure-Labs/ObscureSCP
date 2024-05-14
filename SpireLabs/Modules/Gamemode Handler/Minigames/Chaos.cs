using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.API.Extensions;
using Exiled.API.Enums;
using CustomItems.API;
using Exiled.Events.EventArgs.Server;
using MapEditorReborn.API.Enums;
using Respawning;
using GameCore;
using Log = Exiled.API.Features.Log;
namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    internal class chaos : Plugin.Module
    {
        static bool gamemodeactive = false;
        public override string name { get; set; } = "ChaosRound";
        public override bool initOnStart { get; set; } = false;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Server.RespawningTeam += chaosroundRespawnWave;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Server.RespawningTeam -= chaosroundRespawnWave;
                base.Disable();
                return true;
            }
            catch { return false; };
        }


        public static void chaosroundRespawnWave(RespawningTeamEventArgs ev)
        {
            if (Plugin.IsActiveEventround&& Plugin.EventRoundType != "chaos")
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
                List<Player> respawningPlayers = new List<Player>();
                respawningPlayers = ev.Players;
                var respawningTeam = ev.NextKnownTeam;
                if (ev.NextKnownTeam == SpawnableTeamType.ChaosInsurgency)
                {
                    foreach (Player rP in respawningPlayers)
                    {
                        rP.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
                        rP.ClearInventory();
                        rP.AddItem(ItemType.Coin, 2);
                        rP.AddItem(ItemType.KeycardMTFCaptain, 1);
                        rP.AddItem(ItemType.Flashlight, 1);
                        rP.Teleport(RoomType.EzDownstairsPcs);
                        Respawn.NtfTickets = 100;
                        Respawn.TimeUntilNextPhase = 45;
                    }
                }
                else
                {
                    foreach (Player rP in respawningPlayers)
                    {
                        rP.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.UseSpawnpoint);
                        rP.ClearInventory();
                        rP.AddItem(ItemType.Coin, 2);
                        rP.AddItem(ItemType.KeycardMTFCaptain, 1);
                        rP.AddItem(ItemType.Flashlight, 1);
                        rP.Teleport(RoomType.EzDownstairsPcs);
                        Respawn.ChaosTickets = 100;
                        Respawn.TimeUntilNextPhase = 45;
                    }
                }
            }



        }

        public static IEnumerator<float> runChaos()
        {
   
            var rnd69 = new System.Random();
            Log.Warn("Running Chaos Round");


            Timing.WaitForSeconds(0.1f);
            List<Player> newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime = new List<Player>();

            newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime = Plugin.PlayerList;

            for (int i = 0; i < (Math.Ceiling((double)Plugin.PlayerList.Count) / 2) + 1; i++)
            {

                int playerid = rnd69.Next(0, Plugin.PlayerList.Count());
                Player p = newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime.ElementAt(playerid);
                yield return Timing.WaitForSeconds(0.5f);
                if (p.Role != RoleTypeId.Overwatch)
                {
                    p.Broadcast(5, "<color=green><b>CHAOS ROUND!");
                    p.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
                    p.ClearInventory();
                    p.AddItem(ItemType.Coin, 2);
                    p.AddItem(ItemType.KeycardScientist, 1);
                    p.AddItem(ItemType.Flashlight, 1);
                    p.Teleport(RoomType.LczClassDSpawn);
                }
                newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime.Remove(p);
            }
            foreach (Player p in newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime)
            {
                if (p.Role != RoleTypeId.Overwatch)
                {
                    p.Broadcast(5, "<color=green><b>CHAOS ROUND!");
                    p.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.UseSpawnpoint);
                    p.ClearInventory();
                    p.AddItem(ItemType.Coin, 2);
                    p.AddItem(ItemType.KeycardScientist, 1);
                    p.AddItem(ItemType.Flashlight, 1);
                    p.Teleport(RoomType.LczClassDSpawn);

                }

            }
            Respawn.TimeUntilNextPhase = 45;
        }
    }
}
