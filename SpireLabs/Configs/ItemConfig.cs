#pragma warning disable SA1200

using ObscureLabs.Items;

namespace ObscureLabs.ItemConfigs
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
