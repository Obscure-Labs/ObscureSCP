using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObscureLabs.API.Features;

namespace ObscureLabs.Modules.Gamemode_Handler.Gamemodes
{
    internal class GamemodeHandler : Module
    {
        public override string Name => "GamemodeHandler";
        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {

            return base.Enable();
        } 

        public override bool Disable()
        {

            return base.Disable();
        }

        private void RunRandomGamemode()
        {

        }

        private void RunGamemode(string gamemodeName)
        {
            GamemodeManager.GetGamemode(gamemodeName).Enable();
        }
    }
}
