using CustomItems.API;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HarmonyLib.Code;

namespace SpireLabs.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class impactGrenades : Exiled.CustomItems.API.Features.CustomGrenade
    {
        public override string Name { get; set; } = "Impact Grenade";
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
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                }
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 50f;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

        }

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            base.OnThrownProjectile(ev);
            ev.Projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, ev.Projectile.Base);
        }
    }
}
