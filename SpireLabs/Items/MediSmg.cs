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
using Exiled.API.Features.Toys;
using Light = Exiled.API.Features.Toys.Light;

namespace ObscureLabs.Items
{
    [CustomItem(ItemType.GunCrossvec)]
    public class MediSmg : Exiled.CustomItems.API.Features.CustomWeapon
    {
        public override float Damage { get; set; } = 20f;

        public override string Name { get; set; } = "MediGun (SMG)";

        public override uint Id { get; set; } = 14;

        public override float Weight { get; set; } = 1.25f;

        public override string Description { get; set; } = "\t";

        public override byte ClipSize { get; set; } = 40;

        public static List<Player> MediGunGlowingPlayers = new List<Player>();

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
            if (data.Experience >= Mathf.Pow(data.Level, 2) * 100)
            {
                data.Level = Mathf.Clamp(data.Level, 1, 3);
                data.Level += 1;
                Manager.SendHint(player, $"<color=yellow>Your MediGun just levelled up to Level {data.Level}!</color>", 5f);
                Timing.CallDelayed(5f, () => MvpSystem.AddXpToPlayer(player, 3, "MediGun Level Up"));
            }
            player.CurrentItem.SetData("MediSmgData", data);
        }

        public IEnumerator<float> PylonLight(Player p)
        {
            Light light = Light.Create(p.GameObject.transform.position, new Vector3(90, 0, 0), Vector3.one, false, Color.green);
            light.Intensity = 0f;
            light.Range = 0.0f;
            light.LightType = LightType.Point;
            light.ShadowType = LightShadows.Soft;

            light.Spawn();
            light.Base.gameObject.transform.parent = p.GameObject.transform;
            for (var i = 0; i < 10; i++)
            {
                yield return Timing.WaitForOneFrame;
                light.Range += 0.5f;
                light.Intensity += 1f;
            }

            yield return Timing.WaitForSeconds(0.5f);

            for (var i = 0; i < 10; i++)
            {
                yield return Timing.WaitForOneFrame;
                light.Range -= 0.5f;
                light.Intensity -= 1f;
            }
            yield break;
        }


 

        public IEnumerator<float> HealPylon(ShotEventArgs ev, int lvl)
        {
            foreach (Player p in Player.List)
            {
                if (Vector3.Distance(p.Transform.position, ev.Player.Transform.position) <= 8 && p != ev.Player && p.IsHuman)
                {
                    Timing.RunCoroutine(PylonLight(p));
                    switch (lvl)
                    {
                        case 1:
                            {
                                p.Heal(3, false);

                                break;
                            }
                        case 2:
                            {
                                p.Heal(3, false);
                                p.EnableEffect(EffectType.MovementBoost, 25, 2, false);
                                ev.Player.EnableEffect(EffectType.MovementBoost, 25, 2, false);
                                break;
                            }
                        case 3:
                            {
                                p.Heal(3, false);
                                p.EnableEffect(EffectType.MovementBoost, 40, 5, false);
                                p.EnableEffect(EffectType.DamageReduction, 5, 5, false);
                                ev.Player.Heal(1, false);
                                ev.Player.EnableEffect(EffectType.MovementBoost, 40, 1, false);
                                ev.Player.EnableEffect(EffectType.DamageReduction, 5, 5, false);
                                break;
                            }

                    }
                }
                else
                {
                    continue;
                }
            }
            yield break;
        }
            
          

        private void Shooting(ShotEventArgs ev)
        {
            if (!Check(ev.Item)) { return; }
            var data = ev.Player.CurrentItem.GetData<MediSmgData>("MediSmgData");
            //ev.Firearm.AmmoDrain = 0;


            if (ev.Target != null && ev.Target.Role.Team != ev.Player.Role.Team)
            {
                AddXP(ev.Player, 3);
               
                ev.Player.ShowHitMarker(1500f);
                Timing.RunCoroutine(HealPylon(ev, data.Level));
                PylonLight(ev.Player);
            }
        }

    }
}