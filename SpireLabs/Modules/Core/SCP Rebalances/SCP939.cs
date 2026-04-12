using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.EventArgs.Scp939;
using Exiled.Events.Patches.Events.Scp049;
using ObscureLabs.API.Features;
using PlayerRoles.PlayableScps.Scp939;

namespace ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances
{
    internal class Scp939 : Module
    {
        public override string Name => "SCP939";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Hurting += Attacking;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Hurting -= Attacking;
            return base.Disable();
        }

        private void Attacking(HurtingEventArgs ev)
        {
            if (ev == null) { return; }
            if (ev.Attacker == null) { return; }
            if (ev.Player == null) { return; }
            if (ev.DamageHandler.Base is Scp939DamageHandler handler && handler.Scp939DamageType == Scp939DamageType.LungeTarget) { ev.Amount = 100; return; }
            if (ev.Attacker.Role.Type == PlayerRoles.RoleTypeId.Scp939) { ev.Amount = 60; }
            ev = null;
        }

    }
}
