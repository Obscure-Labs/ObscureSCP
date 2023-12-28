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

namespace SpireLabs.Items
{
    [CustomItem(ItemType.GunE11SR)]
    public class ER16 : Exiled.CustomItems.API.Features.CustomWeapon
    {

        private Exiled.CustomItems.API.Features.CustomGrenade? loadedCustomGrenade = null;
        private ProjectileType loadedGrenade = ProjectileType.FragGrenade;
        public override float Damage { get; set; } = 0f;
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
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
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

        public static int firedTimesTotal = 0;
        public static int firedTimes = 0;
        protected override void OnShooting(ShootingEventArgs ev)
        {
            if (ev.Player.CurrentItem is Firearm firearm)
            {

                ev.IsAllowed = false;

                if (ev.IsAllowed == false) { ev.Player.Connection.Send(new RequestMessage(ev.Firearm.Serial, RequestType.Dryfire)); }
                else
                {

                }

            }
        }
    }
}

