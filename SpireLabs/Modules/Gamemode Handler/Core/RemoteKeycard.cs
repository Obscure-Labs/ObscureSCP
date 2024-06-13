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
using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;

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
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnInteractGenerator;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractinglocker;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnInteractingWarhead;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnInteractGenerator;
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

            if (ev.Player.Items.Any(i => i is Keycard k && k.Base.Permissions.HasFlag(ev.Door.RequiredPermissions.RequiredPermissions)) && !illegalDoors.Contains(ev.Door.Type) && !ev.Door.IsLocked || ev.Door.IsCheckpoint && ev.Player.Items.Any(i => i is Keycard k && k.Base.Permissions.HasFlag(KeycardPermissions.Checkpoints)))
            {
                ev.IsAllowed = true;
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
            if (ev.Player.Items.Any(i => i is Keycard k && k.Base.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead)))
            {
                ev.IsAllowed = true;
            }
        }

        private void OnInteractGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.Items.Any(i =>
                    i is Keycard k &&
                    k.Base.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelTwo)))
            {
                ev.IsAllowed = true;
            }
        }
    }
}
