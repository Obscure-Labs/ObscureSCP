using Exiled.API.Features;

namespace ObscureLabs.API.Features
{
    public abstract class Module
    {
        public abstract string Name { get; }

        public abstract bool IsInitializeOnStart { get; }

        public virtual bool Enable()
        {
            Log.Info($"Module {Name} Initializeialized successfully");
            return true;
        }

        public virtual bool Disable()
        {
            Log.Info($"Module {Name} Disabled successfully");
            return true;
        }
    }
}
