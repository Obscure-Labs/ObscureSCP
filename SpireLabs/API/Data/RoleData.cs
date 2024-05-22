using Exiled.API.Features;

namespace ObscureLabs.API.Data
{
    public class RoleData
    {
        public RoleData(Player player, int? ucrId)
        {
            Player = player;
            UcrId = ucrId;
        }

        public Player Player { get; }

        public int? UcrId { get; }
    }
}
