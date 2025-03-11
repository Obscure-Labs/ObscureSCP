using Exiled.API.Features;
using Exiled.API.Features.Items;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Features
{
    public class ItemRarityAPI
    {
        public static Rarity GetRarity(ItemType type)
        {
            return ((ItemRarityModule)Plugin.Instance._modules.GetModule("ItemRarityModule"))._itemRarityData.Find(x => x.Type == type).Rarity;
        }
    }
    public class ItemRarityModule : Module
    {
        public override string Name => "ItemRarity";

        public override bool IsInitializeOnStart => true;

        public List<ItemRarityData> _itemRarityData = new List<ItemRarityData>();

        public YamlDotNet.Serialization.Serializer _serializer = new YamlDotNet.Serialization.Serializer();
        public YamlDotNet.Serialization.Deserializer _deserializer = new YamlDotNet.Serialization.Deserializer();

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
            return base.Enable();
        }

        public override bool Disable()
        {
            LabApi.Features.Console.Logger.Info("ItemRarity disabled");
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            return base.Disable();
        }

        public void OnWaitingForPlayers()
        {
            if (!System.IO.File.Exists(Plugin.SpireConfigLocation + "itemRarities.yaml"))
            {
                List<ItemRarityData> tempData = new List<ItemRarityData>();
                foreach (ItemType i in Enum.GetValues(typeof(ItemType)))
                {
                    Log.Error("Adding item rarity data for " + i.ToString());
                    tempData.Add(new ItemRarityData { Type = i, Rarity = Rarity.None });
                }
                System.IO.File.WriteAllText(Plugin.SpireConfigLocation + "itemRarities.yaml", _serializer.Serialize(tempData));
            }
            LabApi.Features.Console.Logger.Info("ItemRarity enabled");
            _itemRarityData = _deserializer.Deserialize<List<ItemRarityData>>(System.IO.File.ReadAllText(Plugin.SpireConfigLocation + "itemRarities.yaml"));
        }
    }

    public class ItemRarityData
    {
        public ItemType Type { get; set; }
        public Rarity Rarity { get; set; }
    }

    public enum Rarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Legendary,
        Obscure
    }
}
