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

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class clusterFlash : Exiled.CustomItems.API.Features.CustomGrenade
    {
        public override string Name { get; set; } = "Cluster Flashbang";
        public override uint Id { get; set; } = 7;
        public override string Description { get; set; } = "\t";
        public override float Weight { get; set; } = 0.25f;
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
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
                }
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 3.8f;

        protected override void SubscribeEvents()
        {
            Player.ChangedItem += changedToItem;
            Player.ThrownProjectile += Thrown;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
                Player.ChangedItem -= changedToItem;
                Player.ThrownProjectile -= Thrown;
                base.UnsubscribeEvents();
            
        }

        private void Thrown(ThrownProjectileEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            else
            {
                FlashGrenade origin = ev.Item as FlashGrenade;
                origin.FuseTime = 1.3f;
            }

        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            base.OnExploding(ev);
            Timing.RunCoroutine(boomb(ev));
        }

        private IEnumerator<float> boomb(ExplodingGrenadeEventArgs ev)
        {
            var rnd = new System.Random();
            yield return Timing.WaitForOneFrame;
            for (int i = 0; i < 15; i++)
            {
                FlashGrenade grenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                grenade.FuseTime = 0.001f;
                grenade.SpawnActive(ev.Position, ev.Player);
            }
        }

        private void changedToItem(ChangedItemEventArgs ev)
        {
            if(!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Cluster Flashbang</b> \n <b>This will explode into multiple smaller Flashbang</b>.", 3.0f);
        }
    }
}