using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using UncomplicatedCustomRoles.Manager;
using Exiled.API.Features;
using Exiled.API.Features.Core.StateMachine;
using Exiled.API.Features.Items;
using stat = ObscureLabs.API.Enums.StatisticType.stat;
using System.Diagnostics;

namespace ObscureLabs.Extensions
{
    [Serializable]
    public class NoDataException : Exception
    {
        public NoDataException()
        { }

        public NoDataException(string message)
            : base(message)
        { }

        public NoDataException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    [Serializable]
    public class NoVarException : Exception
    {
        public NoVarException()
        { }

        public NoVarException(string message)
            : base(message)
        { }

        public NoVarException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }



    public static class PlayerExtension
    {
        private static Dictionary<Player, Dictionary<string, object>> _playerData = new Dictionary<Player, Dictionary<string, object>>();

        public static bool HasPermission(this Exiled.API.Features.Player p, KeycardPermissions perms)
        {
            return p.Items.Any(x => x is Keycard k && k.Permissions.HasFlag(perms));
        }
        
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
