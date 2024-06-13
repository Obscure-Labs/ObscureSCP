using CommandSystem;
using Exiled.API.Features;
using ObscureLabs.Gamemode_Handler;
using System;

namespace ObscureLabs.Commands.Admin.Other
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Restart : ICommand
    {
        public bool SanitizeResponse { get; }
        public string Command { get; set; } = "restart";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Testing only!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = $"You must enter argument";
                return false;
            }
            

            response = $"Force restarting";
            Server.ExecuteCommand("restart");
            return true;
        }
    }
}
