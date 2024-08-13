using CommandSystem;
using Exiled.API.Features;
using ObscureLabs.Gamemode_Handler;
using System;

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
                Force(-1);
                response = "doing mode ignore";
                return true;
            }

            if (!int.TryParse(arguments.FirstElement(), out var result))
            {
                Force(-1);
            }
            else
            {
                Force(result);
            }

            response = $"Force Starting Mode {result}";

            return true;
        }

        private void Force(int arguments)
        {
            Plugin.IsActiveEventround = true;
            GamemodeHandler.AttemptGameModeRound(true, arguments);
        }
    }
}
