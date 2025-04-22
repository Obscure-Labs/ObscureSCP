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
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
        };

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            base.OnThrownProjectile(ev);
            ev.Throwable.Destroy();
            ev.Player.ThrowGrenade(ProjectileType.Scp018, true);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.FragGrenade, false);
            ev.Player.ThrowGrenade(ProjectileType.Scp018, false);
            ev.Player.ThrowGrenade(ProjectileType.Scp018, false);
            ev.Projectile.Destroy();

        }
    }
}