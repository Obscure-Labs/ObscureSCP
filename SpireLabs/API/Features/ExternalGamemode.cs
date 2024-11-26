using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.API.Features
{
    public partial class ExternalGamemode
    {
        public static bool CheckGamemode()
        {
            return Plugin.IsActiveEventround;
        }
    }
}
