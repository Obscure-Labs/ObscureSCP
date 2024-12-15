using Exiled.API.Features;
using MEC;
using ObscureLabs.API.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;

namespace SpireLabs.GUI
{
    public class HudHandler
    {
        public static bool killLoop = false;

        public static string joinLeave = string.Empty;

        public static string[] peenNutMSG = new string[60];

        public static string[] modifiers = new string[7];

        internal static string[] hint = new string[60];

        public static void FillPeenNutMessage()
        {
            for (int i = 0; i < peenNutMSG.Length; i++)
            {
                peenNutMSG[i] = "\t";
            }
        }

        public static void SendDead(Player deadPlayer)
        {
            try
            {
                string s = string.Empty;
                s += $"<align=center><size=32>\t\n"; //0
                s += $"\t\n"; //1 (TOP SCREEN)
                try { 
                    if (hint[deadPlayer.Id] == string.Empty)
                    {
                        s += $"\t\n"; //2
                        s += $"\t\n"; //3
                        s += $"\t\n"; //4
                    }
                    else
                    {
                        if (hint[deadPlayer.Id].Split(char.Parse("\n")).Length == 0 || hint[deadPlayer.Id].Length < 70)
                        {
                            s += $"{hint[deadPlayer.Id]}\n"; //2
                            s += $"\t\n"; //3
                            s += $"\t\n"; //4
                        }
                        else if (hint[deadPlayer.Id].Split(char.Parse("\n")).Length == 1 ||
                                 hint[deadPlayer.Id].Length > 70 && hint[deadPlayer.Id].Length < 140)
                        {
                            string[] split = hint[deadPlayer.Id].Split(char.Parse("\n"));
                            s += $"{split[0]}\n"; //2
                            s += $"{split[1]}\n"; //3
                            s += $"\t\n"; //4
                        }
                        else if (hint[deadPlayer.Id].Split(char.Parse("\n")).Length == 2 ||
                                 hint[deadPlayer.Id].Length > 140)
                        {
                            string[] split = hint[deadPlayer.Id].Split(char.Parse("\n"));
                            s += $"{split[0]}\n"; //2
                            s += $"{split[1]}\n"; //3
                            s += $"{split[2]}\n"; //4
                        }
                    }
                }
                catch
                {
                    //Log.Error($"Error: {ex}");
                }

                s += $"\t\n"; //5
                s +=
                    $"<align=right><size=24><color=#7F7FFF>NTF Tickets: <color=#fff>{(Respawn.TryGetTokens(SpawnableFaction.NtfWave, out int pNTFTokens) ? pNTFTokens : "Error Fetching Tokens")}<color=#fff>\n"; //6
                s +=
                    $"<align=right><size=24><color=#090>CHAOS Tickets: <color=#fff>{(Respawn.TryGetTokens(SpawnableFaction.ChaosWave, out int pChaosTokens) ? pChaosTokens : "Error Fetching Tokens")}<color=#fff>\n"; //6.5
                s += $"<align=right><size=24>Round Time: {Round.ElapsedTime.Minutes:00}:{Round.ElapsedTime.Seconds:00}\n"; //7

                if (Warhead.IsDetonated)
                {
                    s +=
                        $"<align=right><size=24><color=#7F7FFF>NTF Tickets: <color=#fff>{(Respawn.TryGetTokens(SpawnableFaction.NtfWave, out int NTFTokens) ? NTFTokens : "Error Fetching Tokens")}<color=#fff>\n"; //6
                    s +=
                        $"<align=right><size=24><color=#090>CHAOS Tickets: <color=#fff>{(Respawn.TryGetTokens(SpawnableFaction.ChaosWave, out int ChaosTokens) ? ChaosTokens : "Error Fetching Tokens")}<color=#fff>\n"; //6.5
                    s +=
                        $"<align=right><size=24>Round Time: {Round.ElapsedTime.Minutes:00}:{Round.ElapsedTime.Seconds:00}\n"; //7

                    if (Warhead.IsDetonated)
                    {
                        s +=
                            $"<align=right><size=24>Warhead Status: <color=#c00>Detonated<color=#fff>\n</size><size=32>"; //7.5
                    }
                    else if (Warhead.IsInProgress)
                    {
                        s +=
                            $"<align=right><size=24>Warhead Status: <color=#b45f06>In Progress<color=#fff>\n</size><size=32>"; //7.5
                    }
                    else if (Warhead.LeverStatus)
                    {
                        s +=
                            $"<align=right><size=24>Warhead Status: <color=#090>Armed<color=#fff>\n</size><size=32>"; //7.5
                    }
                    else
                    {
                        s +=
                            $"<align=right><size=24>Warhead Status: <color=#2986cc>Disarmed<color=#fff>\n</size><size=32>"; //7.5
                    }
                }
                else
                {
                    s +=
                        $"<align=right><size=24>Warhead Status: <color=#2986cc>Disarmed<color=#fff>\n</size><size=32>"; //7.5
                }

                //thats is not good too
                for (var i = 9; i < 38; i++)
                {
                    if (i == 27)
                    {
                        s += $"<size=16><align=center>Respawning in: PLACEHOLDER\n\t</size><size=32>\n"; /*{( - DateTime.UtcNow).Minutes:00}:{(Respawn.NextTeamTime - DateTime.UtcNow).Seconds:00}\n\t</size><size=32>\n";*/
                    }
                    else if (i == 29)
                    {
                        s += $"<size=16><color=#3a5fcf>OBSCURE</color><color=#88d4ff>LABS</color>\n";
                        s += $"<color=#ffffff>DISCORD.GG/58TaSJbyJm</color></size>\n";
                    }
                    else
                    {
                        s += $"\t\n";
                    }
                }

                deadPlayer.ShowHint(s, 0.85f);
            }
            catch
            {
                //Log.Warn($"Error: {e}");
            }
        }

