using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System.Collections.Generic;
using System.Linq;

namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{

    internal class Juggernaut
    {
        public static bool IsGameModeEnabled => false;

        private static readonly IReadOnlyDictionary<ItemType, int> _itemsToGive = new Dictionary<ItemType, int>()
        {
            { ItemType.KeycardGuard, 1 },
            { ItemType.Ammo9x19, 40 },
            { ItemType.GunCOM15, 1 },
            { ItemType.ArmorCombat, 1 },
            { ItemType.Coin, 1 },
            { ItemType.Flashlight, 1 }
        };

        public static IEnumerator<float> DWaveCoroutine()
        {
            var s = Round.StartedTime;

            if (Plugin.IsActiveEventround)
            {

            }

            var wavecount = 0;

            while (!Round.IsEnded)// This is a safe infinite loop 
            {
                Log.Info($"Running Juggernaut Round Respawn Wave Handler for count: {wavecount}");

                Respawn.ChaosTickets += 100;
                Respawn.TimeUntilNextPhase = 120f;

                yield return Timing.WaitForSeconds(120f);
                Respawn.ForceWave(Respawning.SpawnableTeamType.ChaosInsurgency, false);
                wavecount++;
                if (Round.StartedTime != s)
                {
                    break;
                }
            }
        }

        public static IEnumerator<float> RunJuggernautCoroutine()
        {
            Log.Warn("Running Juggernaut round!");
            Respawn.TimeUntilNextPhase = 120f;
            Respawn.ChaosTickets = 100;
            Timing.RunCoroutine(DWaveCoroutine(), "juggerwave");
            yield return Timing.WaitForSeconds(0.5f);
            Log.Warn("Started Juggernaut Round");

            var juggernautPlayer = Player.List.ElementAt(UnityEngine.Random.Range(0, Player.List.Count + 1));
            var maxjhp = 525;
            juggernautPlayer.Role.Set(RoleTypeId.Scientist, RoleSpawnFlags.UseSpawnpoint);
            juggernautPlayer.Teleport(RoomType.HczArmory);
            juggernautPlayer.AddItem(ItemType.KeycardO5);
            Exiled.CustomItems.API.Features.CustomItem.Get((uint)5).Give(juggernautPlayer); //ER16 laser gun
            juggernautPlayer.Broadcast(5, "<color=red><b>You are the Juggernaut!</color> \nKill or be killed.");

            Log.Warn("Spawned Juggernaut");

            yield return Timing.WaitForSeconds(0.5f);

            foreach (var player in Player.List)
            {
                if (player == juggernautPlayer || player == null)
                {
                    continue;
                }

                Log.Warn($"Player: {player.Nickname} is going to be ClassD");

                yield return Timing.WaitForSeconds(0.01f);

                player.Role.Set(RoleTypeId.ClassD, RoleSpawnFlags.UseSpawnpoint);
                player.ClearInventory();

                Log.Warn($"Player: {player.Nickname} spawned");

                foreach (var item in _itemsToGive)
                {
                    Log.Warn($"Player: {player.Nickname} got {item.Key}");
                    player.AddItem(item.Key, item.Value);
                }

                maxjhp += 175;
                Log.Warn($"Player: {player.Nickname} increase juggernaut health by 75");
                player.Broadcast(5, $"<color=green><b>{juggernautPlayer.Nickname} is the Juggernaut, hunt them down!");

            }

            yield return Timing.WaitForSeconds(0.5f);
            juggernautPlayer.Health = maxjhp;
            juggernautPlayer.MaxHealth = maxjhp;
        }
    }
}
