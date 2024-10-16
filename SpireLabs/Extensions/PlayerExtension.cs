using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UncomplicatedCustomRoles.Manager;
using Exiled.API.Features;
using stat = ObscureLabs.API.Enums.StatisticType.stat;
using ObscureLabs.API.Data;

namespace ObscureLabs.Extensions
{
    public static class PlayerExtension
    {
        private static Dictionary<Player, Dictionary<stat, string>> _playerData = new();
        private static Dictionary<Player, Dictionary<Powerup, float>> _powerupData = new();

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
    }
}
