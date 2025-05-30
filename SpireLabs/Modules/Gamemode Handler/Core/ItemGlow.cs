using ObscureLabs.API.Features;
using System.Collections.Generic;
using UnityEngine;
using Pickup = Exiled.API.Features.Pickups.Pickup;
using Light = Exiled.API.Features.Toys.Light;
using MEC;
using Exiled.Events.EventArgs.Player;
using Exiled.CustomItems.API.Features;
using Projectile = Exiled.API.Features.Pickups.Projectiles.Projectile;
using Exiled.API.Features.Doors;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class ItemGlow : Module
    {
        public override string Name => "ItemGlow";

        public List<Pickup> GlowingPickups = new List<Pickup>();

        public override bool IsInitializeOnStart => true;

        public CoroutineHandle Routine;

        public override bool Enable()
        {
            Routine = new CoroutineHandle();

            Door.Get(Exiled.API.Enums.DoorType.LczArmory).IsOpen = true;

            Timing.CallDelayed(1f, () => Door.Get(Exiled.API.Enums.DoorType.LczArmory).IsOpen = false);
            Timing.CallDelayed(1f, () => Routine = Timing.RunCoroutine(GlowManager()));

            Exiled.Events.Handlers.Player.PickingUpItem += PickingUpItem;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.PickingUpItem -= PickingUpItem;
            Timing.KillCoroutines(Routine);
            return base.Disable();
        }

        public void PickingUpItem(PickingUpItemEventArgs ev)
        {
            
            if (GlowingPickups.Contains(ev.Pickup))
            {
                GlowingPickups.Remove(ev.Pickup);
            }
        }



        public void CreateLight(Pickup i, Color color)
        {
            Light light = Light.Create(i.GameObject.transform.position, new Vector3(90, 0, 0), Vector3.one, false, color);
            light.Intensity = 2f;
            light.Range = 0.5f;
            light.LightType = LightType.Point;
            light.ShadowType = LightShadows.Soft;

            light.Spawn();
            light.Base.gameObject.transform.parent = i.GameObject.transform;
        }

        public IEnumerator<float> GlowManager()
        {
            while (true)
            {
                yield return Timing.WaitForOneFrame;
                
                foreach (Pickup i in Pickup.List)
                {
                    if (i.Is<Projectile>(out _)) { continue; }

                    if (!GlowingPickups.Contains(i))
                    {

                        GlowingPickups.Add(i);
                        #pragma warning disable 
                        // ReSharper disable once UnusedVariable
                        if (CustomItem.TryGet(i, out var item))
                        {
                            CreateLight(i, Color.yellow);
                            continue;
                        }

                        switch (ItemRarityAPI.GetRarity(i.Type))
                        { 
                            case Rarity.None:
                                { 
                                    break;
                                }
                            case Rarity.Common:
                                {
                                    CreateLight(i, new Color(0.3f, 0.3f, 0.3f));
                                    break;
                                }
                            case Rarity.Uncommon:
                                {
                                    CreateLight(i, new Color(0, 0.4f, 0));
                                    break;
                                }
                            case Rarity.Rare:
                                {
                                    CreateLight(i, new Color(0, 0.65f, 1f));
                                    break;
                                }
                            case Rarity.Legendary:
                                {
                                    CreateLight(i, Color.magenta);
                                    break;
                                }
                            case Rarity.Obscure:
                                {
                                    CreateLight(i, Color.yellow);
                                    break;
                                }

                        }

                    }
                }
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
