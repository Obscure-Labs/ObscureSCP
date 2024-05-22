using Exiled.API.Features;
using MEC;
using System.Collections.Generic;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class ChaosCounter
    {
        public static IEnumerator<float> ChaosUpdateCoroutine()
        {
            yield return Timing.WaitForSeconds(1f);

            while (!Round.IsEnded)
            {
                yield return Timing.WaitForSeconds(1f);

                var chaos = 0;

                foreach (var p in Player.List)
                {
                    if (p.IsCHI)
                    {
                        chaos++;
                    }
                }

                if (chaos != 0)
                {
                    Round.ChaosTargetCount = chaos;
                }

                yield return Timing.WaitForSeconds(2f);
            }
        }
    }
}
