using CustomItems.API;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using MEC;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using Player = Exiled.Events.Handlers.Player;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class ClusterHE : Exiled.CustomItems.API.Features.CustomGrenade
    {
        public override string Name { get; set; } = "Cluster Grenade";

        public override uint Id { get; set; } = 2;

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
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
                }
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;

        public override float FuseTime { get; set; } = 2.8f;

        protected override void SubscribeEvents()
        {
            Player.ChangedItem += OnChangedItem;
            Player.ThrownProjectile += OnThrownProjectile;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Player.ChangedItem -= OnChangedItem;
            Player.ThrownProjectile -= OnThrownProjectile;
            base.UnsubscribeEvents();
        }

        protected override void OnExploding(ExplodingGrenadeEventArgs ev)
        {
            Timing.RunCoroutine(OnExplodingGrenadeCoroutine(ev));
            base.OnExploding(ev);
        }

        private IEnumerator<float> OnExplodingGrenadeCoroutine(ExplodingGrenadeEventArgs ev)
        {
            var rnd = new System.Random();
            yield return Timing.WaitForSeconds(0.35f);
            for (int i = 0; i < 7; i++)
            {

                ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
                grenade.FuseTime = rnd.Next(0, 101) / 100f * 1.5f;
                grenade.ScpDamageMultiplier = 0.25f;
                grenade.SpawnActive(ev.Position, ev.Player);
                grenade.SpawnActive(ev.Position, ev.Player);
            }
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Cluster Grenade</b> \n <b>This will explode into multiple smaller grenades</b>.", 3.0f);
        }
    }
}