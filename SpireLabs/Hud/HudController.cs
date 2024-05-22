using Exiled.Events.EventArgs.Player;
using MEC;
using ObscureLabs.API.Features;
using PlayerRoles;

namespace SpireLabs.GUI
{
    internal class HudController : Module
    {
        public override string Name => "GuiController";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnRestarting;
            Exiled.Events.Handlers.Player.Joined += OnJoined;
            Exiled.Events.Handlers.Player.Verified += OnVerified;
            Exiled.Events.Handlers.Player.Left += OnLeft;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Player.Hurt += OnHurt;
            Exiled.Events.Handlers.Player.Spawning += OnSpawning;
            Exiled.Events.Handlers.Server.RestartingRound += OnRestarting;

            return base.Enable();
        }

        public override bool Disable()
        {
            Timing.KillCoroutines("guiRoutine");
            HudHandler.peenNutMSG = new string[60];
            HudHandler.killLoop = false;
            HudHandler.joinLeave = string.Empty;
            HudHandler.hint = new string[60];
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnRestarting;
            Exiled.Events.Handlers.Player.Joined -= OnJoined;
            Exiled.Events.Handlers.Player.Verified -= OnVerified;
            Exiled.Events.Handlers.Player.Left -= OnLeft;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Player.Hurt -= OnHurt;
            Exiled.Events.Handlers.Player.Spawning -= OnSpawning;
            Exiled.Events.Handlers.Server.RestartingRound -= OnRestarting;

            return base.Disable();
        }

        private void OnRestarting()
        {
            HudHandler.peenNutMSG = new string[60];
            HudHandler.killLoop = false;
            HudHandler.joinLeave = string.Empty;
            HudHandler.hint = new string[60];
        }

        private void OnSpawning(SpawningEventArgs ev)
        {
            HudHandler.peenNutMSG[ev.Player.Id] = "\t";
        }

        private void OnHurt(HurtEventArgs ev)
        {
            if (ev.Player.Role == RoleTypeId.Scp173 && ev.Player.Health < 1000)
            {
                HudHandler.peenNutMSG[ev.Player.Id] = $"You become enraged.. You can now use breakneck to kill!";
            }
        }

        private void OnJoined(JoinedEventArgs ev)
        {
            //Debug.Log("GUI HANDLER SAYS: Player joined");
            Timing.RunCoroutine(HudHandler.displayGUI(ev.Player), "guiRoutine");
        }

        private void OnVerified(VerifiedEventArgs ev)
        {
            Timing.RunCoroutine(HudHandler.SendJoinOrLeaveCoroutine(ev.Player, false));
        }

        private void OnLeft(LeftEventArgs ev)
        {
            Timing.RunCoroutine(HudHandler.SendJoinOrLeaveCoroutine(ev.Player, true));
        }

        private void OnRoundStarted()
        {
            HudHandler.peenNutMSG = new string[60];
            HudHandler.killLoop = false;
            HudHandler.joinLeave = string.Empty;
            HudHandler.hint = new string[60];
            HudHandler.startHints();
            HudHandler.FillPeenNutMessage();
        }
    }
}
