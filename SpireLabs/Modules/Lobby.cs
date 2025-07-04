﻿using ObscureLabs.API.Features;
using Player = Exiled.API.Features.Player;
using UnityEngine;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using SpireSCP.GUI.API.Features;
using PlayerRoles.FirstPersonControl;
using System.Runtime.CompilerServices;
using Exiled.API.Features;
using LabApi.Events.Arguments.ServerEvents;
using System.Linq;
using Exiled.API.Extensions;
using System.Collections.Generic;
using MEC;
using System;
using Exiled.API.Features.Roles;
using HumanRole = Exiled.API.Features.Roles.HumanRole;
using CommandSystem.Commands.Console;
using MapGeneration;
using ObscureLabs.Modules.Gamemode_Handler;

namespace ObscureLabs.Modules
{
    internal class Lobby : Module
    {
        public override string Name => "Lobby";

        
        public override bool IsInitializeOnStart => true;

        public LabApi.Features.Wrappers.Room room = LabApi.Features.Wrappers.Room.List.GetRandomValue();
        public override bool Enable()
        {

            Exiled.Events.Handlers.Server.WaitingForPlayers += WaitingForPlayers;
            LabApi.Events.Handlers.ServerEvents.RoundStarting += RoundStarting;
            Exiled.Events.Handlers.Player.Joined += PlayerJoin;
            LabApi.Events.Handlers.ServerEvents.MapGenerated += MapGenerated;
            Exiled.Events.Handlers.Player.InteractingElevator += InteractLift;
            Exiled.Events.Handlers.Player.InteractingLocker += InteractLocker;
            Exiled.Events.Handlers.Player.InteractingDoor += Interact;
            Exiled.Events.Handlers.Player.PickingUpItem += PickingUpItem;
            return base.Enable();

            
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
            Exiled.Events.Handlers.Player.InteractingDoor -= Interact;
            Exiled.Events.Handlers.Player.InteractingElevator -= InteractLift;
            Exiled.Events.Handlers.Player.InteractingLocker -= InteractLocker;
            Exiled.Events.Handlers.Player.Joined -= PlayerJoin;
            LabApi.Events.Handlers.ServerEvents.MapGenerated -= MapGenerated;
            LabApi.Events.Handlers.ServerEvents.RoundStarting -= RoundStarting;
            Exiled.Events.Handlers.Player.PickingUpItem -= PickingUpItem;
            return base.Disable();
        }


        public void PickingUpItem(PickingUpItemEventArgs ev)
        {
            if (Round.IsLobby)
            {
                ev.IsAllowed = false;
            }
        }


        public void Interact(InteractingDoorEventArgs ev)
        {
            if (Round.IsLobby)
            {
                ev.CanInteract = false;
            }
            else
            {
                Debug.Log("Why is this triggering");
                return;
            }
        }

        public void InteractLift(InteractingElevatorEventArgs ev)
        {
            if (Round.IsLobby)
            {
                ev.IsAllowed = false;
            }
            else
            {
                return;
            }
        }

        public void InteractLocker(InteractingLockerEventArgs ev)
        {
            if (Round.IsLobby)
            {
                ev.IsAllowed = false;
            }
            else
            {
                return;
            }
        }

        public void MapGenerated(MapGeneratedEventArgs ev)
        {
            room = LabApi.Features.Wrappers.Room.List.GetRandomValue();
            if (room == null)
            {
                LabApi.Features.Wrappers.Room.Get(RoomName.LczClassDSpawn);
            }
            // room = Room.List.GetRandomValue();
            // if (room == null)
            // {
            //     room = Room.Get(RoomType.LczClassDSpawn);
            // }
            // Log.Info(room.name);

        }
        public void RoundStarting(RoundStartingEventArgs args)
        {
            foreach (Player p in Player.List)
            {
                Map.CleanAllRagdolls();
                p.IsGodModeEnabled = false;
                p.RoleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.None);
                p.Transform.position = Vector3.zero;
            }
            base.Disable();
        }
        public void PlayerJoin(JoinedEventArgs ev)
        {
            if (Exiled.API.Features.Round.IsLobby)
            {
                if (ev.Player != null)
                {
                    Manager.SendHint(ev.Player, "You have been sent to the pregame lobby, Waiting for players!", 5);
                    // ev.Player.IsGodModeEnabled = true;
                    ev.Player.RoleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    ev.Player.Teleport(new Vector3(0, 295, -8) + (Vector3.up / 2));

                }
            }


        }

        public IEnumerator<float> Hint()
        {
            while (Round.IsLobby)
            {
                if (Round.LobbyWaitingTime > -1f)
                {
                    foreach (Player p in Player.List)
                    {
                        Manager.SendHint(p, $"Next Mode: <color=#7df229>{((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.Name}</color>" +
                            $"\n<color=#e8ed87>Current players: {Player.List.Count()}</color>" +
                            $"\nStarting in: <color=#7df229>{Round.LobbyWaitingTime} seconds</color>", 2f);

                    }
                }
                else
                {
                    foreach (Player p in Player.List)
                    {
                        Manager.SendHint(p, $"Next Mode: <color=#7df229>{((GamemodeManager)Plugin.Instance._modules.GetModule("GamemodeManager")).selectedGamemode.Name}</color>" +
                            $"\n<color=#e8ed87>Current Players: {Player.List.Count()}</color>" +
                            $"\n<color=red>Waiting Paused...</color>", 2f);
                    }
                }

                    yield return Timing.WaitForSeconds(1f);
            }
        }

            public void WaitingForPlayers()
        {
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;

            Timing.RunCoroutine(Hint());
            foreach (Player p in Player.List)
            {

                if (p != null && p.Role.Type == RoleTypeId.Spectator)
                {
                    p.RoleManager.ServerSetRole(RoleTypeId.ClassD, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    p.Teleport(room.Position + (Vector3.up / 2));
                    p.IsGodModeEnabled = true;
                }
            }
        }

    }
}
