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
namespace SpireLabs.Items
{
    [CustomItem(ItemType.Painkillers)]
    public class esssentialOils : CustomItem
    {
        public override uint Id { get; set; } = 0;
        public override string Name { get; set; } = "Essential Oils";
        public override string Description { get; set; } = "The warning label says \"DO NOT CONSUME\" but they just look so good.";
        public override float Weight { get; set; } = 0.25f;
        private SpawnLocationType pos;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside049Armory,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside096
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.Inside330
                }
            },
        };

        protected override void SubscribeEvents()
        {
            Player.UsingItemCompleted += usingOils;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= usingOils;
            base.UnsubscribeEvents();
        }

        private void usingOils(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            ev.Player.EnableEffect(EffectType.BodyshotReduction, 15);
            ev.Player.ChangeEffectIntensity(EffectType.BodyshotReduction, 10, 15);
            ev.Player.EnableEffect(EffectType.DamageReduction,15);
            ev.Player.ChangeEffectIntensity(EffectType.DamageReduction, 10, 15);
            ev.Player.EnableEffect(EffectType.Concussed, 15);
            ev.Player.ChangeEffectIntensity(EffectType.Concussed, 1, 15);
            ev.Player.EnableEffect(EffectType.MovementBoost, 15);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 35, 15);
            ev.Player.EnableEffect(EffectType.Deafened, 15);
            ev.Player.ChangeEffectIntensity(EffectType.Deafened, 1, 15);
            ev.Player.EnableEffect(EffectType.Invigorated, 15);
            ev.Player.ChangeEffectIntensity(EffectType.Invigorated, 15, 15);
        }
    }
}
