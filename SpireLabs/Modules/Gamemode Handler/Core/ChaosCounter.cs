using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;


namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class ChaosCounter
    {
        public static IEnumerator<float> chaosUpdate()
        {
            yield return Timing.WaitForSeconds(1f);
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                int chaos = 0;
                foreach (Player p in Player.List)
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
