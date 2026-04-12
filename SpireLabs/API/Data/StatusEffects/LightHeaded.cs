using UnityEngine;
using CustomPlayerEffects;
using LabApi.Features.Wrappers;
using SpireSCP.GUI.API.Features;

namespace ObscureLabs.Modules.Gamemode_Handler.StatusEffects
{
    public class LightHeaded : StatusEffectBase
    {
        private Vector3 oldGrav;
        public override void Enabled()
        {
            base.Enabled();
            Manager.SendHint(Hub.playerStats._hub, "You're now lightheaded", 5f);
            oldGrav = Player.Get(Hub.playerStats._hub).Gravity;
            Player.Get(Hub.playerStats._hub).Gravity = oldGrav / 2;
        }

        public override void OnEffectUpdate()
        {
            base.OnEffectUpdate();
        }

        public override void Disabled()
        {
            base.Disabled();
            Player.Get(Hub.playerStats._hub).Gravity = oldGrav;
        }
    }
}
