using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UncomplicatedCustomRoles.Manager;
using Exiled.API.Features;
using stat = ObscureLabs.API.Enums.StatisticType.stat;
using ObscureLabs.API.Data;
using ObscureLabs.API.Data.Effects;
using ObscureLabs.API.Enums;

namespace ObscureLabs.Extensions
{
    public static class PlayerExtension
    {
        private static Dictionary<Player, Dictionary<stat, string>> _playerData = new();
        private static Dictionary<Player, List<EffectBase>> _playerEffects = new();
        public static string Data(this Player player, stat StatisticType)
        {
            if(!_playerData.ContainsKey(player))
            {
                return null;
            }
            else if(!_playerData[player].ContainsKey(StatisticType))
            {
                return null;
            }
            else
            {
                return (_playerData[player])[StatisticType].ToString();
            }
        }

        public static bool SetData(this Player player, stat StatisticType, string data)
        {
            if(!_playerData.ContainsKey(player))
            {
                _playerData.Add(player, new Dictionary<stat, string>());
                return false;
            }
            if(!_playerData[player].ContainsKey(StatisticType))
            {
                _playerData[player].Add(StatisticType, data);
                return true;
            }
            else
            {
                _playerData[player][StatisticType] = data;
                return true;
            }
        }

        public static void GiveEffect(this Player player, Effects effect)
        {
            switch (effect)
            {
                case Effects.StolenUniformGuard:
                    _playerEffects[player].Add(new StolenUniformGuard(player));
                    break;
            }
        }

        public static void GiveEffect(this Player player, Effects effect, float duration)
        {
            switch (effect)
            {
                case Effects.StolenUniformGuard:
                    _playerEffects[player].Add(new StolenUniformGuard(player, duration));
                    break;
            }
        }

        public static void RemoveEffect(this Player player, Effects effect)
        {
            switch (effect)
            {
                case Effects.StolenUniformGuard:
                    _playerEffects[player].Find(x => x.Name == "StolenUniformGuard").Remove();
                    _playerEffects[player].Remove(_playerEffects[player].Find(x => x.Name == "StolenUniformGuard"));
                    break;
            }
        }
    }
}
