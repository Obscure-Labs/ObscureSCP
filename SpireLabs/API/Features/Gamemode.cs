using Exiled.API.Features;
using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Features
{
    public abstract class Gamemode
    {
        public abstract string Name { get; }
        public abstract List<Module> InitModules { get; }
        public abstract List<Module> StartModules { get; }

        public virtual unsafe bool PreInitialise()
        {
            foreach(Module module in InitModules)
            {
                Plugin.Instance._modules.AddModule(&module);
                Plugin.Instance._modules.GetModule(module.Name).Enable();
            }
            return true;
        }

        public virtual unsafe bool Start()
        {
            LabApi.Features.Wrappers.Server.FriendlyFire = false;
            foreach (Module module in StartModules)
            {
                Plugin.Instance._modules.AddModule(&module);
                Plugin.Instance._modules.GetModule(module.Name).Enable();
            }
            return true;
        }

        public virtual unsafe bool Stop()
        {
            foreach (Module module in StartModules)
            {
                try { Plugin.Instance._modules.GetModule(module.Name).Disable(); }
                catch (Exception ex)
                {
                    Log.Error($"[GAMEMODE STOP] Module {module.Name} failed to stop: {ex}");
                    return false;
                }
            }
            foreach (Module module in InitModules)
            {
                try { Plugin.Instance._modules.GetModule(module.Name).Disable(); }
                catch (Exception ex)
                {
                    Log.Error($"[GAMEMODE STOP] Module {module.Name} failed to stop: {ex}");
                    return false;
                }
            }
            return true;
        }
    }
}
