using Exiled.API.Features;
using Exiled.API.Features.Toys;
using System.Collections.Generic;

namespace ObscureLabs.API.Data
{
    public class PlayerPrimitiveData
    {
        public PlayerPrimitiveData(Player player, List<AdminToy> objects)
        {
            Player = player;
            Objects = objects;
        }

        public Player Player { get; set; }

        public List<AdminToy> Objects { get; set; }
    }
}
