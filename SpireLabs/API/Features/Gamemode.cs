using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using ObscureLabs.Modules.Gamemode_Handler.Minigames;
using PluginAPI.Events;
using SpireLabs.GUI;
using SpireSCP.GUI.API.Features;

namespace ObscureLabs.API.Features
{
    public abstract class Gamemode
    {
        public abstract string Name { get; }
        public abstract string HintText { get; }

        public List<Player> Players { get; set; } = new();
        public List<TeamHandler.SerializableTeamData> Teams { get; set; } = new();

        public virtual void Enable(bool force)
        {
            Log.Debug($"Gamemode ({this.Name}) Enabled.");
            if (force) Start();
            else
            {
                Thread t = new Thread(() =>
                {
                    while (Round.IsLobby)
                    {
                        if (Round.LobbyWaitingTime == 1)
                        {
                            Start();
                        }
                    }
                });
                t.Start();
            }
        }

        public virtual void Start()
        {
            foreach (var p in Players)
            {
                Manager.SendHint(p, HintText, 5f);
            }
            Round.Start();
            Log.Debug($"Gamemode ({this.Name}) Round Started.");
            Exiled.Events.Handlers.Player.Joined += PlayerJoinInProgress;
            Exiled.Events.Handlers.Server.RespawningTeam += RespawnWave;
        }

        public virtual void RespawnWave(RespawningTeamEventArgs respawnEvent)
        {
            
        }

        public virtual void End()
        {
            Exiled.Events.Handlers.Player.Joined -= PlayerJoinInProgress;
            Exiled.Events.Handlers.Server.RespawningTeam -= RespawnWave;
            Log.Debug($"Gamemode ({this.Name}) Round Ended.");

        }

        public virtual IEnumerator<float> SpawnPlayer(Player player, TeamHandler.SerializableTeamData team)
        {
            yield return 0;
        }

        public virtual void PlayerJoin(Player player)
        {
            Players.Add(player);
            Log.Debug($"Gamemode ({this.Name}) Player Joined ({player.DisplayNickname}).");
        }

        public virtual void PlayerJoinInProgress(JoinedEventArgs player)
        {
            Players.Add(player.Player);
            Log.Debug($"Gamemode ({this.Name}) Player Joined In Progress ({player.Player.DisplayNickname}).");
        }

        public virtual void PlayerLeave(LeftEventArgs leftEvent)
        {
            Players.Remove(leftEvent.Player);
            Log.Debug($"Gamemode ({this.Name}) Player Left ({leftEvent.Player.DisplayNickname}).");
        }
    }
}
