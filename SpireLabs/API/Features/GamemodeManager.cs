using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Features
{
    public static class GamemodeManager
    {
        private static List<Gamemode> _gamemodeList = new();

        public static List<Gamemode> Gamemodes => _gamemodeList;

        public static Gamemode GetGamemode(string name)
        {
            return _gamemodeList.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        public static void AddGamemode(Gamemode gamemode)
        {
            _gamemodeList.Add(gamemode);
        }

        public static void Clear()
        {
            _gamemodeList.Clear();
        }
    }
}
