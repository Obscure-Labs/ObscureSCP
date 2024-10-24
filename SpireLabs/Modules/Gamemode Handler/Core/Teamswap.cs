using CommandSystem.Commands.RemoteAdmin.ServerEvent;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features.Roles;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class Teamswap : Module
    {

        public static Role[] RoleList { get; } =
    {
            
        };

        public override string Name => "Teamswap";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {

            Exiled.Events.Handlers.Player.Escaping += EscapingTeamSwap;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Escaping -= EscapingTeamSwap;
            return base.Disable();
        }


        public void EscapingTeamSwap(EscapingEventArgs ev)
        {
           if (ev.EscapeScenario is EscapeScenario.CustomEscape)
            {
                ev.NewRole = ev.Player.Role.Type switch
                {
                    RoleTypeId.FacilityGuard or RoleTypeId.NtfPrivate or RoleTypeId.NtfSergeant or RoleTypeId.NtfCaptain or RoleTypeId.NtfSpecialist => RoleTypeId.ChaosConscript,
                    RoleTypeId.ChaosConscript or RoleTypeId.ChaosMarauder or RoleTypeId.ChaosRepressor or RoleTypeId.ChaosRifleman => RoleTypeId.ChaosConscript,
                    _ => RoleTypeId.None,
                };

                ev.IsAllowed = ev.NewRole is not RoleTypeId.None;
            }
        }
    }
}
