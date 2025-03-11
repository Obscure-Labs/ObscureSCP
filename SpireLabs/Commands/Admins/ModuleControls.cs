using CommandSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Commands.Admins
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class Module : ICommand
    {
        public string Command => "module";

        public string[] Aliases => new string[] { "mod" };

        public string Description => "Module controls";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count == 0)
            {
                response =
                    "Usage: module <enable/disable/restart> <module name>\nModule name is case sensitive.\nExample: module restart LightHandler";
                return true;
            }

            string action = arguments.At(0);
            string moduleName = arguments.Count > 1 ? arguments.At(1) : null;

            if (moduleName != null)
            {
                switch (action.ToLower())
                {
                    case "enable":
                    {
                        var module = Plugin.Instance._modules.GetModule(moduleName);
                        if (module == null)
                        {
                            response = $"Module {moduleName} not found.";
                            return true;
                        }

                        if (module.Enable())
                        {
                            response = $"Module {moduleName} enabled successfully.";
                            return true;
                        }
                        else
                        {
                            response = $"Module {moduleName} failed to enable.";
                            return true;
                        }
                    }
                    case "disable":
                    {
                        var module = Plugin.Instance._modules.GetModule(moduleName);
                        if (module == null)
                        {
                            response = $"Module {moduleName} not found.";
                            return true;
                        }

                        if (module.Disable())
                        {
                            response = $"Module {moduleName} disabled successfully.";
                            return true;
                        }
                        else
                        {
                            response = $"Module {moduleName} failed to disable.";
                            return true;
                        }
                    }
                    case "restart":
                    {
                        var module = Plugin.Instance._modules.GetModule(moduleName);
                        if (module == null)
                        {
                            response = $"Module {moduleName} not found.";
                            return true;
                        }

                        if (module.Disable() && module.Enable())
                        {
                            response = $"Module {moduleName} restarted successfully.";
                            return true;
                        }
                        else
                        {
                            response = $"Module {moduleName} failed to restart.";
                            return true;
                        }
                    }
                }
            }
            else
            {
                switch (action)
                {
                    case "list":
                    {
                        response = "Modules:\n";
                        foreach (var module in Plugin.Instance._modules.Modules)
                        {
                            response += $"{module.Name}\n";
                        }

                        return true;
                    }
                    case "refresh":
                    {
                        Plugin.Instance._modules.RefreshModuleFolder();
                        response = "Module folder refreshed.";
                        return true;
                    }
                }
            }

            response = $"Invalid action. Send the following to a developer in ObscureLabs.\n\nAction={action}\nModule={moduleName ?? "null"}\nPlugin instance version: {Plugin.Instance.Version}\nResponse from ModuleManager: {Plugin.Instance._modules.GetModule(moduleName).Name ?? "Module invalid. Error elsewhere."}";
            return false;
        }
    }
}
