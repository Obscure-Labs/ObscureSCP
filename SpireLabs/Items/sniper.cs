using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.DamageHandlers;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using MEC;
using PlayerRoles;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace SpireLabs.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class sniper : CustomWeapon
    {
        public override string Name { get; set; } = "MTF-E14-HSR";
        public override string Description { get; set; } = "A long ranger rifle chambered in 44BMG (44cal) Bullets.";
        public override SpawnProperties? SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
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

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            ev.IsAllowed = false;
            int cal44 = ev.Player.GetAmmo(AmmoType.Ammo44Cal);
            if(cal44 != 0 && ev.Firearm.Ammo == 0 && ev.Firearm.Ammo != 1)
            {
                ev.Player.SetAmmo(AmmoType.Ammo44Cal, (ushort)(cal44 - 1));
                ev.Firearm.Ammo = 1;
                ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Reload));
            }
        }

        protected override void SubscribeEvents()
        {
            //Player.Hurting += hurt;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            //Player.Hurting -= hurt;
            base.UnsubscribeEvents();
        }

        //private void hurt(HurtingEventArgs ev)
        //{
        //    if (!Check(ev.Attacker.CurrentItem))
        //    {
        //        Log.Info($"Item {ev.Attacker.CurrentItem.ToString()} was deemed to be NOT a custom item");
        //        return;
        //    }
        //    Log.Info($"Item {ev.Attacker.CurrentItem.ToString()} was deemed to be a custom item");
        //    if (ev.Player.Role == RoleTypeId.Scp173|| ev.Player.Role == RoleTypeId.Scp049 || ev.Player.Role == RoleTypeId.Scp0492 || ev.Player.Role == RoleTypeId.Scp096 || ev.Player.Role == RoleTypeId.Scp106)
        //    {
        //        ev.Player.Hurt(ev.Attacker, 200f, DamageType.Revolver, null);
        //        Log.Warn("should have taken 400 damage");
        //    }
        //}
    }
}
