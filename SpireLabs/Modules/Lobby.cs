using ObscureLabs.API.Features;
using Player = Exiled.API.Features.Player;
using UnityEngine;
using PlayerRoles;
using Exiled.API.Enums;
using Exiled.Events.EventArgs.Player;
using SpireSCP.GUI.API.Features;
using PlayerRoles.FirstPersonControl;
using System.Runtime.CompilerServices;
namespace ObscureLabs.Modules
{
    internal class Lobby : Module
    {
        public override string Name => "Lobby";

        public override bool IsInitializeOnStart => true;


        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += WaitingForPlayers;
            Exiled.Events.Handlers.Player.Joined += PlayerJoin;
            return base.Enable();
        }

        public override bool Disable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= WaitingForPlayers;
            Exiled.Events.Handlers.Player.Joined -= PlayerJoin;
            return base.Disable();
        }


        public static void PlayerJoin(JoinedEventArgs ev)
        {
            if (Exiled.API.Features.Round.IsLobby)
            {
                if (ev.Player != null && ev.Player.Role.Type == RoleTypeId.Spectator)
                {
                    Manager.SendHint(ev.Player, "You have been sent to the pregame lobby, Waiting for players!", 5);
                    ev.Player.RoleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    ev.Player.Teleport(RoomType.LczArmory);
                    
                }
            }
            return;
        }

        public static void WaitingForPlayers()
        {
            GameObject.Find("StartRound").transform.localScale = Vector3.zero;
            foreach (Player p in Player.List)
            {
                if (p != null && p.Role.Type == RoleTypeId.Spectator)
                {
                    
                    Manager.SendHint(p, "You have been sent to the pregame lobby, Waiting for players!", 5);
                    p.RoleManager.ServerSetRole(RoleTypeId.Tutorial, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
                    p.Teleport(RoomType.LczArmory);
                }
            }
        }

    }
}
