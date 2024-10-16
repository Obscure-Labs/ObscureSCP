using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Data
{
    internal class AbilityData
    {
        public string Name { get; }
        public bool UseInstantly { get; }

        public virtual void Use()
        {

        }
    }
}
