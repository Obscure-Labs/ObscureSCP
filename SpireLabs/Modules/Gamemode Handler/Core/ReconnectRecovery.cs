using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using CustomItems.API;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.API.Features.Roles;
using Exiled.Events.EventArgs.Player;
using InventorySystem;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using UncomplicatedCustomRoles.Extensions;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class ReconnectRecovery : Module
    {
        public override string Name => "ReconnectRecovery";
        public override bool IsInitializeOnStart => true;

        private List<SerializableReconnectData> _reconnectData;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Left += OnPlayerLeft;
            Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
            _reconnectData = new List<SerializableReconnectData>();
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeft;
            Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
            _reconnectData = null;
            return base.Disable();
        }

        private void OnPlayerLeft(LeftEventArgs ev)
        {
            if (ev.Player.Role.Side != Side.Scp)
            {
                SerializableReconnectData data = new()
                {
                    UserId = ev.Player.UserId,
                    Role = ev.Player.Role,
                    HP = ev.Player.Health,
                    Position = ev.Player.Position,
                    LastSeen = DateTime.UtcNow,
                    Items = new HashSet<Item>()
                };
                if (ev.Player.HasCustomRole())
                {
                    data.CustomRoleId = ev.Player.GetCustomRole().Id;
                }

                foreach (Item item in ev.Player.Items.ToHashSet())
                {
                    data.Items.Add(item.Clone());
                }

                foreach (KeyValuePair<ItemType, ushort> ammo in ev.Player.Ammo)
                {
                    data.AmmoCount.Add(ammo.Key, ammo.Value);
                }

                foreach (StatusEffectBase effect in ev.Player.ActiveEffects)
                {
                    KeyValuePair<EffectType, float[]> effectData = new(effect.GetEffectType(),
                        [effect.Duration, effect.Intensity]);
                    data.Effects.Add(effectData);
                }

                Timing.CallDelayed(45f, delegate { Timing.RunCoroutine(checkPlayer(ev.Player)); });
            }
            else
            {
                if (Player.List.Where(x => x.Role == RoleTypeId.Spectator).Count() != 0)
                {
                    var newPlayer = Player.List.Where(x => x.Role == RoleTypeId.Spectator).ToList().PullRandomItem();
                    Timing.RunCoroutine(spawnNew(ev, newPlayer));

                }
            }
        }

        private IEnumerator<float> checkPlayer(Player p)
        {
            yield return Timing.WaitForOneFrame;
            if (_reconnectData.Any(data => data.UserId == p.UserId))
            {
                SerializableReconnectData data = _reconnectData.First(d => d.UserId == p.UserId);
                foreach (var i in data.Items)
                {
                    i.CreatePickup(data.Position).Spawn();
                }
                _reconnectData.Remove(_reconnectData.First(data => data.UserId == p.UserId));
            }
        }

        private IEnumerator<float> spawnNew(LeftEventArgs ev, Player newPlayer)
        {
            yield return Timing.WaitForOneFrame;
            newPlayer.RoleManager.ServerSetRole(ev.Player.Role, RoleChangeReason.None, RoleSpawnFlags.None);
            yield return Timing.WaitForSeconds(0.1f);
            newPlayer.Teleport(ev.Player.Position);
            newPlayer.Health = ev.Player.Health;
        }

        private void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Timing.RunCoroutine(SpawnPlayer(ev.Player));
        }

        private IEnumerator<float> SpawnPlayer(Player p)
        {
            if (_reconnectData.Any(data => data.UserId == p.UserId))
            {
                SerializableReconnectData data = _reconnectData.First(d => d.UserId == p.UserId);
                p.RoleManager.ServerSetRole(data.Role.Type, RoleChangeReason.LateJoin, RoleSpawnFlags.None);
                yield return Timing.WaitForSeconds(0.1f);
                if (data.CustomRoleId != 0)
                {
                    p.SetCustomRole(data.CustomRoleId);
                }
                yield return Timing.WaitForSeconds(0.1f);
                p.Teleport(data.Position);
                yield return Timing.WaitForSeconds(0.1f);
                foreach (Item item in data.Items)
                {
                    item.Give(p);
                }

                foreach (KeyValuePair<ItemType, ushort> ammo in data.AmmoCount)
                {
                   p.Ammo[ammo.Key] = ammo.Value;
                }

                foreach (KeyValuePair<EffectType, float[]> effectData in data.Effects)
                {
                    p.EnableEffect(effectData.Key, effectData.Value[0]);
                    p.ChangeEffectIntensity(effectData.Key, (byte)effectData.Value[1]);
                }
                p.Health = data.HP;
                _reconnectData.RemoveAll(d => d.UserId == p.UserId);
            }

            yield return Timing.WaitForOneFrame;
        }

        private class SerializableReconnectData
        {
            public string UserId { get; set; }
            public Role Role { get; set; }
            public int CustomRoleId { get; set; }
            public HashSet<Item> Items { get; set; }
            public float HP { get; set; }
            public IDictionary<ItemType, ushort> AmmoCount { get; set; }
            public IDictionary<EffectType, float[]> Effects { get; set; }
            public UnityEngine.Vector3 Position { get; set; }
            public DateTime LastSeen { get; set; }
        }
    }
}
