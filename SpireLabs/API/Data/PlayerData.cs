using Exiled.API.Features;

namespace ObscureLabs.API.Data
{

    public class PlayerData
    {
        public PlayerData(Player player, int xp)
        {
            Player = player;
            Xp = xp;
        }

        public Player Player { get; }

        public int Xp { get; set; }
    }
}
