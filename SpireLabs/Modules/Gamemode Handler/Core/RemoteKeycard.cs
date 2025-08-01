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
using ObscureLabs.Extensions;
using System;

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
                    x is Keycard k && k.Permissions.HasFlag(ev.InteractingChamber.RequiredPermissions.RemoveFlags(KeycardPermissions.ScpOverride))) && ev.Player.IsHuman)
            {
                ev.IsAllowed = true;
            }
        }
        
        
        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            if (ev.Door.IsKeycardDoor && ev.Door.KeycardPermissions != KeycardPermissions.None)
            {

                foreach (DoorPermissionFlags flag in Enum.GetValues(typeof(DoorPermissionFlags)))
                {
                    if (flag == DoorPermissionFlags.None)
                        continue;

                    if (ev.Door.RequiredPermissions.HasFlag(flag))
                    {
                        Log.Info(flag);
                    }
                }




                if (ev.Player.Items.Any(x => x is Keycard k && k.Permissions.HasFlag(ev.Door.KeycardPermissions.RemoveFlags(KeycardPermissions.ScpOverride)) && ev.Player.IsHuman))
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
            if (((Exiled.API.Features.Player)ev.Player).Items.Any(x => x is Keycard k && k.Permissions.HasFlag(KeycardPermissions.AlphaWarhead)) && ev.Player.IsHuman)
            {
                ev.IsAllowed = true;
            }
            else
            {
                ev.IsAllowed = false;
            }
            
        }


        private void OnInteractGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.Items.Any(x => x is Keycard k && k.Permissions.HasFlag(KeycardPermissions.ArmoryLevelTwo)) && ev.Player.IsHuman)
            {
                ev.IsAllowed = true;
            }
            else
            {
                ev.IsAllowed = false;
            }
        }
    }
}

