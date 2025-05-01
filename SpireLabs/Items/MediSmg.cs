using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Attachments;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using Light = Exiled.API.Features.Toys.Light;
using UnityEngine;
using Player = Exiled.API.Features.Player;
using Server = Exiled.API.Features.Server;
using Item = Exiled.API.Features.Items.Item;
using Door = Exiled.API.Features.Doors.Door;
using Exiled.API.Features.Toys;
using SpireSCP.GUI.API.Features;
using InventorySystem.Items.Firearms.Modules.Scp127;
using Mirror;
using ObscureLabs.Modules.Gamemode_Handler.Core;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunSCP127)]
    public class MediSmg : Exiled.CustomItems.API.Features.CustomWeapon
    {
        public static int trueAmmo = 30;

        public override float Damage { get; set; } = 0f;

        public override string Name { get; set; } = "MediGun (SMG)";

        public override uint Id { get; set; } = 14;

        public override float Weight { get; set; } = 1.25f;

        public override string Description { get; set; } = "\t";

        public override byte ClipSize { get; set; } = 60;

        public int Experience = 0;
        public int Level = 1;

        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 3,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 0,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideNukeArmory,
                }
            },
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shooting += Shooting;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shooting -= Shooting;
            base.UnsubscribeEvents();
        }


        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            Manager.SendHint(ev.Player, $"You just equipped the <color=#77d65a>\"MediGun (Level {Level})\"</color>, Shoot people to heal them and gain XP!", 5f);
            base.OnChanging(ev);
        }


        private void AddXP(Player player, int xp)
        {
            Experience += xp;

            if (Experience >= Mathf.Pow(2, Level) * 100)
            {
                LevelUp(player);
            }

            

        }

        private void LevelUp(Player player)
        {
            Level = Mathf.Clamp(Level, 1, 4);
            Level += 1;
            Manager.SendHint(player, $"<color=yellow>Your medgun just levelled up to Level {Level}!</color>", 5f);
            Timing.CallDelayed(5f, () => ((MvpSystem)Plugin.Instance._modules.GetModule("MvpSystem")).AddXpToPlayer(player, 3, "MediGun Level Up"));
        }
        private void Shooting(ShootingEventArgs ev)
        {
            if (ev.ClaimedTarget != null && ev.ClaimedTarget.IsHuman && ev.ClaimedTarget.Health < ev.ClaimedTarget.MaxHealth)
            {
                Manager.SendHint(ev.Player, $"You have healed {ev.ClaimedTarget.DisplayNickname}!", 3f);
                AddXP(ev.Player, 1);
                switch (Level)
                {
                    case 1:
                        {
                            ev.ClaimedTarget.Heal(2, false);
                            break;
                        }
                    case 2:
                        {
                            if (ev.ClaimedTarget.ArtificialHealth == 0) { ev.ClaimedTarget.AddAhp(10f); }
                            ev.ClaimedTarget.Heal(4, false);
                            break;
                        }
                    case 3:
                        {
                            if (ev.ClaimedTarget.ArtificialHealth == 0) { ev.ClaimedTarget.AddAhp(25f); }
                            ev.ClaimedTarget.Heal(6, false);
                            break;
                        }
                    case 4:
                        {
                            ev.ClaimedTarget.AddAhp(25f);
                            ev.ClaimedTarget.Heal(8, false);
                            ev.ClaimedTarget.EnableEffect(EffectType.MovementBoost, 2f, true);
                            ev.ClaimedTarget.ChangeEffectIntensity(EffectType.MovementBoost, 70);
                            break;
                        }
                }
            }
        }

    }
}