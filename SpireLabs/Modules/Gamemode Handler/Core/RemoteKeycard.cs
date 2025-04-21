using ObscureLabs.API.Features;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Keycards;
using LabApi.Events.Arguments.PlayerEvents;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public static class Extensions
    {

        // public static bool HasKeycardPermission(
        //     this Player player,
        //     KeycardPermissions permissions,
        //     bool requiresAllPermissions = false)
        // {
        //     if (player.IsEffectActive<AmnesiaVision>())
        //         return false;
        //
        //     return requiresAllPermissions
        //         ? player.Items.Any(item => item is Keycard keycard && keycard.Base.Permissions.HasFlag(permissions))
        //         : player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & permissions) != 0);
        // }
    }

    public class RemoteKeycard : Module
    {
        public override string Name => "RemoteKeycard";

        public override bool IsInitializeOnStart => false;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor += OnInteractingDoor;
            Exiled.Events.Handlers.Player.ThrownProjectile += Gnade;
            //Exiled.Events.Handlers.Player.InteractingLocker += OnInteractinglocker;
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker += OnInteractingLabLocker;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnInteractingWarhead;
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnInteractGenerator;
            base.Enable();
            return true;
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Player.InteractingDoor -= OnInteractingDoor;
            Exiled.Events.Handlers.Player.ThrownProjectile -= Gnade;
            //Exiled.Events.Handlers.Player.InteractingLocker -= OnInteractinglocker;
            LabApi.Events.Handlers.PlayerEvents.InteractingLocker -= OnInteractingLabLocker;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnInteractingWarhead;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnInteractGenerator;
            base.Disable();
            return true;
        }

        private void Gnade(ThrownProjectileEventArgs e)
        {
            Log.Info("Grenade thrown");
        }
        private void OnInteractingLabLocker(PlayerInteractingLockerEventArgs ev)
        {
            Log.Info("Lab locker interacted with");
            if(ev.Player.Items.Where(x => x.Category == ItemCategory.Keycard).FirstOrDefault(x => x.Base is KeycardItem k && k.Permissions.HasFlag(ev.Chamber.RequiredPermissions)) != null)
            {
                Log.Info("suitable keycard found");
                ev.CanOpen = true;
                ev.IsAllowed = true;
            }
            //if(ev.Player.Items.Where(x => x is LabApi.Features.Wrappers.KeycardItem keycard && keycard.Base.Permissions.HasFlag(ev.Chamber.RequiredPermissions)).First() != null)
            //{
            //    ev.CanOpen = true;
            //    ev.IsAllowed = true;
            //}
            //if(ev.Player.CurrentItem.Base is KeycardItem keycard && keycard.Permissions.HasFlag(ev.Chamber.RequiredPermissions))
            //{
            //    ev.CanOpen = true;
            //    ev.IsAllowed = true;
            //}
        }

        private void OnInteractingDoor(InteractingDoorEventArgs ev)
        {
            Log.Info("Interacting door");
            if (ev.Door.IsKeycardDoor)
            {
                if(ev.Player.CurrentItem is Keycard keycard && keycard.Permissions.HasFlag(ev.Door.RequiredPermissions.RequiredPermissions) && !ev.Door.IsLocked)
                {
                    ev.Door.IsOpen = !ev.Door.IsOpen;
                    ev.IsAllowed = true;
                }
                // else if(ev.Player.Items.Where(x => x is Keycard))
                // else if (ev.Player.HasKeycardPermission(ev.Door.RequiredPermissions.RequiredPermissions) &&
                //     !ev.Door.IsLocked)
                // {
                //     ev.Door.IsOpen = !ev.Door.IsOpen;
                //     ev.IsAllowed = true;
                // }
                else
                {
                    ev.Door.PlaySound(DoorBeepType.PermissionDenied);
                }
            }
        }

        //private void OnInteractinglocker(InteractingLockerEventArgs ev)
        //{
        //    Log.Info("Interacting locker");
        //    //Log.Info($"Interacting with locker with perms {ev.InteractingChamber.RequiredPermissions}");
        //    //Log.Info($"Player has keycard permissions: {((Keycard)ev.Player.Items.FirstOrDefault(x => x.IsKeycard)).Permissions}");
        //    Log.Info($"\n Check: {ev.Player.Items.Where(x => x.IsKeycard).FirstOrDefault(x => x is Keycard k && k.Permissions.HasFlag(ev.InteractingChamber.RequiredPermissions)) != null}");
        //    if (ev.Player.Items.Where(x => x is Keycard keycard && keycard.Permissions.HasFlag(ev.InteractingChamber.RequiredPermissions)).First() != null)
        //    {
        //        ev.IsAllowed = true;
        //    }
        //    if (ev.Player.CurrentItem is Keycard keycard && keycard.Permissions.HasFlag(ev.InteractingChamber.RequiredPermissions))
        //    {
        //        ev.IsAllowed = true;
        //    }
        //}

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
                ev.Generator.IsOpen = !ev.Generator.IsOpen;
            }
        }


    }
}
