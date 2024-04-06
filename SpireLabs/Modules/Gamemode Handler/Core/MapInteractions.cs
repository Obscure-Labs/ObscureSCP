using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class MapInteractions
    {
        public static IEnumerator<float> randomFlicker()
        {
            while (true)
            {
                if (Manager.checkLoop()) break;
                var roomFlicker = Room.Random(ZoneType.LightContainment);
                roomFlicker.TurnOffLights(0.15f);
                yield return Timing.WaitForSeconds(1);
                roomFlicker = Room.Random(ZoneType.HeavyContainment);
                roomFlicker.TurnOffLights(0.15f);
                roomFlicker = Room.Random(ZoneType.Entrance);
                roomFlicker.TurnOffLights(0.15f);
                Log.Debug($"Flickering lights in: {roomFlicker.RoomName}");
                var rnd = new System.Random();
                int num = rnd.Next(5, 30);
                Log.Debug($"Waiting for {num} seconds till next flicker");
                yield return Timing.WaitForSeconds(num);
            }
        }

        public static void map_nuked()
        {
            foreach (Door d in Door.List)
            {
                if (d.Zone == ZoneType.Surface)
                {
                    d.IsOpen = true;
                    d.Lock(9999, DoorLockType.Warhead);
                }
            }

        }
    }
}
