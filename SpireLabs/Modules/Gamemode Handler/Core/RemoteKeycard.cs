using ObscureLabs.API.Features;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Keycards;
using LabApi.Events.Arguments.PlayerEvents;
using Exiled.API.Features.Doors;
using System.Runtime.InteropServices;
using Exiled.API.Extensions;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{

    public class RemoteKeycard : Module
    {
        public override string Name => "RemoteKeycard";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker += OnInteractingLabLocker;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnInteractingWarhead;
            LabApi.Events.Handlers.PlayerEvents.InteractingGenerator += OnInteractGenerator;
            //LabApi.Events.Handlers.PlayerEvents.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker -= OnInteractingLabLocker;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnInteractingWarhead;
            LabApi.Events.Handlers.PlayerEvents.InteractingGenerator -= OnInteractGenerator;
            //LabApi.Events.Handlers.PlayerEvents.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            base.Disable();
            return true;
        }

        private void OnInteractingLabLocker(PlayerInteractingLockerEventArgs ev)
        {
            Log.Info("Lab locker interacted with");
            if(ev.Player.Items.Where(x => x.Category == ItemCategory.Keycard).FirstOrDefault(x => x.Base is KeycardItem k && k.Permissions.HasFlag(ev.Chamber.RequiredPermissions)) != null)
            {
                Log.Info("suitable keycard found");
                ev.CanOpen = true;
            }
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            Log.Info("Interacting door");
            if (ev.Door.IsKeycardDoor)
            {
                if (ev.Player.Items.Where(x => x.Category == ItemCategory.Keycard).FirstOrDefault(x => x.Base is KeycardItem k && k.Permissions.HasFlag(ev.Door.RequiredPermissions.RequiredPermissions)) != null)
                {
                    
                    ev.IsAllowed = true;
                }
                else
                {
                    ev.Door.PlaySound(DoorBeepType.PermissionDenied);
                }
            }
        }

        private void OnInteractingWarhead(ActivatingWarheadPanelEventArgs ev)
        {
            Log.Info("Generator");
            if (ev.Player.Items.Where(x => x.Category == ItemCategory.Keycard).FirstOrDefault(x => x.Base is KeycardItem k && k.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead)) != null)
            {
                Log.Info("suitable keycard found");
                ev.IsAllowed = true;
            }
        }

        private void OnInteractGenerator(PlayerInteractingGeneratorEventArgs ev)
        {
            Log.Info("Generator");
            if (ev.Player.Items.Where(x => x.Category == ItemCategory.Keycard).FirstOrDefault(x => x.Base is KeycardItem k && k.Permissions.HasFlag(ev.Generator.RequiredPermissions)) != null)
            {
                Log.Info("suitable keycard found");
                ev.IsAllowed = true;
                ev.Generator.IsUnlocked = true;

            }
        }


    }
}
