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
    internal class customRoles : Plugin.Module
    {

        public override string name { get; set; } = "customRoles";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Player.UsingItemCompleted += usingItem;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Player.UsingItemCompleted -= usingItem;
                base.Disable();
                return true;
            }
            catch { return false; }
        }




        internal class roleData
        {
            public Player player;
            public int? UCRID;
        }

        internal static List<roleData> rd = new List<roleData>();


        private void usingItem(UsingItemCompletedEventArgs ev)
        {
            if (ev.Item.Type == ItemType.SCP330)
            {
                if (UCRAPI.HasCustomRole(ev.Player))
                {
                    if (UCRAPI.Get(ev.Player).Id == 2)
                    {
                        ev.Player.EnableEffect(EffectType.MovementBoost);
                        ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 80, 10);
                    }
                }
            }
        }



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
