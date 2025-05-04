using Exiled.API.Features;
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

        public virtual bool PreInitialise()
        {
            foreach (Module module in InitModules)
            {
                try { module.Enable(); }
                catch (Exception ex)
                {
                    Log.Error($"[GAMEMODE PREINIT] Module {module.Name} failed to start: {ex}");
                    return false;
                }
            }
            return true;
        }

        public virtual bool Start()
        {
           foreach (Module module in StartModules)
           {
                try { module.Enable(); }
                catch (Exception ex)
                {
                    Log.Error($"[GAMEMODE START] Module {module.Name} failed to start: {ex}");
                    return false;
                }
           }
           return true;
        }

        public virtual bool Stop()
        {
            foreach (Module module in StartModules)
            {
                try { module.Disable(); }
                catch (Exception ex)
                {
                    Log.Error($"[GAMEMODE STOP] Module {module.Name} failed to stop: {ex}");
                    return false;
                }
            }
            foreach (Module module in InitModules)
            {
                try { module.Disable(); }
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
