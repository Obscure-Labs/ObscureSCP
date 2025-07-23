using CommandSystem;
using ObscureLabs.Modules.Gamemode_Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Commands.Admins
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ForceGamemode : ICommand
    {
        public string Command => "gmode";
        public string[] Aliases => new[] { "forcegamemode" };

        public string Description => "gamemode force command";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response =
                    "Usage: gamemode {gamemode name}";
                return false;
            }

            switch (arguments.At(0).ToLower())
            {
                case "insanity":
                    ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.Stop();
                    ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode = ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager"))._gamemodes.FirstOrDefault(x => x.Name == "Insanity Mode");
                    ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.PreInitialise();
                    response = $"Gamemode set to {((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.Name}";
                    return true;
                    break;
                case "standard":
                    ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.Stop();
                    ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode = ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager"))._gamemodes.FirstOrDefault(x => x.Name == "Standard Mode");
                    ((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.PreInitialise();
                    response = $"Gamemode set to {((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.Name}";
                    return true;
                    break;
                default:
                    response = "Invalid gamemode. Available gamemodes: Insanity, Standard.";
                    return false;
            }
            response = $"you fucked something up realllllllly bad";
            return false;
        }
    }
}
