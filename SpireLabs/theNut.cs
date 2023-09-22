using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpireLabs
{
    internal class theNut
    {
        internal static void scp173DMG(HurtingEventArgs ev)
        {
            if(ev.Player.Role == RoleTypeId.Scp173)
            {
                var rnd = new Random();
                int num = rnd.Next(1, 100);
                if(num < 20 && num > 13)
                {
                    ev.Amount = 0;
                }
            }
        }
    }
}
