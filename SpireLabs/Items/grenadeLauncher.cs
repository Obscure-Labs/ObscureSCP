using CustomItems.API;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.BasicMessages;
using MEC;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using System.Linq;
using Player = Exiled.Events.Handlers.Player;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunLogicer)]
    public class GrenadeLauncher : Exiled.CustomItems.API.Features.CustomWeapon
    {
        public override string Name { get; set; } = "Grenade Launcher";

        public override float Damage { get; set; } = 0f;

        public override uint Id { get; set; } = 3;

        public override string Description { get; set; } = "\t";

        public override float Weight { get; set; } = 1.25f;

        public override byte ClipSize { get; set; } = 1;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside096,
                }
            },
        };

        private Exiled.CustomItems.API.Features.CustomGrenade? _loadedCustomGrenade = null;
        private ProjectileType _loadedGrenade = ProjectileType.FragGrenade;

        protected override void SubscribeEvents()
        {
            Player.ChangedItem += OnChangedItem;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Grenade Launcher</b> \n <b>This is a single loaded grenade launcher that can fire Flashbangs and HE grenades</b>.", 3.0f);
        }

        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.Player.CurrentItem is Firearm firearm)
            {
                if (firearm.Ammo != ClipSize)
                {
                    ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Dryfire));
                    return;
                }
                else
                {
                    firearm.Ammo -= 1;
                }
            }

            Projectile projectile;
            if (_loadedCustomGrenade is not null)
            {
                _loadedCustomGrenade.Throw(ev.Player.Transform.position, 10f, 5f, 3f, ItemType.GrenadeHE, ev.Player);
            }
            else
            {

                projectile = _loadedGrenade switch
                {
                    ProjectileType.Scp018 => ev.Player.ThrowGrenade(ProjectileType.Scp018).Projectile,
                    ProjectileType.Flashbang => ev.Player.ThrowGrenade(ProjectileType.Flashbang).Projectile,
                    _ => ev.Player.ThrowGrenade(ProjectileType.FragGrenade).Projectile,
                };

                projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, projectile.Base);
            }
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.Player.CurrentItem is not Firearm firearm || firearm.Ammo >= ClipSize)
            {
                return;
            }

            foreach (var item in ev.Player.Items.ToList())
            {
                if (item.Type != ItemType.GrenadeHE && item.Type != ItemType.GrenadeFlash)
                {
                    continue;
                }

                if (TryGet(item, out Exiled.CustomItems.API.Features.CustomItem? customNade))
                {
                    continue;
                }

                ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));

                Timing.CallDelayed(1.5f, () => firearm.Ammo = ClipSize);

                if (item.Type is ItemType.GrenadeFlash)
                {
                    _loadedGrenade = ProjectileType.Flashbang;
                }
                else
                {
                    _loadedGrenade = item.Type is ItemType.GrenadeHE ? ProjectileType.FragGrenade : ProjectileType.FragGrenade;
                }

                ev.Player.RemoveItem(item);
                return;
            }
        }

    }
}
