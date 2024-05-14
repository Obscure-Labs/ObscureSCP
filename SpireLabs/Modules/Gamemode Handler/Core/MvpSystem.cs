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
                Exiled.Events.Handlers.Player.Hurting += killingPlayer;
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
                Exiled.Events.Handlers.Player.Hurting -= killingPlayer;
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
            if (ev.Player.IsCuffed)
            {
                Timing.RunCoroutine(addXPtoPlayer(ev.Player.Cuffer.UserId, 5, "Recruiting Another Player"));
                Timing.RunCoroutine(addXPtoPlayer(ev.Player.UserId, 2, "Escaping as a prisoner"));
            }
            else
            {
                Timing.RunCoroutine(addXPtoPlayer(ev.Player.UserId, 3, "Escaping Facility"));
            }


        }

        public static void unlockingGenerator(UnlockingGeneratorEventArgs ev)
        {
            Timing.RunCoroutine(addXPtoPlayer(ev.Player.UserId, 2, "Unlocking Generator"));
        }

        public static void killingPlayer(HurtingEventArgs ev)
        {
            if (ev.Amount >= ev.Player.Health) { Timing.RunCoroutine(addXPtoPlayer(ev.Attacker.UserId, 5, "Killing Enemy Player")); }
        }

        public static void endingRound(EndingRoundEventArgs ev)
        {
            _PlayerData = _PlayerData.OrderByDescending(p => p.xp).ToList();
            //Log.Warn($"{_PlayerData.ToArray()[0].player.DisplayNickname} . {_PlayerData.ToArray()[0].xp}");
            PlayerDataInstance[] PlayerData = _PlayerData.ToArray();
            if (PlayerData.Count() == 0) return;
            if (PlayerData.Count() == 1)
            {
                foreach (Player p in Plugin.PlayerList)
                {
                    Manager.SendHint(p, $"This rounds MVP was: {PlayerData[0].player.DisplayNickname} with {PlayerData[0].xp} points!", 10);
                }
            }
            if (PlayerData.Count() == 2)
            {
                foreach (Player p in Plugin.PlayerList)
                {
                    Manager.SendHint(p, $"This rounds MVP was: {PlayerData[0].player.DisplayNickname} with {PlayerData[0].xp} points!\n#2 was: {PlayerData[1].player.DisplayNickname} with {PlayerData[1].xp}", 10);
                }
            }
            if (PlayerData.Count() >= 3)
            {
                foreach (Player p in Plugin.PlayerList)
                {
                    Manager.SendHint(p, $"This rounds MVP was: {PlayerData[0].player.DisplayNickname} with {PlayerData[0].xp} points!\n#2 was: {PlayerData[1].player.DisplayNickname} with {PlayerData[1].xp}\n#3 was: {PlayerData[2].player.DisplayNickname} with {PlayerData[2].xp}", 10);
                }
            }

        }

        public static IEnumerator<float> addXPtoPlayer(string p, int xp, string reason)
        {
            yield return Timing.WaitForOneFrame;
                if (p != null && p != null && reason != null)
                _PlayerData.FirstOrDefault(x => x.player.UserId == p).xp += xp;
                Manager.SendHint(Player.Get(p), $"You Gained {xp} points for: {reason}", 5);
        }
    }
}


