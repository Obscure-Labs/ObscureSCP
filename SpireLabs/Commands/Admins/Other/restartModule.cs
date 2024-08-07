using CommandSystem;
using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Commands.Admins.Other
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class restartModule : ICommand
    {
        public bool SanitizeResponse { get; }
        public string Command { get; set; } = "module";

        public string[] Aliases { get; set; } = new string[] { };

        public string Description { get; set; } = "Debug only!";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response = $"You must enter argument";
                return false;
            }

            Force(arguments.FirstElement());

            response = $"Force restarting {arguments.FirstElement()}";

            return true;
        }

        private void Force(string moduleName)
        {
            ModulesManager.GetModule(moduleName).Disable();
            ModulesManager.GetModule(moduleName).Enable();
        }
    }
}
