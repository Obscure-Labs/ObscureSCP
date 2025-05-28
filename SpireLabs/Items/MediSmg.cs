using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using System.Collections.Generic;
using UnityEngine;
using Player = Exiled.API.Features.Player;
using SpireSCP.GUI.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Core;
using ObscureLabs.Extensions;
using Exiled.API.Features.Items;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunSCP127)]
    public class MediSmg : Exiled.CustomItems.API.Features.CustomWeapon
    {
        public override float Damage { get; set; } = 0f;

        public override string Name { get; set; } = "MediGun (SMG)";

        public override uint Id { get; set; } = 14;

        public override float Weight { get; set; } = 1.25f;

        public override string Description { get; set; } = "\t";

        public override byte ClipSize { get; set; } = 60;

        class MediSmgData
        {
            public MediSmgData()
            {
                Experience = 0;
                Level = 1;
            }
            public int Experience { get; set; }
            public int Level { get; set; }
        }  

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

        protected override void OnAcquired(Player player, Item item, bool displayMessage)
        {
            if(item.TryGetData("MediSmgData") == null)
            {
                item.SetData("MediSmgData", new MediSmgData());
            }
        }

        protected override void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.Shot -= Shooting;
            base.UnsubscribeEvents();
        }


        protected override void OnChanging(ChangingItemEventArgs ev)
        {
            var data = ev.Item.GetData<MediSmgData>("MediSmgData");
            Manager.SendHint(ev.Player, $"You just equipped the \"MediGun\" \n<color=#77d65a>Level {data.Level} | {data.Experience}xp</color> \nShoot people to heal them and gain XP!", 5f);
            base.OnChanging(ev);
        }


        private void AddXP(Player player, int xp)
        {
            var data = player.CurrentItem.GetData<MediSmgData>("MediSmgData");
            data.Experience += xp;
            Manager.SendHint(player, $"<pos=0>Your MediGun has <color=#77d65a>{data.Experience}xp</color>", 2f);
            if (data.Experience >= Mathf.Pow(2, data.Level) * 100)
            {
                data.Level = Mathf.Clamp(data.Level, 1, 4);
                data.Level += 1;
                Manager.SendHint(player, $"<color=yellow>Your MediGun just levelled up to Level {data.Level}!</color>", 5f);
                Timing.CallDelayed(5f, () => MvpSystem.AddXpToPlayer(player, 3, "MediGun Level Up"));
            }
            player.CurrentItem.SetData("MediSmgData", data);
        }

        private void Shooting(ShotEventArgs ev)
        {
            if (!Check(ev.Item)) { return; }
            var data = ev.Player.CurrentItem.GetData<MediSmgData>("MediSmgData");
            ev.Firearm.AmmoDrain = 0;
            if (ev.Target != null && ev.Target.IsHuman && ev.Target.Health < ev.Target.MaxHealth)
            {
                AddXP(ev.Player, 1);
                ev.Player.ShowHitMarker(1500f);
                switch (data.Level)
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