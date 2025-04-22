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
using Exiled.Events.Handlers;
using Mirror;
using Item = Exiled.API.Features.Items.Item;
using SpireSCP.GUI.API.Features;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{

    public class RemoteKeycard : Module
    {
        public override string Name => "RemoteKeycard";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker += OnInteractingLabLocker;
            //Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnInteractingWarhead;
            LabApi.Events.Handlers.PlayerEvents.UnlockingWarheadButton += OnInteractingWarhead;
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnInteractGenerator;
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker -= OnInteractingLabLocker;
            //Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnInteractingWarhead;
            LabApi.Events.Handlers.PlayerEvents.UnlockingWarheadButton += OnInteractingWarhead;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnInteractGenerator;
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            base.Disable();
            return true;
        }

        private void OnInteractingLabLocker(PlayerInteractingLockerEventArgs ev)
        {
            Log.Info("Lab locker interacted with");
            if (ev.Player.Items.Where(x => x.Category == ItemCategory.Keycard).FirstOrDefault(x => x.Base is KeycardItem k && k.Permissions.HasFlag(ev.Chamber.RequiredPermissions)) != null)
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
                foreach (Item item in ev.Player.Items.ToList())
                {
                    if (item.Base is not KeycardItem keycard)
                    {
                        continue;
                    }

                    if (keycard.Permissions.HasFlag(ev.Door.RequiredPermissions.RequiredPermissions.RemoveFlags(Interactables.Interobjects.DoorUtils.KeycardPermissions.ScpOverride)))
                    {
                        ev.IsAllowed = true;
                        break;
                    }
                    else
                    {
                        Log.Warn($"{keycard.name} does not have permission: {ev.Door.RequiredPermissions.RequiredPermissions}, keycard only has {keycard.Permissions.ToString()}");
                    }
                }
                ev.Door.PlaySound(DoorBeepType.PermissionDenied);
            }
        }

        private void OnInteractingWarhead(PlayerUnlockingWarheadButtonEventArgs ev)
        {
            Log.Info("Warhead");
            foreach (LabApi.Features.Wrappers.Item item in ev.Player.Items.ToList())
            {
                if (item.Base is not KeycardItem keycard)
                {
                    Log.Info($"{item.Type.ToString()} is not a keycard");
                    continue;
                }

                if (keycard.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead))
                {
                    if (ev.Player.CurrentItem != item)
                    {
                        ev.IsAllowed = true;
                        Log.Info($"{keycard.name} DOES have the right permission");
                    }

                }
                else
                {
                    Log.Info($"{keycard.name} does not have the right permission");
                    Log.Info($"Keycard has the following permissions: \n{keycard.Permissions.ToString()}");
                }
            }
            
        }


        private void OnInteractGenerator(UnlockingGeneratorEventArgs ev)
        {

            Log.Info("Gemeratpr");
            foreach (Item item in ev.Player.Items.ToList())
            {
                if (item.Base is not KeycardItem keycard)
                {
                    continue;
                }

                if (keycard.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.ArmoryLevelTwo))
                {
                    ev.IsAllowed = true;
                    
                    break;
                }
            }
        }
    }
}

