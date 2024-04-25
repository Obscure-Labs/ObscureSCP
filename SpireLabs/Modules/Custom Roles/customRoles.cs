using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using PlayerRoles;
using Respawning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCRAPI = UncomplicatedCustomRoles.API.Features.Manager;
using SpireSCP.GUI.API.Features;
using Exiled.API.Enums;
using System.Xml.Linq;

namespace ObscureLabs
{
    internal class customRoles
    {
        internal class roleData
        {
            public Player player;
            public int? UCRID;
        }

        internal static List<roleData> rd = new List<roleData>();

        internal static IEnumerator<float> CheckRoles(Player p)
        {
            string scps = string.Empty;
            int counter = 0;
            foreach (Player pp in Player.List)
            {
                if (pp.Role.Team == Team.SCPs)
                {
                    if (counter != 0)
                    {
                        scps += $"<color=white>, <color=red>{pp.Role.Name}";
                    }
                    else
                    {
                        scps += $"<color=red>{pp.Role.Name}";
                    }
                    counter++;
                }
            }
            int? UCRID = null;
            yield return Timing.WaitForSeconds(0.5f);
            if (UCRAPI.HasCustomRole(p))
            {
                UCRID = UCRAPI.Get(p).Id;

                if (UCRID == 4)
                {
                    yield return Timing.WaitForSeconds(6);
                    Manager.SendHint(p, $"<b>The currently active SCP subjects are: {scps}", 15);
                }

                if (UCRID == 3)
                {
                    p.ChangeAppearance(RoleTypeId.FacilityGuard);
                }
            }
        }
    }
}
//        internal static void spawnWave(RespawningTeamEventArgs ev)
//        {
//            var random = new Random();

//            SpawnableTeamType team = ev.NextKnownTeam;
//            try
//            {
//                if (team == SpawnableTeamType.ChaosInsurgency)
//                {
//                    Player pl = ev.Players[random.Next(0, ev.Players.Count())];
//                    var chance = random.Next(0, 100);
//                    if (chance > 60)
//                    {
//                        Log.Info(chance);
//                        UncomplicatedCustomRoles.Manager.SpawnManager.SummonCustomSubclass(pl, 9, true);
//                        Timing.RunCoroutine(SpawnCustom(pl, 9));
//                    }
//                }
//                if(team == SpawnableTeamType.NineTailedFox)
//                {
//                    Player pl = ev.Players[random.Next(0, ev.Players.Count())];
//                    int chance = random.Next(0, 100);
//                    if (chance > 85)
//                    {
//                        Log.Info(chance);
//                        Timing.RunCoroutine(SpawnCustom(pl, 8));
//                    }
//                }
//            }
//            catch { }
//        }

//        private static IEnumerator<float> SpawnCustom(Player p, int role)
//        {
//            yield return Timing.WaitForSeconds(0.1f);
//            if(role == 8)
//            {
//                UncomplicatedCustomRoles.Manager.SpawnManager.SummonCustomSubclass(p, 8, true);
//            }
//            else if(role==9)
//            {
//                UncomplicatedCustomRoles.Manager.SpawnManager.SummonCustomSubclass(p, 9, true);
//            }

//        }
//    }
//}
