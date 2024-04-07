using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using UnityEngine;
using SpireSCP.GUI.API.Features;
using CustomItems.API;
using Exiled.API.Features.Pickups.Projectiles;
using InventorySystem.Items.ThrowableProjectiles;
using Exiled.Events.EventArgs.Map;
using System.Linq;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Attachments.Components;
using Exiled.API.Structs;
using Utf8Json.Resolvers.Internal;
using JetBrains.Annotations;
using System.Reflection;
using Exiled.API.Features.Components;
using PluginAPI.Events;
using InventorySystem.Items.Firearms;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.Rendering;
using System;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.ParticleDisruptor)]
    public class ParticleCollapser : Exiled.CustomItems.API.Features.CustomWeapon
    {

        public override float Damage { get; set; } = 0f;
        public override string Name { get; set; } = "Particle Collapser";
        public override uint Id { get; set; } = 6;
        public override string Description { get; set; } = "\t";
        public override float Weight { get; set; } = 1.25f;
        public override byte ClipSize { get; set; } = 3;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside096,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHid,
                },
            },
        };

        protected override void SubscribeEvents()
        {

            Exiled.Events.Handlers.Player.ChangedItem += Equipped;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        private void Equipped(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Particle Collapser</b> \n The lable on the stock reads: \n <b><color=red>WARNING EMITS VERY BRIGHT LIGHTS - WEAR EYE PROTECTION</b>.", 3.0f);
        }

        protected override void OnShot(ShotEventArgs ev)
        {
            base.OnShot(ev);
            if (ev.Player.CurrentItem is Exiled.API.Features.Items.Firearm firearm)
            {
                ev.CanHurt = false;

                Timing.RunCoroutine(energyBurst(ev));

            }


        }
        public Color[] colors = {
            Color.red, //this is red you fucking twat
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.yellow,
            };
        private IEnumerator<float> energyBurst(ShotEventArgs ev)
        {
            var rnd = new System.Random();
            Color color = colors[rnd.Next(0, colors.Count())];
            yield return Timing.WaitForOneFrame;
            var target = ev.Position;
            var light = Exiled.API.Features.Toys.Light.Create(target, null, Vector3.one, true, color);
            light.Intensity = 1000000;
            light.Range = 25;
            light.ShadowEmission = true;
            light.Spawn();
            Room room = Room.Get(target);
            room.TurnOffLights(5f);
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0.001f;
            grenade.ScpDamageMultiplier = 0f;
            grenade.DeafenDuration = 15;
            grenade.ConcussDuration = 25;
            ExplosionGrenadeProjectile g = grenade.SpawnActive(target, ev.Player);

            Timing.WaitForSeconds(0.05f);
            #region Raycasting
            foreach (Player pp in Player.List)
            {
                
                int loopCntr = 0;
                RaycastHit h;
                Player ppp;
                do
                {
                    Vector3 dir = pp.Position - new Vector3(g.Position.x, g.Position.y, g.Position.z);
                    Physics.Raycast(g.Position, dir, out h);
                    loopCntr++;
                } while (!Player.TryGet(h.collider, out ppp) && loopCntr != 5);
                if (ppp == null) continue;
                if (Math.Sqrt((Math.Pow((g.Position.x - ppp.Position.x), 2)) + (Math.Pow((g.Position.y - ppp.Position.y), 2))) > 3.5f) continue;
                if (ppp.Role.Team != ev.Player.Role.Team)
                {
                    ppp.Hurt(250, DamageType.Explosion);
                    ppp.EnableEffect(EffectType.Burned, 30, true);
                }
            }
            #endregion

            for(int i = 0; i < 250; i++)
            {
                yield return Timing.WaitForSeconds(0.005f);
                light.Range -= 0.10f;
                light.Intensity -= 4000f;
            }
            light.Destroy();
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;
        }
    }
}

