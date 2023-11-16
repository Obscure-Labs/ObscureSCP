using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Item;
using Exiled.Events.EventArgs.Player;

namespace SpireLabs.SpawnSystem
{
    internal static class HealthOverride
    {
        internal static IEnumerator<float> OverrideHealth(SpawnedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(0.25f);
            if (Plugin.OCaptain.enabled)
            {
                if (ev.Player.RoleManager.CurrentRole.RoleTypeId == RoleTypeId.NtfCaptain)
                {
                    ev.Player.MaxHealth = Plugin.OCaptain.healthOverride;
                    ev.Player.Heal(Plugin.OCaptain.healthOverride, false);
                }
            }
            Player p = ev.Player;
            int humanPlayers = 0;
            foreach (Player rP in Player.List)
            {
                if (rP.IsHuman)
                {
                    humanPlayers++;
                }
            }
            switch (p.RoleManager.CurrentRole.RoleTypeId)
            {
                case RoleTypeId.Scp049:
                    if (Plugin.OScp049.enabled)
                    {
                        p.MaxHealth = Plugin.OScp049.healthOverride;
                    }
                    Task.Delay(50);
                    if (Plugin.Scp049.enabled)
                    {
                        p.MaxHealth += (Plugin.Scp049.healthIncrease * humanPlayers);
                        p.Heal(p.MaxHealth);
                    }
                    break;
                case RoleTypeId.Scp079:
                    if (Plugin.OScp079.enabled)
                    {
                        p.MaxHealth = Plugin.OScp079.healthOverride;
                    }
                    Task.Delay(50);
                    if (Plugin.Scp079.enabled)
                    {
                        p.MaxHealth += (Plugin.Scp079.healthIncrease * humanPlayers);
                        p.Heal(p.MaxHealth);
                    }
                    break;
                case RoleTypeId.Scp096:
                    if (Plugin.OScp096.enabled)
                    {
                        p.MaxHealth = Plugin.OScp096.healthOverride;
                    }
                    Task.Delay(50);
                    if (Plugin.Scp096.enabled)
                    {
                        p.MaxHealth += (Plugin.Scp096.healthIncrease * humanPlayers);
                        p.Heal(p.MaxHealth);
                    }
                    break;
                case RoleTypeId.Scp106:
                    if (Plugin.OScp106.enabled)
                    {
                        p.MaxHealth = Plugin.OScp106.healthOverride;
                    }
                    Task.Delay(50);
                    if (Plugin.Scp106.enabled)
                    {
                        p.MaxHealth += (Plugin.Scp106.healthIncrease * humanPlayers);
                        p.Heal(p.MaxHealth);
                    }
                    break;
                case RoleTypeId.Scp173:
                    if (Plugin.OScp173.enabled)
                    {
                        p.MaxHealth = Plugin.OScp173.healthOverride;
                    }
                    Task.Delay(50);
                    if (Plugin.Scp173.enabled)
                    {
                        p.MaxHealth += (Plugin.Scp173.healthIncrease * humanPlayers);
                        p.Heal(p.MaxHealth);
                    }
                    break;
                case RoleTypeId.Scp939:
                    if (Plugin.OScp939.enabled)
                    {
                        p.MaxHealth = Plugin.OScp939.healthOverride;
                    }
                    Task.Delay(50);
                    if (Plugin.Scp939.enabled)
                    {
                        p.MaxHealth += (Plugin.Scp939.healthIncrease * humanPlayers);
                        p.Heal(p.MaxHealth);
                    }
                    break;
                case RoleTypeId.Scp3114:
                    if (Plugin.OScp3114.enabled)
                    {
                        p.MaxHealth = Plugin.OScp3114.healthOverride;
                    }
                    Task.Delay(50);
                    if (Plugin.Scp939.enabled)
                    {
                        p.MaxHealth += (Plugin.Scp3114.healthIncrease * humanPlayers);
                        p.Heal(p.MaxHealth);
                    }
                    break;
            }
        }
    }
}
