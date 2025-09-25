using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs.Player;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Extensions;
using MEC;
using SpireSCP.GUI.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Enums;
using InventorySystem.Items.Usables.Scp330;
using PlayerRoles;
using Exiled.API.Features.Doors;
using System.Runtime.Remoting.Metadata;
using CommandSystem.Commands.RemoteAdmin.Doors;
using System.Runtime.InteropServices;
using Exiled.API.Interfaces;
using Exiled.API.Features.Core;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class FlipResult
    {
        public FlipResult(string name, string hint, Func<Exiled.API.Features.Player, bool> func)
        {
            Name = name;
            Hint = hint;
            Func = func;
        }

        public string Name { get; set; }
        public string Hint { get; set; }
        public Func<Exiled.API.Features.Player, bool> Func { get; set; }

        public void SendHint(Player player)
        {
            Manager.SendHint(player, Hint, 5f);
        }
    }

    public class CoinFlip : Module
    {
        public override string Name => "CoinFlip";

        public override bool IsInitializeOnStart => true;

        public List<FlipResult> _goodResults = new List<FlipResult>()
        {
            new("RandomItem", "You got a random item!", (Player) =>
            {
                var vals = Enum.GetValues(typeof(ItemType));
                Player.AddItem((ItemType)vals.GetValue(UnityEngine.Random.Range(0, vals.Length)));
                return true;
            }),
            new("AddHp50", "You gained <color=green>50</color> HP!", (Player) =>
            {
                Player.Heal(50, false);
                return true;
            }),
            new("SpeedBoost5", "You gained a <color=blue><u>5 second speed boost!</u></color>", (Player) =>
            {
                Player.EnableEffect(Exiled.API.Enums.EffectType.MovementBoost, 5f);
                Player.ChangeEffectIntensity(Exiled.API.Enums.EffectType.MovementBoost, 205, 5f);
                return true;
            }),
            new("RandKeycard", "You found a <color=yellow>Keycard!</color>", (Player) =>
            {
                if(!Player.IsInventoryFull)
                {
                    Player.AddItem(Item.List.Where(item => item is Keycard).GetRandomValue().Type);
                }
                else
                {
                    Pickup.CreateAndSpawn(Item.List.Where(item => item is Keycard).GetRandomValue().Type, Player.Position, Player.Rotation);
                }

                return true;
            }),
            new("Invis5", "You gained a <color=purple><u>5 second invisibility!</u></color>", (Player) =>
            {
                Player.EnableEffect(Exiled.API.Enums.EffectType.Invisible, 5f);
                return true;
            }),
            new("Heal", "You are fully healed!", (Player) =>
            {
                Player.Heal(Player.MaxHealth, false);
                return true;
            }),
            new("AmmoFountain", "<color=purple><u>AMMO FOUNTAIN</u></color>", (Player) =>
            {
                Timing.RunCoroutine(AmmoFountainCoroutine(Player));
                return true;
            }),
            new("CandyFountain", "<color=purple><u>CANDY FOUNTAIN</u></color>", (Player) =>
            {
                Timing.RunCoroutine(CandyFountainCoroutine(Player));
                return true;
            }),
            new("DamageReduction", "You gained <color=green><u>5 second Damage Reduction!</u></color>", (Player) =>
            {
                Player.EnableEffect(Exiled.API.Enums.EffectType.DamageReduction, 5f);
                Player.ChangeEffectIntensity(Exiled.API.Enums.EffectType.DamageReduction, 50, 5f);
                return true;
            }),
            new("HealSurrounding", "You heal neaby players for 5 seconds!", (Player) =>
            {
                Timing.RunCoroutine(HealSurrounding(Player));
                return true;
            }),
            new("GiveSCP268", "You found a <color=red><u>HAT!!</u></color>", (Player) =>
            {
                if(!Player.IsInventoryFull)
                {
                    Player.AddItem(ItemType.SCP268);
                }
                else
                {
                    Pickup.CreateAndSpawn(ItemType.SCP268, Player.Position, Player.Rotation);
                }
                return true;
            }),
            new("GLOW", "<color=yellow>DAMB THAT'S BRIGHT</color>", (Player) =>
            {
                Exiled.API.Features.Toys.Light li = Exiled.API.Features.Toys.Light.Create(new Vector3(Player.Transform.position.x, Player.Transform.position.y + 1.2f, Player.Transform.position.z), Vector3.zero, Vector3.one, false);
                li.Range = 45f;
                li.Intensity = 9999f;
                li.Color = Color.yellow;
                li.Spawn();
                li.Base.gameObject.transform.SetParent(Player.GameObject.transform);
                Timing.CallDelayed(30f, () => { li.Destroy(); });
                return true;
            }),
            new("TPtoPlayer", "You have been teleported to a random player!", (Player) =>
            {
                var pList = Player.List.ToList();
                try{
                var p = pList.OrderBy(x => Guid.NewGuid()).FirstOrDefault(x => x.Role != RoleTypeId.Spectator && x.Role != RoleTypeId.None && x.Role != RoleTypeId.Overwatch && x.IsAlive && x != Player);
                Player.Position = p.Position;
                }
                catch
                {
                    Player.Vaporize();    
                }
                return true;
            }),
        };

        public List<FlipResult> _badResults = new List<FlipResult>()
        {
            new("Antigravity", "uʍop ǝpᴉsd∩", (Player) =>
            {
                Vector3 oldGrav = LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity;
                LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity *= -1;
                Timing.CallDelayed(10f, () => {LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity = Plugin.PlayerDefaultGravity; });
                return true;
            }),
            new("Death", "You rolled <color=#767676><u>DEATH</u></color>", (Player) =>
            {
                Player.Vaporize();
                return true;
            }),
            new("GrenadeFountain", "<color=red><u>GRENADE FOUNTAIN</u></color>", (Player) =>
            {
                Timing.RunCoroutine(GrenadeFountainCoroutine(Player));
                return true;
            }),
            new("HealthMinus50", "Your health is now <color=red>50</color>", (Player) =>
            {
                Player.Health = 50f;
                return true;
            }),
            new("Disarm", "You dropped all your items!", (Player) =>
            {
                Player.DropItems();
                return true;
            }),
            new("HeavyFeet", "Your feet turn to lead!", (Player) =>
            {
                Player.EnableEffect(EffectType.SinkHole, 10f);
                Player.ChangeEffectIntensity(EffectType.SinkHole, 1, 10f);
                return true;
            }),
            new("Flashbang", "THINK FAST CHUCKLENUTS", (Player) =>
            {
                for (int i = 0; i < 5; i++)
                {
                    FlashGrenade grenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                    grenade.FuseTime = UnityEngine.Random.Range(0, 100) / 100f * 1.5f;
                    grenade.SpawnActive(Player.Transform.position, null);
                }
                return true;
            }),
            new("Low Gravity", "Low Gravity", (Player) =>
            {
                Vector3 oldGrav = LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity;
                LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity = (Plugin.PlayerDefaultGravity * 0.8f);
                Timing.CallDelayed(10f, () => {LabApi.Features.Wrappers.Player.Get(Player.NetworkIdentity).Gravity = Plugin.PlayerDefaultGravity; });
                return true;
            }),
            new("Loud", "WAAAAAAAAAAAAAAAAAAAAAAAH", (Player) =>
            {
                for (int i = 0; i < 10; i++)
                {
                    foreach (Door d in Door.List)
                    {
                        Timing.CallDelayed(UnityEngine.Random.Range(0.01f, 1f), () => d.PlaySound(DoorBeepType.PermissionDenied));
                    }
                }
                return true;
            }),
            new("RandRoom", "You find yourself in a random room...", (Player) =>
            {
                if (Warhead.IsDetonated)
                {
                    return false;
                }

                bool goodRoom = false;
                Room room = Room.List.ElementAt(0);
                while (!goodRoom)
                {
                    room = Room.List.GetRandomValue();
                    if(room.Zone == ZoneType.Surface)
                    {
                        continue;
                    }
                    if (Map.IsLczDecontaminated && room.Zone == ZoneType.LightContainment)
                    {
                        continue;
                    }
                    if(room.Type is RoomType.HczTesla or RoomType.HczElevatorA or RoomType.HczElevatorB or RoomType.Hcz079)
                    {
                        continue;
                    }
                    goodRoom = true;
                }
                Player.Position = room.Doors.FirstOrDefault().Position + new Vector3(0f, 1.5f, 0f);
                return true;
            }),
        };

        private int _goodThreshold = 60;


        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.FlippingCoin += CoinFlipEvent;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.FlippingCoin -= CoinFlipEvent;
            return base.Disable();
        }


        private void CoinFlipEvent(FlippingCoinEventArgs ev)
        {
            int chance = UnityEngine.Random.Range(0, 100);
            bool result = chance >= _goodThreshold ? true : false;
            FlipResult outcome;

            if (result)
            {
                outcome = _goodResults.ElementAt(UnityEngine.Random.Range(0, _goodResults.Count()));
            }
            else
            {
                outcome = _badResults.ElementAt(UnityEngine.Random.Range(0, _badResults.Count()));
            }

            if (outcome.Func.Invoke(ev.Player))
            {
                outcome.SendHint(ev.Player);
            }
        }

        #region CoroutinesForResults
        public static IEnumerator<float> GrenadeFountainCoroutine(Player p)
        {
            var bombs = 0;
            while (bombs != 2)
            {
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                p.ThrowGrenade(ProjectileType.FragGrenade, false);
                yield return Timing.WaitForSeconds(0.1f);
                bombs++;
            }
        }

        public static IEnumerator<float> AmmoFountainCoroutine(Player p)
        {
            int itemTotal = (UnityEngine.Random.Range(0, 101) >= 95) ? 350 : 50;

            for (int i = 0; i < itemTotal; i++)
            {
                Pickup.CreateAndSpawn(ItemType.Ammo9x19, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo762x39, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo12gauge, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo556x45, p.Position, p.Rotation);
                Pickup.CreateAndSpawn(ItemType.Ammo44cal, p.Position, p.Rotation);
                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> CandyFountainCoroutine(Player p)
        {
            for(int i = 0; i < 50; i++)
            {
                var pickup = Pickup.Create(ItemType.SCP330) as Exiled.API.Features.Pickups.Scp330Pickup;
                pickup.Candies.Add((CandyKindID)typeof(CandyKindID).GetEnumValues().GetValue(UnityEngine.Random.Range(0, Enum.GetValues(typeof(CandyKindID)).Length + 1)));
                pickup.Spawn(p.Position, p.Rotation);
                yield return Timing.WaitForOneFrame;
            }
        }

        public static IEnumerator<float> HealSurrounding(Player p)
        {
            for (int i = 0; i < 50; i++)
            {
                foreach (var player in Player.List)
                {
                    if (Vector3.Distance(p.Position, player.Position) <= 5f)
                    {
                        player.Heal(5, false);
                    }
                }
                yield return Timing.WaitForSeconds(0.1f);
            }
        }
        #endregion
    }
}
