using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UncomplicatedCustomRoles.Manager;
using Exiled.API.Features;
using Exiled.API.Features.Core.StateMachine;
using stat = ObscureLabs.API.Enums.StatisticType.stat;

namespace ObscureLabs.Extensions
{
    public static class PlayerExtension
    {
        private static Dictionary<Player, Dictionary<string, object>> _playerData = new Dictionary<Player, Dictionary<string, object>>();

        public static T GetData<T>(this Player player, string variableName)
        {
            if (!_playerData.ContainsKey(player))
            {
                throw new Exception("Player has no data.");
            }
            else
            {
                if (!_playerData[player].ContainsKey(variableName))
                {
                    throw new Exception($"Player doesnt have a variable called {variableName}");
                }
                else
                {
                    return (T)_playerData[player][variableName];
                }
            }
        }
        
        public static object GetData(this Player player, string variableName)
        {
            if (!_playerData.ContainsKey(player))
            {
                throw new Exception("Player has no data.");
            }
            else
            {
                if (!_playerData[player].ContainsKey(variableName))
                {
                    throw new Exception($"Player doesnt have a variable called {variableName}");
                }
                else
                {
                    return _playerData[player][variableName];
                }
            }
        }

        public static void SetData(this Player player, string variableName, object data)
        {
            if (!_playerData.ContainsKey(player))
            {
                _playerData.Add(player, new Dictionary<string, object>() { { variableName, data } });
            }
            else
            {
                if (!_playerData[player].ContainsKey(variableName))
                {
                    _playerData[player].Add(variableName, data);
                }
                else
                {
                    _playerData[player][variableName] = data;
                }
            }
        }
    }
}
