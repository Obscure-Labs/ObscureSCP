using System.Collections.Generic;
using System.Linq;

namespace ObscureLabs.API.Features
{
    public static class ModulesManager
    {
        private static List<Module> _moduleList = new();

        public static List<Module> Modules => _moduleList;

        public static Module GetModule(string name)
        {
            return _moduleList.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        public static void AddModule(Module module)
        {
            _moduleList.Add(module);
        }

        public static void Clear()
        {
            _moduleList.Clear();
        }
    }
}
