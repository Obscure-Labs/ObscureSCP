using CommandSystem;
using Exiled.API.Features;
using ObscureLabs.Gamemode_Handler;
using SpireSCP.GUI.API.Features;
using System;

namespace ObscureLabs.Commands.Other
{

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class setNext : ICommand
    {
        public string Command { get; set; } = "nextMode";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Testing only!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var gameMode = GamemodeHandler.ReadGameMode();

            if (gameMode.IsGameModeRound)
            {
                response = "Forcing next round to minigame";
                return false;
            }

            GamemodeHandler.WriteAllGameModeData(gameMode.IsGameModeRound, gameMode.LastGameMode, true);

            foreach (var p in Player.List)
            {
                Manager.SendHint(p, "<b><color=green>THE NEXT ROUND WILL BE A MINIGAME ROUND</color></b> \n", 5);
            }

            response = "Force Starting Mode";
            return false;
        }
    }
}
