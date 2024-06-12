using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using ObscureLabs.API.Features;
using PlayerRoles;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class DamageModifiers : Module
    {
        public override string Name => "DamageModifiers";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Hurting += SetDamageModifiers;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Hurting -= SetDamageModifiers;

            return base.Disable();
        }

        private void SetDamageModifiers(HurtingEventArgs ev)
        {
            if (ev.DamageHandler.Type is DamageType.MicroHid)
            {
                ev.Amount *= Plugin.Instance.Config.HidDPS / 100;
            }
            if (ev.DamageHandler.Type is DamageType.Scp3114 && Plugin.Instance.Config.RolesDamage.TryGetValue(RoleTypeId.Scp3114, out var value))
            {
                ev.Amount = value;
            }
        }
    }
}
