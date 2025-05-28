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
using UnityEngine.Assertions.Must;
using ObscureLabs.Extensions;
using ObscureLabs.API.Data;

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
            DynamicSpawnPoints = new List<DynamicSpawnPoint>()
        };

        protected override void SubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shot += Shooting;
            
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shot -= Shooting;
            base.UnsubscribeEvents();
        }


        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            
            Manager.SendHint(ev.Player, $"You just equipped the \"MediGun\" \n<color=#77d65a>Level {Level} | {Experience}xp</color> \nShoot people to heal them and gain XP!", 5f);
            base.OnChanging(ev);
        }


        private void AddXP(Player player, int xp)
        {
            Experience += xp;
            Manager.SendHint(player, $"<pos=0>Your MediGun has <color=#77d65a>{Experience}xp</color>", 2f);
            if (Experience >= Mathf.Pow(2, Level) * 100)
            {
                LevelUp(player);
            }



        }
        
        

        private void LevelUp(Player player)
        {
            Level = Mathf.Clamp(Level, 1, 4);
            Level += 1;
            Manager.SendHint(player, $"<color=yellow>Your MediGun just levelled up to Level {Level}!</color>", 5f);
            Timing.CallDelayed(5f, () => MvpSystem.AddXpToPlayer(player, 3, "MediGun Level Up"));
        }

        private void Shooting(ShotEventArgs ev)
        {
            
            ev.Firearm.AmmoDrain = 0;
            if (ev.Target != null && ev.Target.IsHuman && ev.Target.Health < ev.Target.MaxHealth)
            {
                AddXP(ev.Player, 1);
                ev.Player.ShowHitMarker(1500f);
                
                switch (Level)
                {
                    case 1:
                        {
                            ev.Target.Heal(2, false);
                            break;
                        }
                    case 2:
                        {
                            if (ev.Target.ArtificialHealth <= 25) { ev.Target.ArtificialHealth += 3f; }
                            ev.Target.Heal(4, false);
                            break;
                        }
                    case 3:
                        {
                            if (ev.Target.ArtificialHealth <= 50) { ev.Target.ArtificialHealth += 5f; }
                            ev.Target.Heal(6, false);
                            break;
                        }
                    case 4:
                        {
                            if (ev.Target.ArtificialHealth <= 100) { ev.Target.ArtificialHealth += 10f; }
                            ev.Target.Heal(8, false);
                            ev.Target.EnableEffect(EffectType.MovementBoost, 2f, true);
                            ev.Target.ChangeEffectIntensity(EffectType.MovementBoost, 70);
                            break;
                        }
                }
            }
        }

    }
}