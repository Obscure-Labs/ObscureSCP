using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using ObscureLabs.Commands.Admin.Other;

namespace ObscureLabs.API.Features
{
    public static class GamemodeManager
    {
        private static List<Gamemode> _gamemodeList = new();

        public static List<Gamemode> Gamemodes => _gamemodeList;

        public static Gamemode GetGamemode(string name)
        {
            var gm = _gamemodeList.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
            if(gm == null)
            {
                return GetGamemode();
            }
            return gm;
        }

        public static Gamemode GetGamemode(int index)
        {
            var gm = _gamemodeList.ElementAtOrDefault(index);
            if(gm == null)
            {
                return GetGamemode();
            }
            return gm;
        }

        public static Gamemode GetGamemode()
        {
            return _gamemodeList.GetRandomValue();
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
