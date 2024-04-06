using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using JetBrains.Annotations;
using MEC;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firearm = Exiled.API.Features.Items.Firearm;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class ER16 : Exiled.CustomItems.API.Features.CustomWeapon
    {

        public override float Damage { get; set; } = 40f;
        public override string Name { get; set; } = "MTF-ER16-SR";
        public override uint Id { get; set; } = 5;
        public override float Weight { get; set; } = 1.25f;
        public override string Description { get; set; } = "\t";
        public override byte ClipSize { get; set; } = 30;

        public override AttachmentName[] Attachments { get; set; } = new[]
{
            AttachmentName.ExtendedBarrel,
            AttachmentName.Laser,
            AttachmentName.LowcapMagJHP,
            AttachmentName.HoloSight,
            AttachmentName.StandardStock,
        };

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
            },
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangedItem += Equipped;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
        }

        private void Equipped(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>ER16</b> \n <b>This is a burst fire rifle chambered in 5.56x45 ammunition</b>.", 3.0f);
        }

        public static int trueAmmo = 30;
        public int shotsFired = 0;
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Player.CurrentItem is Firearm firearm)
            {
                if (trueAmmo <= 1) { ev.Firearm.Ammo = 0; }
                else
                {
                    if (shotsFired == 3) { ev.Firearm.Ammo = 0; shotsFired = 0; }
                    else
                    {
                        ev.Firearm.Ammo = (byte)trueAmmo;
                        trueAmmo--;
                        shotsFired++;
                    }
                }
            }
        }

        private IEnumerator<float> Burst(ShootingEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;
        }

        protected override void OnReloading(ReloadingWeaponEventArgs ev)
        {
            int ammo556 = ev.Player.GetAmmo(AmmoType.Nato556);
            if (ammo556 < 30) { trueAmmo = ammo556; }
            else { trueAmmo = 30; }
            shotsFired = 0;
        }
    }
}

