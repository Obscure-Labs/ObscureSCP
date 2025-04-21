using Exiled.API.Enums;
using MEC;
using ObscureLabs.API.Features;
using SpireSCP.GUI.API.Features;
using AttackingEventArgs = Exiled.Events.EventArgs.Scp106.AttackingEventArgs;
namespace ObscureLabs.Modules.Gamemode_Handler.Core.SCP_Rebalances
{
    internal class SCP106 : Module
    {
        public override string Name => "SCP106";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Scp106.Attacking += Attacking;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Scp106.Attacking -= Attacking;
            return base.Disable();
        }



        public void Attacking(AttackingEventArgs ev)
        {
            Timing.CallDelayed(0.15f, () => {
                ev.Target.EnableEffect(EffectType.PocketCorroding, 999, true);
                Manager.SendHint(ev.Target, $"You have been sent to the pocket dimension by <color=red>SCP 106</color>!", 5f);
                Manager.SendHint(ev.Player, $"You sent {ev.Target.DisplayNickname} to the pocket dimension!", 5f);
            });
        }
    }
}
