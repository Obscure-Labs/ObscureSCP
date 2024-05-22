using Exiled.API.Enums;
using Exiled.Events.EventArgs.Scp106;
using MEC;
using ObscureLabs.API.Features;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;

namespace ObscureLabs
{
    public class Larry : Module
    {
        public override string Name { get; } = "Larry";

        public override bool IsInitializeOnStart { get; } = true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Scp106.Attacking += OnAttacking;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Scp106.Attacking -= OnAttacking;

            return base.Disable();
        }

        private void OnAttacking(AttackingEventArgs ev)
        {
            Timing.RunCoroutine(HitCoroutine(ev));
        }

        private IEnumerator<float> HitCoroutine(AttackingEventArgs ev)
        {
            var player = ev.Player;
            var target = ev.Target;

            yield return Timing.WaitForSeconds(0.15f);
            target.EnableEffect(EffectType.PocketCorroding);
            target.ChangeEffectIntensity(EffectType.PocketCorroding, 1, 0);
            Manager.SendHint(player, $"You sent {target.Nickname} to the pocket dimension!", 5);
            Manager.SendHint(target, $"You were sent to the pocket dimension by: {player.Nickname} as SCP-106!", 5);
        }
    }
}
