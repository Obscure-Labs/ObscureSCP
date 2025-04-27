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
using Exiled.API.Features.Lockers;
using Exiled.Events.Handlers;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Distributors;
using Mirror;
using Item = Exiled.API.Features.Items.Item;
using SpireSCP.GUI.API.Features;
using KeycardItem = LabApi.Features.Wrappers.KeycardItem;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{

    public class RemoteKeycard : Module
    {
        public override string Name => "RemoteKeycard";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            //LabApi.Events.Handlers.PlayerEvents.InteractingLocker += OnInteractingLabLocker;
            Exiled.Events.Handlers.Player.InteractingLocker += OnInteractingLocker;
            //Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnInteractingWarhead;
            LabApi.Events.Handlers.PlayerEvents.UnlockingWarheadButton += OnInteractingWarhead;
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnInteractGenerator;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            //LabApi.Events.Handlers.PlayerEvents.InteractingLocker -= OnInteractingLabLocker;
            Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractingLocker;
            //Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnInteractingWarhead;
            LabApi.Events.Handlers.PlayerEvents.UnlockingWarheadButton += OnInteractingWarhead;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnInteractGenerator;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            base.Disable();
            return true;
        }

        private void OnInteractingLocker(InteractingLockerEventArgs ev)
        {
            if (ev.Player.Items.Any(x =>
                    x is Keycard k && k.Permissions.HasFlag(ev.InteractingChamber.RequiredPermissions.RemoveFlags(KeycardPermissions.ScpOverride))))
            {
                ev.IsAllowed = true;
            }
        }
        
        // private void OnInteractingLabLocker(PlayerInteractingLockerEventArgs ev)
        // {
        //     Log.Debug("Lab locker interacted with");
        //     if (ev.Player.Items.Any(x => x is KeycardItem k && k.Base.GetPermissions(ev.)))
        //     {
        //         Log.Debug("suitable keycard found"); 
        //         ev.CanOpen = true;
        //     }
        // }
        
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            Log.Debug("Interacting door");
            if (ev.Door.IsKeycardDoor)
            {
                if (ev.Player.Items.Any(x => x is Keycard k && k.Permissions.HasFlag(ev.Door.KeycardPermissions.RemoveFlags(KeycardPermissions.ScpOverride))) && !ev.Door.IsLocked)
                {
                    ev.IsAllowed = true;
                }
                else
                {
                    ev.Door.PlaySound(DoorBeepType.PermissionDenied);
                }
            }
        }

        private void OnInteractingWarhead(PlayerUnlockingWarheadButtonEventArgs ev)
        {
            Log.Debug("Warhead");
            if (((Exiled.API.Features.Player)ev.Player).Items.Any(x => x is Keycard k && k.Permissions.HasFlag(KeycardPermissions.AlphaWarhead)))
            {
                ev.IsAllowed = true;
            }
            else
            {
                ev.IsAllowed = false;
            }
            // foreach (LabApi.Features.Wrappers.Item item in ev.Player.Items.ToList())
            // {
            //     if (item.Base is not KeycardItem keycard)
            //     {
            //         Log.Debug($"{item.Type.ToString()} is not a keycard");
            //         continue;
            //     }
            //
            //     if (keycard.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead))
            //     {
            //         if (ev.Player.CurrentItem != item)
            //         {
            //             ev.IsAllowed = true;
            //             Log.Debug($"{keycard.name} DOES have the right permission");
            //         }
            //
            //     }
            //     else
            //     {
            //         Log.Debug($"{keycard.name} does not have the right permission");
            //         Log.Debug($"Keycard has the following permissions: \n{keycard.Permissions.ToString()}");
            //     }
            // }
            
        }


        private void OnInteractGenerator(UnlockingGeneratorEventArgs ev)
        {

            Log.Debug("Gemeratpr");
            if (ev.Player.Items.Any(x => x is Keycard k && k.Permissions.HasFlag(KeycardPermissions.ArmoryLevelTwo)))
            {
                ev.IsAllowed = true;
            }
            else
            {
                ev.IsAllowed = false;
            }
            // foreach (Item item in ev.Player.Items.ToList())
            // {
            //     if (item.Base is not KeycardItem keycard)
            //     {
            //         continue;
            //     }
            //
            //     if (keycard.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelTwo))
            //     {
            //         ev.IsAllowed = true;
            //         
            //         break;
            //     }
            // }
        }
    }
}

