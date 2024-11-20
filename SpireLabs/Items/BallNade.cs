using Exiled.API.Features.Attributes;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.Events.Handlers;
using InventorySystem.Items.ThrowableProjectiles;
using Item = Exiled.API.Features.Items.Item;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class BallNade : CustomGrenade
    {
        public override string Name { get; set; } = "BallNade";

        public override uint Id { get; set; } = 9;

        public override string Description { get; set; } = "\t";

        public override float Weight { get; set; } = 0.25f;

        public override bool ExplodeOnCollision { get; set; } = false;

        public override float FuseTime { get; set; } = 2.8f;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 0,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
#pragma warning disable CS0618
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
#pragma warning restore CS0618
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
                }
            },
        };

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Player.ThrowGrenade(ProjectileType.Scp018);
            ev.Projectile.Destroy();
            base.OnThrownProjectile(ev);
        }
    }
}