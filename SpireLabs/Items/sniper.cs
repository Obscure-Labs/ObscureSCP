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
using System.Collections.Generic;
using Player = Exiled.Events.Handlers.Player;

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
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
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
            Player.Hurting += OnNewHurting;
            Player.ReloadingWeapon += OnReloadinWeapon;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.ChangedItem -= OnChangedItem;
            Player.Hurting -= OnNewHurting;

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

        private void OnNewHurting(HurtingEventArgs ev)
        {
            if (ev.Attacker == null || ev.Attacker.CurrentItem == null)
            {
                return;
            }

            if (!Check(ev.Attacker.CurrentItem))
            {
                Log.Debug($"Item {ev.Attacker.CurrentItem} was deemed to be NOT a custom item");
                return;
            }

            Log.Debug($"Item {ev.Attacker.CurrentItem} was deemed to be a custom item");
            if (ev.Player.Role.Type is RoleTypeId.Scp173 or RoleTypeId.Scp049 or RoleTypeId.Scp0492 or RoleTypeId.Scp096 or RoleTypeId.Scp106)
            {
                ev.Player.Hurt(ev.Attacker, 200f, DamageType.Revolver, null);
                Log.Debug("should have taken 400 damage");
            }
        }
    }
}
