using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using MEC;
using SpireSCP.GUI.API.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObscureLabs.Modules.Gamemode_Handler.Core
{
    internal class MvpSystem : Plugin.Module
    {
        public override string name { get; set; } = "MVPSystem";
        public override bool initOnStart { get; set; } = true;

        public override bool Init()
        {
            try
            {
                Exiled.Events.Handlers.Server.RoundStarted += roundStarted;
                Exiled.Events.Handlers.Player.Escaping += escaping;
                Exiled.Events.Handlers.Player.KillingPlayer += killingPlayer;
                Exiled.Events.Handlers.Player.UnlockingGenerator += unlockingGenerator;
                Exiled.Events.Handlers.Server.EndingRound += endingRound;
                base.Init();
                return true;
            }
            catch { return false; }
        }

        public override bool Disable()
        {
            try
            {
                _PlayerData.Clear();
                firstPlace = null;
                secondPlace = null;
                thirdPlace = null;
                Exiled.Events.Handlers.Server.RoundStarted -= roundStarted;
                Exiled.Events.Handlers.Player.Escaping -= escaping;
                Exiled.Events.Handlers.Player.KillingPlayer -= killingPlayer;
                Exiled.Events.Handlers.Player.UnlockingGenerator -= unlockingGenerator;
                Exiled.Events.Handlers.Server.EndingRound -= endingRound;
                base.Disable();
                return true;
            }
            catch { return false; };
        }

        public class PlayerDataInstance
        {
            public Player player { get; set; }
            public int xp { get; set; }

        }

        static List<PlayerDataInstance> _PlayerData = new List<PlayerDataInstance>();
        static PlayerDataInstance firstPlace;
        static PlayerDataInstance secondPlace;
        static PlayerDataInstance thirdPlace;
        public static void roundStarted()
        {
            foreach (Player p in Plugin.PlayerList)
            {
                _PlayerData.Add(new PlayerDataInstance
                {
                    player = p,
                    xp = 0
                });
            }
        }



        public static void escaping(EscapingEventArgs ev)
        {
            Timing.RunCoroutine(addXPtoPlayer(ev.Player.NetworkIdentity, 3, "Escaping Facility"));

        }

        public static void unlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            Timing.RunCoroutine(addXPtoPlayer(ev.Player.NetworkIdentity, 2, "Unlocking Generator"));
        }

        public static void killingPlayer(KillingPlayerEventArgs ev)
        {
            Timing.RunCoroutine(addXPtoPlayer(ev.Player.NetworkIdentity, 5, "Killing Enemy Player"));
        }

        public static void endingRound(EndingRoundEventArgs ev)
        {
            _PlayerData.OrderBy(p => p.xp);
            if (_PlayerData.ElementAt(0) == null) { return; }
            firstPlace = _PlayerData.ElementAt(0);
            if (_PlayerData.ElementAt(1) == null) { return; }
            secondPlace = _PlayerData.ElementAt(1);
            if (_PlayerData.ElementAt(2) == null) { return; }
            thirdPlace = _PlayerData.ElementAt(2);
            foreach (Player p in Plugin.PlayerList)
            {
                p.Broadcast(5, $"This round's #1 MVP was: {firstPlace.player.DisplayNickname} with: {firstPlace.xp} points! \n#2 was: {secondPlace.player.DisplayNickname} with: {secondPlace.xp} points!\n#2 was: {thirdPlace.player.DisplayNickname} with: {thirdPlace.xp} points!", shouldClearPrevious: true);
            }
        }

        public static IEnumerator<float> addXPtoPlayer(Mirror.NetworkIdentity p, int xp, string reason)
        {
            yield return Timing.WaitForOneFrame;
                if (!p == null && !p == null && reason != null)
                _PlayerData.FirstOrDefault(x => x.player.NetworkIdentity == p).xp += xp;
                Manager.SendHint(Player.Get(p), $"You Gained {xp} points for: {reason}", 5);
        }
    }
}


