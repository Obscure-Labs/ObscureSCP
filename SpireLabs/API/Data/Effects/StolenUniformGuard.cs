using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;

namespace ObscureLabs.API.Data.Effects
{
    internal class StolenUniformGuard : EffectBase
    {
        public override string Name => "StolenUniformGuard";
        public override float Duration { get; set; } = 15f;
        public override bool IsPermanent { get; set; } = false;
        public override Player player { get; set; }

        public StolenUniformGuard(Player player, float duration = 15f)
        {
            player = player;
            Duration = duration;
            Give();
        }

        public override void Give()
        {
            player.ChangeAppearance(RoleTypeId.FacilityGuard);
            base.Give();
        }

        public override void Remove()
        {
            player.ChangeAppearance(player.Role);
            base.Remove();
        }
    }
}
