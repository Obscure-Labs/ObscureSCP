using Exiled.Events.EventArgs.Player;
using MEC;
using PlayerRoles;
using SpireLabs.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObscureLabs;

namespace SpireLabs.GUI
{
    internal class GUIController : Plugin.Module
    {
        public override string name { get; set; } = "GuiController";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Server.WaitingForPlayers += restarting;
                Exiled.Events.Handlers.Player.Joined += OnPlayerJoined;
                Exiled.Events.Handlers.Player.Verified += JoinMSG;
                Exiled.Events.Handlers.Player.Left += LeaveMSG;
                Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
                Exiled.Events.Handlers.Player.Hurt += playerShot;
                Exiled.Events.Handlers.Player.Spawning += spawning;
                Exiled.Events.Handlers.Server.RestartingRound += restarting;
                base.Init();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override bool Disable()
        {
            guiHandler.peenNutMSG = new string[60];
            guiHandler.killLoop = false;
            guiHandler.joinLeave = string.Empty;
            guiHandler.hint = new string[60];
            Exiled.Events.Handlers.Player.Joined -= OnPlayerJoined;
            Exiled.Events.Handlers.Player.Verified -= JoinMSG;
            Exiled.Events.Handlers.Player.Left -= LeaveMSG;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
            Exiled.Events.Handlers.Player.Hurt -= playerShot;
            Exiled.Events.Handlers.Player.Spawning -= spawning;
            base.Disable();
            return true;
        }

        private void restarting()
        {
            guiHandler.peenNutMSG = new string[60];
            guiHandler.killLoop = false;
            guiHandler.joinLeave = string.Empty;
            guiHandler.hint = new string[60];
        }

        private void spawning(SpawningEventArgs ev)
        {
            guiHandler.peenNutMSG[ev.Player.Id] = "\t";
        }

        private void playerShot(HurtEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Scp173 && ev.Player.Health < 1000)
            {
                guiHandler.peenNutMSG[ev.Player.Id] = $"You become enraged.. You can now use breakneck to kill!";
            }
        }

        private void OnPlayerJoined(JoinedEventArgs ev)
        {
            Timing.RunCoroutine(guiHandler.displayGUI(ev.Player));
        }

        private void JoinMSG(VerifiedEventArgs ev)
        {
            Timing.RunCoroutine(guiHandler.sendJoinLeave(ev.Player, 'j'));
        }

        private void LeaveMSG(LeftEventArgs ev)
        {
            Timing.RunCoroutine(guiHandler.sendJoinLeave(ev.Player, 'l'));
        }
        private void OnRoundStart()
        {
            guiHandler.peenNutMSG = new string[60];
            guiHandler.killLoop = false;
            guiHandler.joinLeave = string.Empty;
            guiHandler.hint = new string[60];
            guiHandler.startHints();
            guiHandler.fillPeenNutMSG();
        }
    }
}
