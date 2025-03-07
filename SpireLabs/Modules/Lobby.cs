using ObscureLabs.API.Features;
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
namespace ObscureLabs.Modules
{
    internal class Lobby : Module
    {
        public override string Name => "Lobby";

        
        public override bool IsInitializeOnStart => true;

        public Room room = Room.List.GetRandomValue();
        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += WaitingForPlayers;
            LabApi.Events.Handlers.ServerEvents.RoundStarting += RoundStarting;
            Exiled.Events.Handlers.Player.Joined += PlayerJoin;
            LabApi.Events.Handlers.ServerEvents.MapGenerated += MapGenerated;
            Exiled.Events.Handlers.Player.InteractingElevator += InteractLift;
            Exiled.Events.Handlers.Player.InteractingLocker += InteractLocker;
            Exiled.Events.Handlers.Player.InteractingDoor += Interact;

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
            return base.Disable();
        }

        public void Interact(InteractingDoorEventArgs ev)
        {
            if (Round.IsLobby)
            {
                ev.CanInteract = false;
            }
            else
            {
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
            room = Room.List.GetRandomValue();
            if (room == null)
            {
                room = Room.Get(RoomType.LczClassDSpawn);
            }
            Log.Info(room.name);

        }
        public void RoundStarting(RoundStartingEventArgs args)
        {
            foreach (Player p in Player.List)
            {
                p.RoleManager.ServerSetRole(RoleTypeId.Spectator, RoleChangeReason.None);
                p.Transform.position = Vector3.zero;
            }
        }
        public void PlayerJoin(JoinedEventArgs ev)
        {
            if (Exiled.API.Features.Round.IsLobby)
            {
                if (ev.Player != null)
                {
                    Manager.SendHint(ev.Player, "You have been sent to the pregame lobby, Waiting for players!", 5);
                    ev.Player.RoleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    //ev.Player.Teleport(room.transform.position);
                    
                }
            }
            return;
        }

        
        public void WaitingForPlayers()
        {
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            foreach (Player p in Player.List)
            {
                if (p != null && p.Role.Type == RoleTypeId.Spectator)
                {
                    Log.Info(room.name);
                    Log.Info("AA");
                    Manager.SendHint(p, "You have been sent to the pregame lobby, Waiting for players!", 5);
                    p.RoleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    p.Teleport(room.Position);
                }
            }
        }

    }
}