        public static IEnumerator<float> SendJoinOrLeaveCoroutine(Player player, bool isLeave)
        {
            var usr = player.DisplayNickname;

            if (!isLeave)
            {
                joinLeave = $"Hello, {usr}!";
                yield return Timing.WaitForSeconds(3);
                if (joinLeave == $"Hello, {usr}!") joinLeave = string.Empty;
            }
            else
            {
                joinLeave = $"Goodbye, {usr}!";
                yield return Timing.WaitForSeconds(3);
                if (joinLeave == $"Goodbye, {usr}!") joinLeave = string.Empty;
            }
        }

        public static IEnumerator<float> SendHintCoroutine(Player player, string hint, float time)
        {
            var localHint = hint;
            HudHandler.hint[player.Id] = hint;

            yield return Timing.WaitForSeconds(time);

            if (player.CurrentHint.Content.Contains(localHint))
            {
                HudHandler.hint[player.Id] = string.Empty;
            }
        }

        internal static void startHints()
        {
            for (int i = 0; i < hint.Length; i++)
            {
                hint[i] = string.Empty;
            }
        }

        //bottom text <align=center><size=15><b><color=#3a5fcf>ObscureLabs</color> <color=#88D4FF>-</color> </b>Discord.gg/f8uEpZWcBv/</size>

