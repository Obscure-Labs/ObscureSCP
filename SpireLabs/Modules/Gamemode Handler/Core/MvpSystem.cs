using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Server;
using Interactables.Interobjects.DoorUtils;
using MEC;
using ObscureLabs.API.Data;
using ObscureLabs.API.Features;
using SpireSCP.GUI.API.Features;
using System.Collections.Generic;
using System.Linq;
using Player = Exiled.API.Features.Player;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    public class MvpSystem : Module
    {
        public static List<PlayerData> _playerData = new();
        private static bool _warheadPanelUnlocked = false;

        public override string Name => "MVPSystem";

        public override bool IsInitializeOnStart => true;

        public override bool Enable()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Player.Escaping += OnEscaping;
            Exiled.Events.Handlers.Player.Died += OnKillingPlayer;
            Exiled.Events.Handlers.Player.UnlockingGenerator += OnUnlockingGenerator;
            Exiled.Events.Handlers.Server.RoundEnded += OnEndingRound;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Scp096.AddingTarget += OnAddingTarget;

            return base.Enable();
        }

        public override bool Disable()
        {
            _playerData.Clear();
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Player.Escaping -= OnEscaping;
            Exiled.Events.Handlers.Player.Died -= OnKillingPlayer;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= OnUnlockingGenerator;
            Exiled.Events.Handlers.Server.RoundEnded -= OnEndingRound;
            Exiled.Events.Handlers.Player.EscapingPocketDimension -= OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= OnActivatingWarheadPanel;
            Exiled.Events.Handlers.Scp096.AddingTarget -= OnAddingTarget;

            return base.Disable();
        }

        private void OnRoundStarted()
        {
            foreach (var player in Player.List)
            {
                _playerData.Add(new PlayerData(player, 0));
            }
        }

        private void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (!ev.IsLooking)
            {
                return;
            }

            Timing.RunCoroutine(AddXpToPlayer(ev.Player, 1, "Had a player look at your face..."));
        }

        private void OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs ev)
        {
            if (!ev.Player.Items.Any(item => item is Keycard keycard && keycard.Base.Permissions.HasFlag(KeycardPermissions.AlphaWarhead) || !Round.InProgress)) { return; }
            else if (!_warheadPanelUnlocked)
            {
                _warheadPanelUnlocked = true;
                Timing.RunCoroutine(AddXpToPlayer(ev.Player, 7, "Opening The Warhead Panel"));
            }
            else { return; }

        }

        private void OnEscapingPocketDimension(EscapingPocketDimensionEventArgs ev)
        {
            Timing.RunCoroutine(AddXpToPlayer(ev.Player, 3, "Escaping The Pocket Dimension"));
        }

        private void OnEscaping(EscapingEventArgs ev)
        {
            if (!ev.IsAllowed)
            {
                return;
            }

            if (ev.Player.IsCuffed)
            {
                Timing.RunCoroutine(AddXpToPlayer(ev.Player.Cuffer, 5, "Recruiting Another Player"));
                Timing.RunCoroutine(AddXpToPlayer(ev.Player, 2, "Escaping as a prisoner"));
            }
            else
            {
                Timing.RunCoroutine(AddXpToPlayer(ev.Player, 3, "Escaping Facility"));
            }
        }

        private void OnUnlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            if (ev.Player.Items.Any(item => item is Keycard keycard && keycard.Base.Permissions.HasFlag(KeycardPermissions.ArmoryLevelTwo)))
            {
                Timing.RunCoroutine(AddXpToPlayer(ev.Player, 2, "Unlocking Generator"));
            }
            else
            {
                return;
            }
        }

        private void OnKillingPlayer(DiedEventArgs ev)
        {
            if (ev.Attacker is null || ev.Attacker == ev.Player)
            {
                return;
            }

            if (ev.Player.IsScp)
            {
                Timing.RunCoroutine(AddXpToPlayer(ev.Attacker, 12, $"Killing SCP Player: {ev.Player.DisplayNickname}"));
            }
            else
            {
                Timing.RunCoroutine(AddXpToPlayer(ev.Attacker, 5, $"Killing Player: {ev.Player.DisplayNickname}"));
            }

        }

        private void OnEndingRound(RoundEndedEventArgs ev)
        {
            Log.Warn("ROUND ENDED");
            _playerData = _playerData.OrderByDescending(p => p.Xp).ToList();
            var playerData = _playerData.ToArray();
            var message = string.Empty;

            if (playerData.Count() == 0)
            {
                message = "There was no MVP this round :(";
            }

            if (playerData.Count() == 1)
            {
                if (playerData[0].Xp > 0)
                {
                    message += $"This rounds MVP was: {playerData[0].Player.DisplayNickname} with {playerData[0].Xp} points!";
                }
                else
                {
                    message = "There was no MVP this round :(";
                }
            }
            if (playerData.Count() == 2)
            {
                if (playerData[0].Xp > 0)
                {
                    message += $"This rounds MVP was: {playerData[0].Player.DisplayNickname} with {playerData[0].Xp} points!";
                }
                else
                {
                    message = "There was no MVP this round :(";
                }
                if (playerData[1].Xp > 0)
                {
                    message += $"#2 was: {playerData[1].Player.DisplayNickname} with {playerData[1].Xp} points!";
                }
            }
            if (playerData.Count() >= 3)
            {
                if (playerData[0].Xp > 0)
                {
                    message += $"This rounds MVP was: {playerData[0].Player.DisplayNickname} with {playerData[0].Xp} points!";
                }
                else
                {
                    message = "There was no MVP this round :(";
                }
                if (playerData[1].Xp > 0)
                {
                    message += $"#2 was: {playerData[1].Player.DisplayNickname} with {playerData[1].Xp} points!";
                }
                if (playerData[2].Xp > 0)
                {
                    message += $"#3 was: {playerData[2].Player.DisplayNickname} with {playerData[2].Xp} points!";
                }
            }

            foreach (Player p in Player.List)
            {
                Manager.SendHint(p, message, 10);
            }
        }

        private IEnumerator<float> AddXpToPlayer(Player player, int xp, string reason)
        {
            yield return Timing.WaitForOneFrame;
            if (player is null || reason is null)
            {
                yield break;
            }

            _playerData.FirstOrDefault(x => x.Player.Id == player.Id).Xp += xp;
            Manager.SendHint(player, $"You Gained <color=green><u>{xp} MVP points</u></color> for: <color=yellow>{reason}</color>\nYou currently have: <color=green><u>{_playerData.FirstOrDefault(x => x.Player.Id == player.Id).Xp} MVP points</u></color>", 5);
        }
    }
}


