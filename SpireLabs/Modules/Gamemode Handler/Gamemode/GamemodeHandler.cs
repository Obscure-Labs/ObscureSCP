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
            //GamemodeManager.AddGamemode(new Chaos());
            return base.Enable();
        } 

        public override bool Disable()
        {

            return base.Disable();
        }

        private void RunRandomGamemode()
        {
            GamemodeManager.GetRandomGamemode().Enable();
        }

        private void RunGamemode(string gamemodeName)
        {
            GamemodeManager.GetGamemode(gamemodeName).Enable();
        }
    }
}
