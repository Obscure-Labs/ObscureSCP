using Exiled.Events.EventArgs.Player;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Items
{
    internal class ItemUtils
    {
        public static void item_change(ChangedItemEventArgs ev)
        {
            if (ev.Item == null) return;

            if (ev.Item.Type != ItemType.Coin)
                return;
            string hint = string.Empty;
            hint += "Flipping this coin will cause a random event, use with caution!";
            Manager.SendHint(ev.Player, hint, 5);

        }

    }
}
