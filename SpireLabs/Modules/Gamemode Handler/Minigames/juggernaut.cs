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
using SpireSCP.GUI.API.Features;
using UnityEngine;
using CustomPlayerEffects;
namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    internal static class juggernaut
    {
        static bool gamemodeactive = false;


        public static IEnumerator<float> dWave()
        {
            if (Plugin.IsActiveEventround) { }
            int wavecount = 0;
            while (true)// This is a safe infinite loop 
            {
                Log.Info($"Running Juggernaut Round Respawn Wave Handler for count: {wavecount}");
                Respawn.ChaosTickets = 100;
                Respawn.TimeUntilNextPhase = 120f;
                yield return Timing.WaitForSeconds(120f);
                Respawn.ForceWave(Respawning.SpawnableTeamType.ChaosInsurgency, false);
                wavecount++;

            }
        }

        public static IEnumerator<float> runJuggernaut()
        {
            var rnd69 = new System.Random();
            Log.Warn("Running Juggernaut round!");
            Respawn.TimeUntilNextPhase = 120f;
            Respawn.ChaosTickets = 100;
            
            Timing.RunCoroutine(dWave());
            yield return Timing.WaitForSeconds(0.5f);
            List<Player> newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime = new List<Player>();
            Log.Warn("Started Juggernaut Round");
            newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime = Plugin.PlayerList;

            var juggernautPlayer = Plugin.PlayerList.ElementAt(rnd69.Next(0, Plugin.PlayerList.Count + 1));
            var maxjhp = 725;
            juggernautPlayer.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.UseSpawnpoint);
            juggernautPlayer.Teleport(RoomType.HczArmory);
            juggernautPlayer.AddItem(ItemType.KeycardO5);
            Exiled.CustomItems.API.Features.CustomItem.Get((uint)5).Give(juggernautPlayer); //ER16 laser gun
            juggernautPlayer.ChangeAppearance(RoleTypeId.Tutorial, false);
            juggernautPlayer.Broadcast(5, "<color=red><b>You are the Juggernaut!</color> \nKill or be killed.");


            Log.Warn("Spawned Juggernaut");

            yield return Timing.WaitForSeconds(0.5f);
            foreach (Player p in newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime)
            {
                if (p == juggernautPlayer) { continue; }
                Log.Warn($"Player: {p.Nickname} is going to be ClassD");
                    yield return Timing.WaitForSeconds(0.01f);
                p.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
                p.ClearInventory();
                Log.Warn($"Player: {p.Nickname} spawned");
                p.AddItem(ItemType.KeycardGuard);
                Log.Warn($"Player: {p.Nickname} got keycard");
                p.AddItem(ItemType.Ammo9x19, 40);
                p.AddItem(ItemType.GunCOM15);
                p.AddItem(ItemType.ArmorHeavy);
                Log.Warn($"Player: {p.Nickname} got gun and ammo and armor");
                p.AddItem(ItemType.Coin);
                Log.Warn($"Player: {p.Nickname} got coin");
                p.AddItem(ItemType.Flashlight);
                Log.Warn($"Player: {p.Nickname} got flashlight");

                maxjhp += 175;
                Log.Warn($"Player: {p.Nickname} increase juggernaut health by 75");
                p.Broadcast(5, $"<color=green><b>{juggernautPlayer.Nickname} is the Juggernaut, hunt them down!");

            }
            juggernautPlayer.MaxArtificialHealth = maxjhp;
            juggernautPlayer.MaxHealth = maxjhp;
            juggernautPlayer.Heal(maxjhp, true);


        }
    }
}
