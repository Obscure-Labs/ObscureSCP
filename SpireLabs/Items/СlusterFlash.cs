using Exiled.CustomItems.API;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using UnityEngine;
using Player = Exiled.Events.Handlers.Player;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GrenadeFlash)]
    public class СlusterFlash : Exiled.CustomItems.API.Features.CustomGrenade
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
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
                }
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;

        public override float FuseTime { get; set; } = 3.8f;

        protected override void SubscribeEvents()
        {
            Player.ChangedItem += OnChangedItem;
            Player.ThrownProjectile += OnNewThrownProjectile;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Player.ChangedItem -= OnChangedItem;
            Player.ThrownProjectile -= OnNewThrownProjectile;
            base.UnsubscribeEvents();
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            base.OnExploding(ev);
            Timing.RunCoroutine(OnExplodingGrenadeCoroutine(ev));
        }

        private void OnNewThrownProjectile(ThrownProjectileEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                return;
            }
            else
            {
                FlashGrenade origin = ev.Item as FlashGrenade;
                origin.FuseTime = 0.5f;
            }
        }

        private IEnumerator<float> OnExplodingGrenadeCoroutine(ExplodingGrenadeEventArgs ev)
        {
            yield return Timing.WaitForOneFrame;

            for (int i = 0; i < 4; i++)
            {
                yield return Timing.WaitForOneFrame;
                FlashGrenade grenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                grenade.FuseTime = 0.1f;
                grenade.SpawnActive(new Vector3(ev.Position.x + Random.Range(-1, 1), ev.Position.y + Random.Range(-1, 1), ev.Position.z + Random.Range(-1, 1)), ev.Player);
            }
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item))
            {
                return;
            }

            Manager.SendHint(ev.Player, "You equipped the <b>Cluster Flashbang</b> \n <b>This will explode into multiple smaller Flashbang</b>.", 3.0f);
        }
    }
}