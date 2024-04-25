using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using MEC;
using UnityEngine;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class antiFall
    {
        public static IEnumerator<float> antiFallRoutine(Player p)
        {
            float timeFalling = 0;
            Vector3 lastNonFallingPosition = new Vector3(29.746f, 1f, 90.012f);
            //lastNonFallingPosition = p.Position;
            yield return Timing.WaitForOneFrame;
            while (true)
            {
                yield return Timing.WaitForSeconds(0.000001f);
                if (timeFalling == 0)
                {
                    //lastNonFallingPosition = p.Position;
                }
                if (p.IsAlive && p.Velocity.magnitude > 500)
                {
                    Log.Info("playe ris falling");
                    timeFalling += Time.deltaTime;
                    if (timeFalling > 10f)
                    {
                        p.Teleport(lastNonFallingPosition);
                        //p.fall
                        timeFalling = 0;
                    }
                }
                else
                {
                    timeFalling = 0;
                }
            }
        }
    }
}
