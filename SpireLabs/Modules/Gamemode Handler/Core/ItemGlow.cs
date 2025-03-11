using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using GameCore;
using LabApi.Features.Wrappers;
using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using Light = Exiled.API.Features.Toys.Light;
using MEC;
using Exiled.Events.EventArgs.Player;
namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class ItemGlow : Module
    {
        public override string Name => "ItemGlow";

        public List<Pickup> GlowingPickups = new List<Pickup>();

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += RoundStart;
            Exiled.Events.Handlers.Player.PickingUpItem += PickingUpItem;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= RoundStart;
            Exiled.Events.Handlers.Player.PickingUpItem -= PickingUpItem;
            return base.Disable();
        }

        public void PickingUpItem(PickingUpItemEventArgs ev)
        {
            if (GlowingPickups.Contains(ev.Pickup))
            {
                GlowingPickups.Remove(ev.Pickup);
            }
        }

        public void RoundStart()
        {
            Timing.RunCoroutine(GlowManager());
        }

        public IEnumerator<float> GlowManager()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                foreach (Pickup i in Pickup.List)
                {
                    if (!GlowingPickups.Contains(i))
                    {
                        Light light = Light.Create(i.Position, new Vector3(90, 0, 0), Vector3.one, false, Color.red);
                        light.Intensity = 2f;
                        light.Range = 0.5f;
                        light.LightType = LightType.Point;
                        light.ShadowType = LightShadows.Soft;
                        light.Color = Color.magenta;
                        light.Spawn();
                        light.Base.gameObject.transform.SetParent(i.GameObject.transform);
                        GlowingPickups.Add(i);
                    }
                }
            }

        }
    }
}
