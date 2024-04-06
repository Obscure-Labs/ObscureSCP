using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class DamageModifiers
    {
        public static void SetDamageModifiers(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type == DamageType.MicroHid)
            {
                ev.Amount *= (Plugin.hidDPS / 100);
            }
            if (ev.DamageHandler.Type == DamageType.Scp207)
            {
                //Log.Info($"Conk hit {ev.Player.DisplayNickname}");
                ev.Amount *= (Plugin.cokeDPS / 100);
            }
            if (ev.DamageHandler.Type == DamageType.Scp3114)
            {
                ev.Amount = Plugin.Scp3114DMG;
            }
        }
    }
}
