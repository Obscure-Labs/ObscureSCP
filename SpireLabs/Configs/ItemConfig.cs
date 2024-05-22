
using ObscureLabs.Items;

namespace ObscureLabs.ItemConfigs
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class ItemConfig
    {
        [Description("Essentials oils")]
        public List<EsssentialOils> EssentialOils { get; private set; } = new()
        {
            new EsssentialOils(),
        };

        [Description("Sniper")]
        public List<Sniper> Sniper { get; private set; } = new()
        {
            new Sniper(),
        };
    }
}
