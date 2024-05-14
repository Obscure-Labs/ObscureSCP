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
    internal class MapInteractions : Plugin.Module
    {
        public override string name { get; set; } = "MapInteractions";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Warhead.Detonated += map_nuked;
                base.Init();
                return true;
            }
            catch (Exception ex) { return false; }
        }
        public override bool Disable()
        {
            try
            {
                Exiled.Events.Handlers.Warhead.Detonated += map_nuked;
                base.Init();
                return true;
            }
            catch (Exception ex) { return false; }
        }
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

        private IEnumerator<float> lockAnounce()
        {
            yield return Timing.WaitForSeconds(420);
            if (Manager.checkLoop()) { }
            else
            {
                Cassie.Message(@"jam_043_3 Surface armory has been opened for all jam_020_3 pitch_0.8 warhead pitch_1 authorized personnel . . . enter with pitch_0.9 jam_010_1 caution", false, false, true);
                foreach (Door d in Door.List)
                {
                    if (d.Zone == ZoneType.Surface)
                        d.Unlock();
                    switch (d.Type)
                    {
                        case DoorType.NukeSurface: d.Unlock(); break;
                        case DoorType.EscapePrimary: d.Unlock(); break;
                        case DoorType.EscapeSecondary: d.Unlock(); break;
                        case DoorType.ElevatorGateA: d.Unlock(); break;
                        case DoorType.ElevatorGateB: d.Unlock(); break;
                        case DoorType.SurfaceGate: d.Unlock(); break;
                    }

                }
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
