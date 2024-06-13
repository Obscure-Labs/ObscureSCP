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
            SerializableReconnectData data = new()
            {
                UserId = ev.Player.UserId,
                Role = ev.Player.Role,
                HP = ev.Player.Health,
                Position = ev.Player.Position
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
                KeyValuePair<EffectType, float[]> effectData = new(effect.GetEffectType(), [effect.Duration, effect.Intensity]);
                data.Effects.Add(effectData);
            }
        }

        private void OnPlayerVerified(VerifiedEventArgs ev)
        {
            Timing.RunCoroutine(SpawnPlayer(ev));
        }

        private IEnumerator<float> SpawnPlayer(VerifiedEventArgs ev)
        {
            if (_reconnectData.Any(data => data.UserId == ev.Player.UserId))
            {
                SerializableReconnectData data = _reconnectData.First(d => d.UserId == ev.Player.UserId);
                ev.Player.RoleManager.ServerSetRole(data.Role.Type, RoleChangeReason.LateJoin, RoleSpawnFlags.None);
                yield return Timing.WaitForSeconds(0.1f);
                if (data.CustomRoleId != 0)
                {
                    ev.Player.SetCustomRole(data.CustomRoleId);
                }
                yield return Timing.WaitForSeconds(0.1f);
                ev.Player.Teleport(data.Position);
                yield return Timing.WaitForSeconds(0.1f);
                foreach (Item item in data.Items)
                {
                    ev.Player.AddItem(item);
                }

                foreach (KeyValuePair<ItemType, ushort> ammo in data.AmmoCount)
                {
                    ev.Player.Ammo[ammo.Key] = ammo.Value;
                }

                foreach (KeyValuePair<EffectType, float[]> effectData in data.Effects)
                {
                    ev.Player.EnableEffect(effectData.Key, effectData.Value[0]);
                    ev.Player.ChangeEffectIntensity(effectData.Key, (byte)effectData.Value[1]);
                }
                ev.Player.Health = data.HP;
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
        }
    }
}
