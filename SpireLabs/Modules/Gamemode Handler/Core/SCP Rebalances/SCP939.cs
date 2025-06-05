using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp049;
using Exiled.Events.Patches.Events.Scp049;
using ObscureLabs.API.Features;

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
            if (ev.Attacker.Role.Type == PlayerRoles.RoleTypeId.Scp939) { ev.Amount = 60; }
        }
    }
}
