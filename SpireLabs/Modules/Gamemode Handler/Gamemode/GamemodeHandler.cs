using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Gamemode.Gamemodes;

namespace ObscureLabs.Modules.Gamemode_Handler.Gamemodes
{
    internal class GamemodeHandler : Module
    {
        public override string Name => "GamemodeHandler";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            //GamemodeManager.AddGamemode(new TDM());
            GamemodeManager.AddGamemode(new Chaos());
            var c = UnityEngine.Random.Range(0, 101);
            if(c >= 50 && c <= 55)
            {
                RunRandomGamemode(false);
            }
            return base.Enable();
        } 

        public override bool Disable()
        {
            GamemodeManager.Clear();
            return base.Disable();
        }

        private void RunRandomGamemode(bool force)
        {
            GamemodeManager.GetGamemode().Enable(force);
        }

        private void RunGamemode(string gamemodeName, bool force)
        {
            GamemodeManager.GetGamemode(gamemodeName).Enable(force);
        }
    }
}
