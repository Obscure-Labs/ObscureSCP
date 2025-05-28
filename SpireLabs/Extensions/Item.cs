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
using Exiled.CustomItems.API.Features;

namespace ObscureLabs.Extensions
{
    public static class ItemExtensions
    {
        private static Dictionary<ushort, Dictionary<string, object>> _itemData = new Dictionary<ushort, Dictionary<string, object>>();

        public static T GetData<T>(this Item item, string variableName)
        {
            if (!_itemData.ContainsKey(item.Serial))
            {
                throw new Exception("item has no data.");
            }
            else
            {
                if (!_itemData[item.Serial].ContainsKey(variableName))
                {
                    throw new Exception($"item doesnt have a variable called {variableName}");
                }
                else
                {
                    return (T)_itemData[item.Serial][variableName];
                }
            }
        }
        
        public static object GetData(this Item item, string variableName)
        {
            if (!_itemData.ContainsKey(item.Serial))
            {
                throw new NullReferenceException();
            }
            else
            {
                if (!_itemData[item.Serial].ContainsKey(variableName))
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return _itemData[item.Serial][variableName];
                }
            }
        }

        public static object TryGetData(this Item item, string variableName)
        {
            if (!_itemData.ContainsKey(item.Serial))
            {
                return null;
            }
            else
            {
                if (!_itemData[item.Serial].ContainsKey(variableName))
                {
                    return null;
                }
                else
                {
                    return _itemData[item.Serial][variableName];
                }
            }
        }

        public static void SetData(this Item item, string variableName, object data)
        {
            if (!_itemData.ContainsKey(item.Serial))
            {
                _itemData.Add(item.Serial, new Dictionary<string, object>() { { variableName, data } });
            }
            else
            {
                if (!_itemData[item.Serial].ContainsKey(variableName))
                {
                    _itemData[item.Serial].Add(variableName, data);
                }
                else
                {
                    _itemData[item.Serial][variableName] = data;
                }
            }
        }
    }
}
