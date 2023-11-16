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
namespace SpireLabs.Items
{
    [CustomItem(ItemType.Painkillers)]
    public class esssentialOils : CustomItem
    {
        public override uint Id { get; set; } = 0;
        public override string Name { get; set; } = "Essential Oils";
        public override string Description { get; set; } = "\t";
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
            Player.ChangedItem += changedToItem;
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            Player.UsingItemCompleted -= usingOils;
            Player.ChangedItem -= changedToItem;
            base.UnsubscribeEvents();
        }

        private void changedToItem(ChangedItemEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            Manager.SendHint(ev.Player, "You equipped the <b>Essential Oils</b> \nThe label reads: <b>DO NOT EAT - TOXIC TO HUMANS</b>.", 3.0f);
        }


        private void usingOils(UsingItemCompletedEventArgs ev)
        {
            if (!Check(ev.Item))
                return;
            Manager.SendHint(ev.Player, "You feel a little odd... maybe you should have read the label", 5.0f);
            ev.Player.EnableEffect(EffectType.BodyshotReduction, 15);
            ev.Player.ChangeEffectIntensity(EffectType.BodyshotReduction, 50, 15);
            ev.Player.EnableEffect(EffectType.DamageReduction,15);
            ev.Player.ChangeEffectIntensity(EffectType.DamageReduction, 10, 15);
            ev.Player.EnableEffect(EffectType.Concussed, 15);
            ev.Player.ChangeEffectIntensity(EffectType.Concussed, 1, 15);
            ev.Player.EnableEffect(EffectType.MovementBoost, 15);
            ev.Player.ChangeEffectIntensity(EffectType.MovementBoost, 55, 15);
            ev.Player.EnableEffect(EffectType.Deafened, 15);
            ev.Player.ChangeEffectIntensity(EffectType.Deafened, 1, 15);
            ev.Player.EnableEffect(EffectType.Invigorated, 15);
            ev.Player.ChangeEffectIntensity(EffectType.Invigorated, 15, 15);
        }
    }
}
