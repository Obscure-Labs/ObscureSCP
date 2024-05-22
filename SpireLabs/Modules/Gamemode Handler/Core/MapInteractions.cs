using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Doors;
using MEC;
using ObscureLabs.API.Features;
using System.Collections.Generic;
using UnityEngine;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class MapInteractions : Module
    {
        public override string Name => "MapInteractions";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Timing.RunCoroutine(RandomFlickerCoroutine());
            Exiled.Events.Handlers.Warhead.Detonated += OnDetonated;

            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Warhead.Detonated -= OnDetonated;

            return base.Disable();
        }

        private void OnDetonated()
        {
            foreach (var door in Door.List)
            {
                if (door.Zone is ZoneType.Surface)
                {
                    door.IsOpen = true;
                    door.Lock(Mathf.Infinity, DoorLockType.Warhead);
                }
            }
        }

        private IEnumerator<float> RandomFlickerCoroutine()
        {
            while (!Round.IsEnded)
            {
                var roomFlicker = Room.Random(ZoneType.LightContainment);
                roomFlicker.TurnOffLights(0.15f);

                yield return Timing.WaitForSeconds(1);

                roomFlicker = Room.Random(ZoneType.HeavyContainment);
                roomFlicker.TurnOffLights(0.15f);
                roomFlicker = Room.Random(ZoneType.Entrance);
                roomFlicker.TurnOffLights(0.15f);

                Log.Debug($"Flickering lights in: {roomFlicker.RoomName}");

                var num = UnityEngine.Random.Range(5, 30);

                Log.Debug($"Waiting for {num} seconds till next flicker");
                yield return Timing.WaitForSeconds(num);
            }
        }
    }
}
