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
using Player = Exiled.Events.Handlers.Player;
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

namespace SpireLabs.Items
{
    [CustomItem(ItemType.GunLogicer)]
    public class grenadeLauncher : Exiled.CustomItems.API.Features.CustomWeapon
    {


        private Exiled.CustomItems.API.Features.CustomGrenade? loadedCustomGrenade = null;
        private ProjectileType loadedGrenade = ProjectileType.FragGrenade;
        public override float Damage { get; set; } = 0f;
        public override string Name { get; set; } = "Grenade Launcher";
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
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside096,
                }
            },
        };

        protected override void SubscribeEvents()
        {
            Player.ChangedItem += Equipped;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        private void Equipped(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Grenade Launcher</b> \n <b>This is a single loaded grenade launcher that can fire Flashbangs and HE grenades</b>.", 3.0f);
        }

        protected override void OnShooting(ShootingEventArgs ev)
        {
            ev.IsAllowed = false;

            if (ev.Player.CurrentItem is Firearm firearm)
                if (firearm.Ammo != ClipSize) { ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Dryfire)); return; } else firearm.Ammo -= 1;
            Projectile projectile;
            if(loadedCustomGrenade != null)
            {
                loadedCustomGrenade.Throw(ev.Player.Transform.position, 10f, 5f, 3f, ItemType.GrenadeHE, ev.Player);
            }
            else
            {

                switch (loadedGrenade)
                {
                    case ProjectileType.Scp018:
                        projectile = ev.Player.ThrowGrenade(ProjectileType.Scp018).Projectile;
                        break;
                    case ProjectileType.Flashbang:
                        projectile = ev.Player.ThrowGrenade(ProjectileType.Flashbang).Projectile;
                        break;
                    default:
                        projectile = ev.Player.ThrowGrenade(ProjectileType.FragGrenade).Projectile;
                        break;
                }
                projectile.GameObject.AddComponent<CollisionHandler>().Init(ev.Player.GameObject, projectile.Base);
            }
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            ev.IsAllowed = false;
            if (!(ev.Player.CurrentItem is Firearm firearm) || firearm.Ammo >= ClipSize) return;
            foreach (Item item in ev.Player.Items.ToList())
            {
                if (item.Type != ItemType.GrenadeHE && item.Type != ItemType.GrenadeFlash) continue;
                if(TryGet(item, out Exiled.CustomItems.API.Features.CustomItem? customNade))
                {
                    continue;
                }

                ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));

                Timing.CallDelayed(1.5f, () => firearm.Ammo = ClipSize);

                loadedGrenade = item.Type == ItemType.GrenadeFlash ? ProjectileType.Flashbang :
                    item.Type == ItemType.GrenadeHE ? ProjectileType.FragGrenade : ProjectileType.FragGrenade;
                ev.Player.RemoveItem(item);
                return;
            }
        }

    } 
}
