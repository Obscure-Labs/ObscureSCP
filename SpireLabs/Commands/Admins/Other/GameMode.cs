using CommandSystem;
using Exiled.API.Features;
using ObscureLabs.Gamemode_Handler;
using System;

namespace ObscureLabs.Commands.Admin.Other
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class GameMode : ICommand
    {
        public string Command { get; set; } = "gamemode";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Testing only!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = $"You must enter argument";
                return false;
            }

            if (!int.TryParse(arguments.Array[0], out var result))
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
            Round.Start();
            GamemodeHandler.AttemptGameModeRound(true, arguments);
        }
    }
}
