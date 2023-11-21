using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using SpireLabs.Gamemode_Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpireLabs.Commands.Other
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class JailbirdTDM : ICommand
    {
        public JailbirdTDM() => LoadGeneratedCommands();
        public string Command { get; set; } = "gamemode";
        public string[] Aliases { get; set; } = new string[] { };
        public string Description { get; set; } = "Testing only!";
        public void LoadGeneratedCommands() { }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            if (((CommandSender)sender).CheckPermission("*"))
            {
                Timing.RunCoroutine(jailBirdTDM.runJbTDM());
                Round.Start();
                response = "Force Starting Mode";
                return false;
            }
            else response = "lol no";

            return false;
        }
    }
}
