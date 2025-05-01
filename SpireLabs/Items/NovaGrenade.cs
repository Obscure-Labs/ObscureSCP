
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using MEC;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using System.Linq;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using UnityEngine;


namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class NovaGrenade : Exiled.CustomItems.API.Features.CustomGrenade
    {
        public override string Name { get; set; } = "Nova Grenade";

        public override uint Id { get; set; } = 4;

        public override string Description { get; set; } = "\t";

        public override float Weight { get; set; } = 0.01f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                }
            },
        };

        public override bool ExplodeOnCollision { get; set; } = false;

        public override float FuseTime { get; set; } = 999f;

        public Color[] colors = {
            Color.red, //this is red you fucking twat
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.yellow,
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
            base.UnsubscribeEvents();
        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            ExplosionGrenadeProjectile g = ev.Projectile as ExplosionGrenadeProjectile;
            g.MaxRadius = 15f;
            g.ScpDamageMultiplier = 2f;
            Timing.RunCoroutine(GrenadeLightCoroutine(ev));
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Nova Grenade</b> \n <b>This will implode with a shining bright light.</b>.", 3.0f);
            ev.Player.Emotion = EmotionPresetType.Chad;
        }

        private IEnumerator<float> GrenadeLightCoroutine(ThrownProjectileEventArgs ev)
        {
            ExplosionGrenadeProjectile g = ev.Projectile as ExplosionGrenadeProjectile;

            yield return Timing.WaitForSeconds(0.65f);

            var color = colors[UnityEngine.Random.Range(0, colors.Count())];

            yield return Timing.WaitForOneFrame;

            var target = ev.Projectile.Position;

            g.Destroy();

            var light = Exiled.API.Features.Toys.Light.Create(target, null, Vector3.one, true, color);
            light.Intensity = 0;
            light.Range = 0;
            light.MovementSmoothing = 60;
            light.Spawn();

            var room = Room.Get(target);
            room.TurnOffLights(9999f);

            for (var i = 0; i < 20; i++)
            {
                yield return Timing.WaitForOneFrame;
                light.Range += 6.5f;
                light.Intensity += 400f;
            }

            yield return Timing.WaitForSeconds(0.5f);

            for (var i = 0; i < 20; i++)
            {
                yield return Timing.WaitForOneFrame;
                light.Range -= 6.5f;
                light.Intensity -= 400f;
            }

            var grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0;
            grenade.ScpDamageMultiplier = 1.25f;
            grenade.MaxRadius = 50;

            grenade.SpawnActive(new Vector3(target.x + 0.5f, target.y, target.z));
            grenade.SpawnActive(new Vector3(target.x + 0.25f, target.y, target.z + 0.5f));
            grenade.SpawnActive(new Vector3(target.x + 0.25f, target.y, target.z - 0.5f));

            room.TurnOffLights(0.5f);
            light.Destroy();
        }
    }
}
