#pragma warning disable SA1200

using SpireLabs.Items;

namespace SpireLabs.ItemConfigs
{
    using System.Collections.Generic;
    using System.ComponentModel;

    public class Items
    {
        public List<esssentialOils> essentialOils { get; private set; } = new()
        {
            new esssentialOils(),
        };
        public List<sniper> sniper { get; private set; } = new()
        {
            new sniper(),
        };
    }
}
