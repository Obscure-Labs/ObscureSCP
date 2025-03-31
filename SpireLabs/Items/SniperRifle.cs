using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HarmonyLib.Code;
using static InventorySystem.Items.ThrowableProjectiles.ThrowableNetworkHandler;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class Sniper : CustomWeapon
    {
        public override string Name { get; set; } = "MTF-E14-HSR";

        public override string Description { get; set; } = "\t";

        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                }
            },
        };
        public override uint Id { get; set; } = 1;

        public override float Damage { get; set; } = 200f;

        public override float Weight { get; set; } = 2f;

        public override byte ClipSize { get; set; } = 1;

        public override AttachmentName[] Attachments { get; set; } = new[]
        {
            AttachmentName.ExtendedBarrel,
            AttachmentName.Laser,
            AttachmentName.LowcapMagFMJ,
            AttachmentName.SoundSuppressor,
            AttachmentName.ScopeSight,
            AttachmentName.StandardStock,
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem += OnChangedItem;
            Exiled.Events.Handlers.Player.Shot += OnShot;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem -= OnChangedItem;
            Exiled.Events.Handlers.Player.Shot -= OnShot;

            base.UnsubscribeEvents();
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                return;
            }
            ev.Firearm.PrimaryMagazine.AmmoType = AmmoType.Ammo44Cal;
            ev.Firearm.PrimaryMagazine.MaxAmmo = 1;
            Log.Info("Reload was triggered");
            base.OnReloading(ev);

        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            Manager.SendHint(ev.Player, "You equipped the <b>MTF-E14-HSR</b> \nThis is a long range rifle chambered in .44 magnum rounds \nand can only hold 1 round in the magazine at any given time.", 3);
            
        }

        private void OnShot(ShotEventArgs ev)
        {
            ev.Firearm.PrimaryMagazine.AmmoType = AmmoType.Ammo44Cal;
            if (ev.Player == null || ev.Player.CurrentItem == null)
                return;

            if (!Check(ev.Player.CurrentItem))
            {
                return;
            }
            ev.CanHurt = false;
            
            if (ev.Target == null) { return; }
            if (ev.Target.IsHuman)
            {
                Log.Info($"hitbox was: {ev.Hitbox.HitboxType.ToString()}");
                if (ev.Hitbox.HitboxType == HitboxType.Headshot)
                {
                    ev.Target.Hurt(ev.Player, 150f, DamageType.Revolver, null);
                    ev.Player.ShowHitMarker(4f);
                }
                else
                {
                    ev.Target.Hurt(ev.Player, 75f, DamageType.Revolver, null);
                    ev.Player.ShowHitMarker();
                }
            }
            else
            {
                Log.Info($"hitbox was: {ev.Hitbox.HitboxType.ToString()}");
                Log.Info("Player was not human");
                ev.Player.ShowHitMarker();
                ev.Target.Hurt(ev.Player, 200f, DamageType.Revolver, null);
            }
        }
    }
}
