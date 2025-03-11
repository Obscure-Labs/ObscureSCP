﻿using ObscureLabs.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomPlayerEffects;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using InventorySystem;
using InventorySystem.Items.Keycards;
using KeycardPermissions = Interactables.Interobjects.DoorUtils.KeycardPermissions;
using Interactables.Interobjects.DoorUtils;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public static class Extensions
    {

        public static bool HasKeycardPermission(
            this Player player,
            KeycardPermissions permissions,
            bool requiresAllPermissions = false)
        {
            if (player.IsEffectActive<AmnesiaVision>())
                return false;

            return requiresAllPermissions
                ? player.Items.Any(item => item is Keycard keycard && keycard.Base.Permissions.HasFlag(permissions))
                : player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & permissions) != 0);
        }
    }

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
            Log.Info("Interacting door");
                if (ev.Player.HasKeycardPermission(ev.Door.RequiredPermissions.RequiredPermissions) &&
                    !ev.Door.IsLocked)
                {
                    ev.IsAllowed = true;
                    ev.Door.IsOpen = !ev.Door.IsOpen;

                }
                else { Log.Warn("Smth wrong"); }
        }

        private void OnInteractinglocker(InteractingLockerEventArgs ev)
        {
#pragma warning disable CS0618
            if (ev.Player.HasKeycardPermission((KeycardPermissions)ev.InteractingChamber.RequiredPermissions)) ;
#pragma warning restore CS0618
            {
                ev.IsAllowed = true;
                ev.InteractingChamber.IsOpen = !ev.InteractingChamber.IsOpen;
                
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
                ev.Generator.IsOpen = !ev.Generator.IsOpen;
            }
        }


    }
}
