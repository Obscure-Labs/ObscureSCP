using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers;
using SpireSCP.GUI.API.Features;
using UnityEngine;
using static InventorySystem.Items.Firearms.ShotEvents.ShotEventManager;
namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class EmotionRandomiser : Module
    {
        public override string Name => "EmotionRandomiser";
        public override bool IsInitializeOnStart => true;
        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.Spawned += OnSpawned;
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.Spawned -= OnSpawned;
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
            return base.Disable();
        }
        
        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            ev.Player.Emotion = 
                (EmotionPresetType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EmotionPresetType)).Length);
        }

        private void OnSpawned(SpawnedEventArgs ev)
        {
            ev.Player.Emotion =
                (EmotionPresetType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(EmotionPresetType)).Length);
        }
    }
}
