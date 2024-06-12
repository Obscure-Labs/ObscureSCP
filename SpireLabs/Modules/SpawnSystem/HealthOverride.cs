using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using System.Collections.Generic;

namespace ObscureLabs.SpawnSystem
{
    internal class HealthOverride : Module
    {
        public override string Name => "HealthOverride";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Spawned += OverridePlayerHealth;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Spawned -= OverridePlayerHealth;
            return base.Disable();
        }

        public void OverridePlayerHealth(SpawnedEventArgs ev)
        {
            Timing.RunCoroutine(OverrideHealth(ev));
        }

        public static IEnumerator<float> OverrideHealth(SpawnedEventArgs ev)
        {
            yield return Timing.WaitForSeconds(Timing.WaitForOneFrame);

            if (!Plugin.Instance.overrideConfigs.HealthOverrides.TryGetValue(ev.Player.Role.Type, out var healthData))
            {
                yield break;
            }

            if (!healthData.IsEnabled)
            {
                yield break;
            }

            var humanPlayers = 0;

            foreach (var player in Player.List)
            {
                if (player.IsHuman)
                {
                    humanPlayers++;
                }
            }

            ev.Player.MaxHealth += healthData.Increase * humanPlayers;
            ev.Player.Heal(ev.Player.MaxHealth);
        }
    }
}
