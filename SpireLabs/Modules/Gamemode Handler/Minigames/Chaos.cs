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
namespace ObscureLabs.Modules.Gamemode_Handler.Minigames
{
    internal static class Chaos
    {
        static bool gamemodeactive = false;


        public static IEnumerator<float> runChaos()
        {
            gamemodeactive = true;
            var rnd69 = new System.Random();
            Log.Warn("Starting checks for players with wrong roles");
            Respawn.TimeUntilNextPhase = 300f;

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
                    p.Teleport(RoomType.LczGlassBox);
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
                    p.Teleport(RoomType.LczGlassBox);
                }

            }

        }
    }
}
