using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using PlayerRoles;
using PlayerStatsSystem;
using SpireSCP.GUI.API.Features;
using Subtitles;
using System.Collections.Generic;
using Player = Exiled.Events.Handlers.Player;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class Sniper : CustomWeapon
    {
        public override string Name { get; set; } = "MTF-E14-HSR";

        public override string Description { get; set; } = "\t";

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 0,
#pragma warning disable CS0618
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
#pragma warning restore CS0618
                },
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
            Player.ChangedItem += OnChangedItem;
            Player.ReloadingWeapon += OnReloadinWeapon;
            Player.Shot += OnShot;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.ChangedItem -= OnChangedItem;
            Player.ReloadingWeapon -= OnReloadinWeapon;
            Player.Shot -= OnShot;

            base.UnsubscribeEvents();
        }

        private void OnReloadinWeapon(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                return;
            }

            ev.IsAllowed = false;

            var cal44 = ev.Player.GetAmmo(AmmoType.Ammo44Cal);

            if (cal44 != 0 && ev.Firearm.Ammo == 0 && ev.Firearm.Ammo != 1)
            {
                ev.Player.SetAmmo(AmmoType.Ammo44Cal, (ushort)(cal44 - 1));
                ev.Firearm.Ammo = 1;
                ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));
            }
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            Manager.SendHint(ev.Player, "You equipped the <b>MTF-E14-HSR</b> \nThis is a long range rifle chambered in .44 magnum rounds \nand can only hold 1 round in the magazine at any given time.", 3);
        }

#pragma warning disable CS0114
        private void OnShot(ShotEventArgs ev)
#pragma warning restore CS0114
        {
            if (ev.Player == null || ev.Player.CurrentItem == null)
                return;

            if(!Check(ev.Player.CurrentItem))
            {
                return;
            }
            ev.CanHurt = false;
            if (ev.Target.IsHuman)
            {
                Log.Info($"hitbox was: {ev.Hitbox.HitboxType.ToString()}");
                if(ev.Hitbox.HitboxType == HitboxType.Headshot)
                {
                    ev.Target.Hurt(attacker: ev.Player, damage: 75f);
                    ev.Player.ShowHitMarker(7f);
                }
                else
                {
                    ev.Target.Hurt(attacker: ev.Player, damage: 150f);
                    ev.Player.ShowHitMarker();
                }
            }
            else
            {
                Log.Info($"hitbox was: {ev.Hitbox.HitboxType.ToString()}");
                Log.Info("Player was not human");
                ev.Player.ShowHitMarker();
                ev.Target.Hurt(attacker: ev.Player, damage: 200f);
            }
        }
    }
}
