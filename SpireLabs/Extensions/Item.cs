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
        private static Dictionary<Item, Dictionary<string, object>> _itemData = new Dictionary<Item, Dictionary<string, object>>();

        public static T GetData<T>(this Item item, string variableName)
        {
            if (!_itemData.ContainsKey(item))
            {
                throw new Exception("item has no data.");
            }
            else
            {
                if (!_itemData[item].ContainsKey(variableName))
                {
                    throw new Exception($"item doesnt have a variable called {variableName}");
                }
                else
                {
                    return (T)_itemData[item][variableName];
                }
            }
        }
        
        public static object GetData(this Item item, string variableName)
        {
            if (!_itemData.ContainsKey(item))
            {
                throw new NullReferenceException();
            }
            else
            {
                if (!_itemData[item].ContainsKey(variableName))
                {
                    throw new NullReferenceException();
                }
                else
                {
                    return _itemData[item][variableName];
                }
            }
        }

        public static object TryGetData(this Item item, string variableName)
        {
            if (!_itemData.ContainsKey(item))
            {
                return null;
            }
            else
            {
                if (!_itemData[item].ContainsKey(variableName))
                {
                    return null;
                }
                else
                {
                    return _itemData[item][variableName];
                }
            }
        }

        public static void SetData(this Item item, string variableName, object data)
        {
            if (!_itemData.ContainsKey(item))
            {
                _itemData.Add(item, new Dictionary<string, object>() { { variableName, data } });
            }
            else
            {
                if (!_itemData[item].ContainsKey(variableName))
                {
                    _itemData[item].Add(variableName, data);
                }
                else
                {
                    _itemData[item][variableName] = data;
                }
            }
        }
    }
}
