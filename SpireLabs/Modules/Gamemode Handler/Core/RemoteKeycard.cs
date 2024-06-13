using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem;
using InventorySystem.Items.Keycards;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class RemoteKeycard : Module
    {
        public override string Name => "RemoteKeycard";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInteractinglocker;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnInteractingWarhead;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractinglocker;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnInteractingWarhead;
            base.Disable();
            return true;
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            List<DoorType> illegalDoors = new List<DoorType>
            {
                DoorType.Scp079First,
                DoorType.Scp079Second,
                DoorType.Scp939Cryo,
            };
            if(ev.Door.IsLocked) { ev.IsAllowed = false;}

            if (illegalDoors.Contains(ev.Door.Type)) { ev.IsAllowed = false;}

            if (!ev.Player.Items.Any(i => i is Keycard k && k.Base.Permissions.HasFlag(ev.Door.KeycardPermissions)))
            {
                ev.IsAllowed = false;
            }
        }

        private void OnInteractinglocker(InteractingLockerEventArgs ev)
        {
            if (ev.Player.Items.Any(i => i is Keycard k && k.Base.Permissions.HasFlag(ev.Chamber.RequiredPermissions)))
            {
                ev.IsAllowed = true;
            }
            else
            {
                ev.IsAllowed = false;
            }
        }

        private void OnInteractingWarhead(ActivatingWarheadPanelEventArgs ev)
        {
            if (!ev.Player.Items.Any(i => i is Keycard k && k.Base.Permissions.HasFlag(KeycardPermissions.AlphaWarhead)))
            {
                ev.IsAllowed = false;
            }
        }
    }
}
