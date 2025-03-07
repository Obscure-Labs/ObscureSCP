using System.Collections.Generic;
using System.Linq;

namespace ObscureLabs.API.Features
{
    public class ModulesManager
    {
        private List<Module> _moduleList = new();

        public List<Module> Modules => _moduleList;

        public Module GetModule(string name)
        {
            return _moduleList.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
        }

        public void AddModule(Module module)
        {
            _moduleList.Add(module);
        }

        public void Clear()
        {
            _moduleList.Clear();
        }
    }
}
