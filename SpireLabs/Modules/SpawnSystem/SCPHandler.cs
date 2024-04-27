using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.SpawnSystem
{
    
    internal class SCPHandler
    {
        public static List<Player> scPPs = new List<Player>();
        private static IEnumerator<float> SpawnBoner()
        {
            if (!Plugin.IsActiveEventround)
            {
                yield return Timing.WaitForSeconds(0.5f);
                var rnd = new Random();
                int dttiffss = rnd.Next(0, 100);
                Log.Info($"Boner RNG was {dttiffss} (should between 30 and 40)");
                if (dttiffss > 30 && dttiffss < 40)
                {
                    int personindex = rnd.Next(0, scPPs.Count());
                    Player.Get(scPPs.ElementAt(personindex).Id).RoleManager.ServerSetRole(RoleTypeId.Scp3114, RoleChangeReason.RemoteAdmin);
                }
            }
        }

        private IEnumerator<float> SpawnDoge(Player p)
        {
            yield return 0;
        }

        internal static void doSCPThings()
        {
            foreach(Player iP in Player.List)
            {
                if (iP.IsScp)
                {
                    scPPs.Add(iP);
                }
                else
                {
                    scPPs.Remove(scPPs.FirstOrDefault(x => x.Id == iP.Id));
                }
            }
            Timing.RunCoroutine(SpawnBoner());
        }
    }
}
