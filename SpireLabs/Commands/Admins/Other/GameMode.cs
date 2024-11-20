using CommandSystem;
using Exiled.API.Features;
using System;
using ObscureLabs.API.Features;
using ObscureLabs.Modules.Gamemode_Handler.Gamemodes;

namespace ObscureLabs.Commands.Admin.Other
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GameMode : ICommand
    {
        public bool SanitizeResponse { get; }
        public string Command { get; set; } = "gamemode";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Testing only!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                Force(-1, false);
                response = "doing mode ignore";
                return true;
            }

            if (arguments.Count == 1)
            {
                Force(arguments.FirstElement(), false);
            }

            if(arguments.Count == 2)
            {
                if (arguments.At(1) == "force")
                {
                    Force(arguments.FirstElement(), true);
                }
            }

            response = $"Force Starting Mode {arguments.FirstElement()}";

            return true;
        }

        private void Force(string arguments, bool force)
        {
            Plugin.IsActiveEventround = true;

            GamemodeManager.GetGamemode(arguments).Enable(force);
        }

        private void Force(int arguments, bool force)
        {
            Plugin.IsActiveEventround = true;

            GamemodeManager.GetGamemode(arguments).Enable(force);
#warning SHOULD ATTEMPT ROUND HERE
        }
    }
}
