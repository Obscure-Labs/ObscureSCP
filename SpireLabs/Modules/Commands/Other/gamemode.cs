using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using ObscureLabs.Gamemode_Handler;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Commands.Other
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
                gamemodeHandler.AttemptGMRound(true);
                Plugin.IsActiveEventround = true;
                Round.Start();
                response = "Force Starting Mode";
                return false;
            }
            else response = "lol no";

            return false;
        }
    }

    [CommandHandler(typeof(ClientCommandHandler))]
    public class setNext : ICommand
    {
        public setNext() => LoadGeneratedCommands();
        public string Command { get; set; } = "nextMode";
        public string[] Aliases { get; set; } = new string[] { };
        public string Description { get; set; } = "Testing only!";
        public void LoadGeneratedCommands() { }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {

            if (((CommandSender)sender).CheckPermission("*"))
            {
                if (gamemodeHandler.ReadNext()) { response = "Forcing next round to minigame"; return false; }
                else
                {
                    gamemodeHandler.WriteAllGMInfo(gamemodeHandler.ReadLast(), gamemodeHandler.ReadMode(), true);
                    foreach (Player p in Player.List)
                    {
                        Manager.SendHint(p, "<b><color=green>THE NEXT ROUND WILL BE A MINIGAME ROUND</color></b> \n", 5);
                    }

                    response = "Force Starting Mode";
                    return false;
                }
            }
            else response = "lol no";

            return false;
        }
    }
}
