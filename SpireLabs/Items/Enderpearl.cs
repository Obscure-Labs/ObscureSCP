using Exiled.API;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.Handlers;
using MEC;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.SCP2176)]
    public class Enderpearl : Exiled.CustomItems.API.Features.CustomGrenade
    {
        public override string Name { get; set; } = "EnderPearl";

        public override uint Id { get; set; } = 15;

        public override string Description { get; set; } = "\t";

        public override float Weight { get; set; } = 0.25f;

        public override float FuseTime { get; set; } = 99;

        public override bool ExplodeOnCollision { get; set; } = false;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
#pragma warning disable CS0618
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
#pragma warning restore CS0618
                },
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
                }
            }
        };



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
            yield return Timing.WaitForSeconds(0.1f);

            if (ev.Projectile.PreviousOwner != null)
            {
                ev.Player.Transform.position = ev.Projectile.Transform.position;
                ev.Projectile.Destroy();
            }
            else
            {
                ev.Projectile.Destroy();
            }
        }

        private void OnChangedItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item)) return;
            Manager.SendHint(ev.Player, "You equipped the <b>Ender Pearl</b> \n <b>You know how this works...</b>.", 3.0f);
        }
    }
}