        internal static IEnumerator<float> displayGUI(Player p)
        {
            Log.Debug($"Displaying GUI FOR {p.DisplayNickname}");
            hint[p.Id] = string.Empty;
            yield return Timing.WaitForSeconds(5f);
            Log.Debug("Displaying GUI");
            while (true)
            {
                if (p.IsAlive)
                {
                    var effects = new EffectData[7];

                    for (int i = 0; i < p.ActiveEffects.Count(); i++)
                    {
                        if (i >= 8) break;
                        effects[i] = new EffectData(p.ActiveEffects.ElementAt(i).GetEffectType(),
                            p.ActiveEffects.ElementAt(i).TimeLeft);
                    }
                    
                    Log.Debug("Got past effects");
                    string s = "<align=center><size=32>";

                    #region lines

                    s += $"\t\n"; //0
                    s += $"\t\n"; //1 (TOP OF SCREEN)
                    try
                    {
                        if (hint[p.Id] == string.Empty)
                        {
                            s += $"\t\n"; //2
                            s += $"\t\n"; //3
                            s += $"\t\n"; //4
                        }
                        else
                        {
                            if (hint[p.Id].Split(char.Parse("\n")).Length == 0 || hint[p.Id].Length < 70)
                            {
                                s += $"{hint[p.Id]}\n"; //2
                                s += $"\t\n"; //3
                                s += $"\t\n"; //4
                            }
                            else if (hint[p.Id].Split(char.Parse("\n")).Length == 1 ||
                                     hint[p.Id].Length > 70 && hint[p.Id].Length < 140)
                            {
                                string[] split = hint[p.Id].Split(char.Parse("\n"));
                                s += $"{split[0]}\n"; //2
                                s += $"{split[1]}\n"; //3
                                s += $"\t\n"; //4
                            }
                            else if (hint[p.Id].Split(char.Parse("\n")).Length == 2 || hint[p.Id].Length > 140)
                            {
                                string[] split = hint[p.Id].Split(char.Parse("\n"));
                                s += $"{split[0]}\n"; //2
                                s += $"{split[1]}\n"; //3
                                s += $"{split[2]}\n"; //4
                            }
                        }
                    }
                    catch
                    {
                        //Log.Error($"Error: {ex}");
                    }
                    Log.Debug("Got past hints");
                    s += $"\t\n"; //5
                    if (modifiers[0] is null)
                    {
                        s += $"<align=right>\t</align>\n"; //6
                    }
                    else
                    {
                        s += $"<align=right>{modifiers[0]}</align>\n"; //6
                    }

                    if (modifiers[1] is null)
                    {
                        s += $"<align=right>\t</align>\n"; //6
                    }
                    else
                    {
                        s += $"<align=right>{modifiers[1]}</align>\n"; //6
                    }

                    if (modifiers[2] is null)
                    {
                        s += $"<align=right>\t</align>\n"; //6
                    }
                    else
                    {
                        s += $"<align=right>{modifiers[2]}</align>\n"; //6
                    }

                    if (modifiers[3] is null)
                    {
                        s += $"<align=right>\t</align>\n"; //6
                    }
                    else
                    {
                        s += $"<align=right>{modifiers[3]}</align>\n"; //6
                    }

                    if (modifiers[4] is null)
                    {
                        s += $"<align=right>\t</align>\n"; //6
                    }
                    else
                    {
                        s += $"<align=right>{modifiers[4]}</align>\n"; //6
                    }

                    if (modifiers[5] is null)
                    {
                        s += $"<align=right>\t</align>\n"; //6
                    }
                    else
                    {
                        s += $"<align=right>{modifiers[5]}</align>\n"; //6
                    }

                    if (modifiers[6] is null)
                    {
                        s += $"<align=right>\t</align>\n"; //6
                    }
                    else
                    {
                        s += $"<align=right>{modifiers[6]}</align>\n"; //6
                    }

                    s += $"\t\n"; //13
                    s += $"\t\n"; //14
                    s += $"\t\n"; //15
                    s += $"\t\n"; //16
                    s += $"\t\n"; //17
                    s += $"\t</size>\n<size=16>"; //18
                    Log.Debug("Got to effects");
                    //effectData[] effectsSorted = effects.OrderBy(x => x.time).ToArray();
                    //foreach (effectData fx in effectsSorted)
                    //{
                    //    if (fx == null || (int)fx.time <= 0)
                    //    {
                    //        s += $"<align=left>\t</align>\n"; //19
                    //        s += $"<align=left>\t</align>\n"; //19
                    //    }
                    //    else
                    //    {
                    //        s += $"<align=left>{fx.name} - {(int)fx.time}s</align>\n"; //19
                    //        s += $"<align=left>\t</align>\n"; //19
                    //    }
                    //}
                    if (effects[0] == null || (int)effects[0].Duration <= 0)
                    {
                        s += $"<align=left>\t</align>\n"; //19
                        s += $"<align=left>\t</align>\n"; //19
                    }
                    else
                    {
                        s += $"<align=left>{effects[0].GetMessage()} - {(int)effects[0].Duration}s</align>\n"; //19
                        s += $"<align=left>\t</align>\n"; //19
                    }

                    Log.Debug("Did effect 1");
                    if (effects[1] == null || (int)effects[1].Duration <= 0)
                    {
                        s += $"<align=left>\t</align>\n"; //20
                        s += $"<align=left>\t</align>\n"; //19
                    }
                    else
                    {
                        s += $"<align=left>{effects[1].GetMessage()} - {(int)effects[1].Duration}s</align>\n"; //20
                        s += $"<align=left>\t</align>\n"; //19
                    }

                    Log.Debug("Did effect 1");
                    if (effects[2] == null || (int)effects[2].Duration <= 0)
                    {
                        s += $"<align=left>\t</align>\n"; //21
                        s += $"<align=left>\t</align>\n"; //19
                    }
                    else
                    {
                        s += $"<align=left>{effects[2].GetMessage()}  -  {(int)effects[2].Duration}s</align>\n"; //21
                        s += $"<align=left>\t</align>\n"; //19
                    }

                    Log.Debug("Did effect 2");
                    if (effects[3] == null || (int)effects[3].Duration <= 0)
                    {
                        s += $"<align=left>\t</align>\n"; //22
                        s += $"<align=left>\t</align>\n"; //19
                    }
                    else
                    {
                        s += $"<align=left>{effects[3].GetMessage()} - {(int)effects[3].Duration}s</align>\n"; //22
                        s += $"<align=left>\t</align>\n"; //19
                    }

                    Log.Debug("Did effect 3");
                    if (effects[4] == null || (int)effects[4].Duration <= 0)
                    {
                        s += $"<align=left>\t</align>\n"; //23
                        s += $"<align=left>\t</align>\n"; //19
                    }
                    else
                    {
                        s += $"<align=left>{effects[4].GetMessage()} - {(int)effects[4].Duration}s</align>\n"; //23
                        s += $"<align=left>\t</align>\n"; //19
                    }

                    Log.Debug("Did effect 4");
                    if (effects[5] == null || (int)effects[5].Duration <= 0)
                    {
                        s += $"<align=left>\t</align>\n"; //24
                        s += $"<align=left>\t</align>\n"; //19
                    }
                    else
                    {
                        s += $"<align=left>{effects[5].GetMessage()} - {(int)effects[5].Duration}s</align>\n"; //24
                        s += $"<align=left>\t</align>\n"; //19
                    }

                    Log.Debug("Did effect 5");
                    if (effects[6] == null || (int)effects[6].Duration <= 0)
                    {
                        s += $"<align=left>\t</align>\n"; //25
                        s += $"<align=left>\t</align>\n"; //19
                    }
                    else
                    {
                        s += $"<align=left>{effects[6].GetMessage()} - {(int)effects[6].Duration}s</align>\n"; //25
                        s += $"<align=left>\t</align>\n"; //19
                    }

                    Log.Debug("Did effects");
                    s += $"</size><size=32>\t\n"; //26
                    s += $"\t\n"; //27
                    s += $"\t\n"; //28
                    s +=
                        $"<size=16><color=#3a5fcf>OBSCURE</color><color=#88d4ff>LABS</color>\n"; //29 (BOTTOM OF SCREEN)
                    s += $"<color=#ffffff>DISCORD.GG/58TaSJbyJm</color></size>\n";
                    s += $"<size=32><color=#ffffff>\t\n"; //30
                    s += $"\t\n"; //31
                    s += $"\t\n"; //32
                    s += $"\t\n"; //33
                    s += $"\t\n"; //34
                    s += $"\t\n"; //35
                    s += $"\t\n"; //36
                    s += $"\t\n"; //37
                    s += $"\t\n"; //38

                    #endregion

                    s += "</align></size>";
                    Log.Debug("What the fuck");
                    yield return Timing.WaitForSeconds(0.5f);

                    Log.Debug("Completed Message");
                    if (p.IsAlive)
                    {
                        p.ShowHint(s, 0.85f);
                    }

                    Log.Debug("Shown Hint");
                }
                else if (!Round.IsLobby && !p.IsAlive)
                {
                    yield return Timing.WaitForSeconds(0.5f);
                    SendDead(p);
                    Log.Debug($"Sending Dead Message to : {p.DisplayNickname}");
                }
                else
                {
                    yield return Timing.WaitForSeconds(0.1f);
                }
            }
        }
    }
}
