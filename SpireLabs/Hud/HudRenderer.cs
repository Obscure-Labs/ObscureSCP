using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using YamlDotNet.Serialization;

namespace ObscureLabs.Hud
{
    internal class HudRenderer
    {
        private const float BaseFontSize = 34.7f;
        private const float DefaultFontWidth = 67.81861f;

        private static readonly ConcurrentDictionary<char, float> ChWidth = new();

        public HudRenderer()
        {
            Task.Run(() =>
            {
                try
                {
                        
                        string widthstr = File.ReadAllText(Path.Combine(Plugin.SpireConfigLocation, "fontwidths"));
                        Dictionary<int, float> dictionary = new DeserializerBuilder().Build().Deserialize<Dictionary<int, float>>(widthstr);

                        foreach (KeyValuePair<int, float> kvp in dictionary)
                        {
                            ChWidth.TryAdd((char)kvp.Key, kvp.Value);
                        }
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            });
        }

        public static void SetPermHint(ReferenceHub p, string hint, int slot)
        {
            if(!HudRenderer.hint.ContainsKey(p.PlayerId))
            {
                HudRenderer.hint[p.PlayerId] = new List<Hint>();
            }
            if (HudRenderer.hint[p.PlayerId].Any(x => x.slot == slot))
            {
                HudRenderer.hint[p.PlayerId].RemoveAll(x => x.slot == slot);
            }
            HudRenderer.hint[p.PlayerId].Add(new Hint(hint, 0f, HintPosition.PmtHint) { slot = slot });
        }

        public static void SetGmdInfo(ReferenceHub p, string hint, int slot)
        {
            if (!HudRenderer.hint.ContainsKey(p.PlayerId))
            {
                HudRenderer.hint[p.PlayerId] = new List<Hint>();
            }
            if (HudRenderer.hint[p.PlayerId].Any(x => x.slot == slot))
            {
                HudRenderer.hint[p.PlayerId].RemoveAll(x => x.slot == slot);
            }
            HudRenderer.hint[p.PlayerId].Add(new Hint(hint, 0f, HintPosition.GmdInfo) { slot = slot });
        }

        public static void SetGlobalGmdInfo(string hint, int slot)
        {
            if(globalHints.Any(x => x.slot == slot))
            {
                globalHints.RemoveAll(x => x.slot == slot);
            }
            globalHints.Add(new Hint(hint, 0f, HintPosition.GmdInfo) { slot = slot });
        }

        public static IEnumerator<float> SendHintCoroutine(ReferenceHub p, string hint, float time, HintPosition position)
        {
            HudRenderer.hint[p.PlayerId].RemoveAll(x => x.Position == HintPosition.TmpHint);
            hint += $"@{Guid.NewGuid()}";
            if (!HudRenderer.hint.ContainsKey(p.PlayerId))
            {
                HudRenderer.hint[p.PlayerId] = new List<Hint>();
            }
            HudRenderer.hint[p.PlayerId].Add(new Hint(hint, time, position));
            yield return Timing.WaitForSeconds(time);
            HudRenderer.hint[p.PlayerId].RemoveAll(x => x.Content.Contains(hint.Split('@')[1]));
        }

        public enum HintPosition
        {
            TmpHint,
            PmtHint,
            GmdInfo,
            EftInfo
        }

        public class Hint
        {
            public string Content { get; set; }
            public float Time { get; set; }
            public DateTime StartTime { get; set; }
            public HintPosition Position { get; set; }
            public int slot { get; set; }
            public Hint(string content, float time, HintPosition position)
            {
                Content = content;
                Time = time;
                StartTime = DateTime.UtcNow;
                Position = position;
            }
        }
        public static TMP_FontAsset fontAsset;
        //public static string[] hint = new string[Server.MaxPlayerCount];
        public static Dictionary<int, List<Hint>> hint = new Dictionary<int, List<Hint>>();

        public static List<Hint> globalHints = new List<Hint>();

        //public static IEnumerator<float> SendHintCoroutine(Player player, string hint, float time)
        //{
        //    var localHint = $"{hint}@{Guid.NewGuid()}";
        //    HudRenderer.hint[player.Id] = localHint;

        //    yield return Timing.WaitForSeconds(time);

        //    if (player.CurrentHint.Content.Contains(localHint.Split('@')[1]))
        //    {
        //        HudRenderer.hint[player.Id] = string.Empty;
        //    }
        //}

        // <pos= NEGATIVE GOES LEFT > </pos>
        // <line-height=0> </line-height> stops the line from being fucked with by others
        // <voffset= VERTICAL POSITION > </voffset> moves the text vertically center of screen is 146
        // CENTER TEXT = 600-(GetStringWidth(text, 16)/2/3)

        internal static IEnumerator<float> RenderUI(ReferenceHub p)
        {
            hint[p.PlayerId] = new();//{new Hint("THIS IS A PERM HINT", 0f, HintPosition.PmtHint), new Hint("THIS IS A GMD INFO HINT", 0f, HintPosition.GmdInfo) };
            yield return Timing.WaitForSeconds(5f);
            Timing.RunCoroutine(EffectChecker(p));
            Log.Info($"Current aspect ratio is: {p.aspectRatioSync.AspectRatio}");
            while (true)
            {
                string s = string.Empty;
                try
                {
                    if (Player.Get(p).IsAlive)
                    {
                        float xScalar = 1f;
                        switch (p.aspectRatioSync.AspectRatio.ToString())
                        {
                            case "1.777778": // 16:9
                                xScalar = 0.525f;
                                break;
                            case "2.37037": // 21:9
                                xScalar = 1f;
                                break;
                            case "1.333333": // 4:3
                                xScalar = 0.175f;
                                break;
                            case "1.6": // 16:10
                                xScalar = 0.375f;
                                break;

                        }
                        s += $"<align=left><pos=540><line-height=0><voffset=9999><size=16>TOPANCHOR</size></voffset></line-height></pos></align>";
                        //s += $"<align=left><pos=540><line-height=0><voffset=0><size=16>is center?</size></voffset></line-height></pos></align>";
                        int effectCount = 0;
                        int gmdCount = 0;
                        foreach (Hint hint in hint[p.PlayerId])
                        {
                            int vOffset = -69420; // Arbitraty number that would never get used
                            int baseXPos = -69420; // Center position (will be modded based on aspect ratio later)

                            switch (hint.Position)
                            {
                                case HintPosition.TmpHint:
                                    vOffset = 670;
                                    break;
                                case HintPosition.PmtHint:
                                    vOffset = -300 - (hint.slot * 16);
                                    //baseXPos = Mathf.RoundToInt(-675*xScalar); // Slightly off-center for PmtHint
                                    break;
                                case HintPosition.GmdInfo:
                                    if (gmdCount >= 4) { continue; }
                                    vOffset = 200 + (hint.slot * 16);
                                    baseXPos = Mathf.RoundToInt(-675 * xScalar); // Centered for GmdInfo
                                    gmdCount++;
                                    break;
                                case HintPosition.EftInfo:
                                    continue;
                                    break;
                            }
                            if (hint.Position == HintPosition.TmpHint)
                            {
                                string nouid = hint.Content.Split('@')[0];
                                s += $"<align=left><line-height=0><voffset=2000><size=16>{nouid}</size></voffset></line-height></align>";
                                if (nouid.Split(char.Parse("\n")) is string[] wtf && wtf.Count() == 1)
                                {
                                    s += $"<align=left><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf[0], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf[0]}</pos></size></voffset></line-height></align>";
                                }
                                else if (nouid.Split(char.Parse("\n")) is string[] wtf1 && wtf1.Count() == 2)
                                {
                                    s += $"<align=left><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf1[0], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf1[0]}</pos></size></voffset></line-height></align>";
                                    s += $"<align=left><line-height=0><voffset={vOffset - 16}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf1[1], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf1[1]}</pos></size></voffset></line-height></align>";
                                }
                                else if (nouid.Split(char.Parse("\n")) is string[] wtf2 && wtf2.Count() == 3)
                                {
                                    s += $"<align=left><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf2[0], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf2[0]}</pos></size></voffset></line-height></align>";
                                    s += $"<align=left><line-height=0><voffset={vOffset - 16}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf2[1], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf2[1]}</pos></size></voffset></line-height></align>";
                                    s += $"<align=left><line-height=0><voffset={vOffset - 32}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf2[2], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf2[2]}</pos></size></voffset></line-height></align>";
                                }
                            }
                            else
                            {
                                s += $"<align=left><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(hint.Content, 16) / 2 / 3) : baseXPos)}>{hint.Content}</pos></size></voffset></line-height></align>";
                            }
                        }
                        if (Player.Get(p).IsAlive)
                        {
                            foreach (Hint hint in hint[p.PlayerId].Where(x => x.Position == HintPosition.EftInfo))
                            {
                                if (effectCount >= 7) { break; }
                                s += $"<align=right><line-height=0><voffset={140 - (gmdCount * 16) - (effectCount * 16)}><size=16><pos={Mathf.RoundToInt(-675 * xScalar)}>{hint.Content}</pos></size></voffset></line-height></align>";
                                effectCount++;
                            }
                        }
                        string watermarkText = $"<color=#F932DB>O</color><color=#ED36DD>b</color><color=#E13ADF>s</color><color=#D53EE1>c</color><color=#C942E3>u</color><color=#BD46E5>r</color><color=#B14AE7>e</color><color=#A54EE9>L</color><color=#9952EB>a</color><color=#8D56ED>b</color><color=#815AEF>s</color>";
                        //s += $"<align=left><line-height=0><voffset={-385}><size=32><pos={600 - (GetStringWidth("█████████████", 32) / 2 / 3)}><color=#000000><alpha=#AA>█████████████<alpha=#FF></color></pos></size></voffset></line-height></align>";
                        s += $"<align=left><line-height=0><voffset={-370}><size=20><pos={602 - (GetStringWidth("ObscureLabs", 20) / 2 / 3)}>{watermarkText}</pos></size></voffset></line-height></align>";
                        s += $"<align=left><line-height=0><voffset={-385}><size=16><pos={600 - (GetStringWidth("CHAT.OBSC.NET'", 16) / 2 / 3)}>{"CHAT.OBSC.NET"}</pos></size></voffset></line-height></align>";
                        s += $"<align=left><pos=540><line-height=0><voffset=-9999><size=16>BOTTOMANCHOR</size></voffset></line-height></pos></align>";
                        Player.Get(p).SendConsoleMessage(s, Color.white.ToString());
                        Player.Get(p).ShowHint(s, 1f);
                        //}
                    }
                    else 
                    {
                        float xScalar = 1f;
                        switch (p.aspectRatioSync.AspectRatio.ToString())
                        {
                            case "1.777778": // 16:9
                                xScalar = 0.525f;
                                break;
                            case "2.37037": // 21:9
                                xScalar = 1f;
                                break;
                            case "1.333333": // 4:3
                                xScalar = 0.175f;
                                break;
                            case "1.6": // 16:10
                                xScalar = 0.375f;
                                break;

                        }
                        s += $"<align=left><pos=540><line-height=0><voffset=9999><size=16>TOPANCHOR</size></voffset></line-height></pos></align>";
                        //s += $"<align=left><pos=540><line-height=0><voffset=0><size=16>is center?</size></voffset></line-height></pos></align>";
                        int effectCount = 0;
                        int gmdCount = 0;
                        foreach (Hint hint in hint[p.PlayerId])
                        {
                            int vOffset = -69420; // Arbitraty number that would never get used
                            int baseXPos = -69420; // Center position (will be modded based on aspect ratio later)

                            switch (hint.Position)
                            {
                                case HintPosition.TmpHint:
                                    vOffset = 670;
                                    break;
                                case HintPosition.PmtHint:
                                    vOffset = -300 - (hint.slot * 16);
                                    //baseXPos = Mathf.RoundToInt(-675*xScalar); // Slightly off-center for PmtHint
                                    break;
                                case HintPosition.GmdInfo:
                                    if (gmdCount >= 4) { continue; }
                                    vOffset = 200 + (hint.slot * 16);
                                    baseXPos = Mathf.RoundToInt(-675 * xScalar); // Centered for GmdInfo
                                    gmdCount++;
                                    break;
                                case HintPosition.EftInfo:
                                    continue;
                                    break;
                            }
                            //if (hint.Position == HintPosition.TmpHint)
                            //{
                            //    string nouid = hint.Content.Split('@')[0];
                            //    s += $"<align=right><line-height=0><voffset=2000><size=16>{nouid}</size></voffset></line-height></align>";
                            //    if (nouid.Split(char.Parse("\n")) is string[] wtf && wtf.Count() == 1)
                            //    {
                            //        s += $"<align=right><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf[0], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf[0]}</pos></size></voffset></line-height></align>";
                            //    }
                            //    else if (nouid.Split(char.Parse("\n")) is string[] wtf1 && wtf1.Count() == 2)
                            //    {
                            //        s += $"<align=right><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf1[0], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf1[0]}</pos></size></voffset></line-height></align>";
                            //        s += $"<align=right><line-height=0><voffset={vOffset - 16}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf1[1], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf1[1]}</pos></size></voffset></line-height></align>";
                            //    }
                            //    else if (nouid.Split(char.Parse("\n")) is string[] wtf2 && wtf2.Count() == 3)
                            //    {
                            //        s += $"<align=right><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf2[0], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf2[0]}</pos></size></voffset></line-height></align>";
                            //        s += $"<align=right><line-height=0><voffset={vOffset - 16}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf2[1], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf2[1]}</pos></size></voffset></line-height></align>";
                            //        s += $"<align=right><line-height=0><voffset={vOffset - 32}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(Regex.Replace(wtf2[2], @"\<.*?\>", ""), 16) / 2 / 3) : baseXPos)}>{wtf2[2]}</pos></size></voffset></line-height></align>";
                            //    }
                            //}
                            //else
                            //{
                            //    s += $"<align=right><line-height=0><voffset={vOffset}><size=16><pos={(600 - (GetStringWidth(hint.Content, 16) / 2 / 3))}>{hint.Content}</pos></size></voffset></line-height></align>";
                            //}
                        }
                        if (Player.Get(p).Role is SpectatorRole role)
                        {
                            foreach (Hint hint in hint[role.SpectatedPlayer.ReferenceHub.PlayerId].Where(x => x.Position == HintPosition.EftInfo))
                            {
                                if (effectCount >= 7) { break; }
                                s += $"<align=right><line-height=0><voffset={140 - (gmdCount * 16) - (effectCount * 16)}><size=16><pos={Mathf.RoundToInt(875 * xScalar)}>{hint.Content}</pos></size></voffset></line-height></align>";
                                effectCount++;
                            }
                        }
                        string watermarkText = $"<color=#F932DB>O</color><color=#ED36DD>b</color><color=#E13ADF>s</color><color=#D53EE1>c</color><color=#C942E3>u</color><color=#BD46E5>r</color><color=#B14AE7>e</color><color=#A54EE9>L</color><color=#9952EB>a</color><color=#8D56ED>b</color><color=#815AEF>s</color>";
                        //s += $"<align=left><line-height=0><voffset={-385}><size=32><pos={600 - (GetStringWidth("█████████████", 32) / 2 / 3)}><color=#000000><alpha=#AA>█████████████<alpha=#FF></color></pos></size></voffset></line-height></align>";
                        s += $"<align=right><line-height=0><voffset={-370}><size=20><pos={52 - (GetStringWidth("ObscureLabs", 20) / 2 / 3)}>{watermarkText}</pos></size></voffset></line-height></align>";
                        s += $"<align=right><line-height=0><voffset={-385}><size=16><pos={50 - (GetStringWidth("CHAT.OBSC.NET'", 16) / 2 / 3)}>{"CHAT.OBSC.NET"}</pos></size></voffset></line-height></align>";
                        s += $"<align=right><pos=540><line-height=0><voffset=-9999><size=16>BOTTOMANCHOR</size></voffset></line-height></pos></align>";
                        Player.Get(p).SendConsoleMessage(s, Color.white.ToString());
                        Player.Get(p).ShowHint(s, 1f);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error rendering UI: {ex.Message}");
                    s = string.Empty; // Reset the string in case of an error
                }
                yield return Timing.WaitForSeconds(0.9f);
            }

        }

        public static int GetStringWidth(string text, int fontSize)
        {
            float width = 0;
            try
            {
                foreach (char c in text)
                {
                    if (char.IsControl(c)) continue;
                    float ratio = fontSize / (BaseFontSize * 1.25f);
                    float cw = ChWidth.TryGetValue(c, out float charWidth) ? charWidth * ratio : DefaultFontWidth * ratio;
                    //Log.Info("Character: " + c + ", Width: " + cw + ", Ratio: " + ratio);
                    width += cw;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error calculating string width: {ex.Message}");
                return 0;
            }
            return Mathf.RoundToInt(width);
            //Log.Error($"Calculated width for text '{text}' with font size {fontSize} is {width}.");
        }

        public class EffectData
        {
            public string EffectName { get; set; }
            public int TimeLeft { get; set; }
            public float Intensity { get; set; }
        }

        public static IEnumerator<float> EffectChecker(ReferenceHub p)
        {
            while (true)
            {
                hint[p.PlayerId].RemoveAll(x => x.Position == HintPosition.EftInfo);
                try
                {
                    foreach (var effect in p.playerEffectsController.AllEffects)
                    {
                        if (effect.Intensity != 0)
                        {
                            if (!effectDictionary.TryGetValue(effect.name, out string effectsString))
                            {
                                Log.Error($"Effect '{effect.name}' not found in effect dictionary.");
                            }
                            else
                            {
                                hint[p.PlayerId].Add(new Hint($"{effectsString}<size=10>x{effect.Intensity}</size> <color=#8a8a8a>-</color> {Mathf.RoundToInt(effect.TimeLeft)}s", 0, HintPosition.EftInfo));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error checking effects: {ex.Message}");
                }
                yield return Timing.WaitForSeconds(0.75f);
            }
        }

        #region effectList
        public static Dictionary<string, string> effectDictionary = new Dictionary<string, string>
        {
            { "Asphyxiated", "<b><color=#95c700>Asphyxiated</color></b>" },
            { "Bleeding", "<b><color=#750004>Bleeding</color></b>" },
            { "Blinded", "<b><color=#394380>Blinded</color></b>" },
            { "Burned", "<b><color=#de831b>Burned</color></b>" },
            { "Deafened", "<b><color=#4a2700>Deafened</color></b>" },
            { "Ensnared", "<b><color=#00750e>Ensnared</color></b>" },
            { "Exhausted", "<b><color=#696969>Exhausted</color></b>>" },
            { "Flashed", "<b><color=#d6fffd>Flashed</color></b>" },
            { "Invigorated", "<b><color=green>Invigorated</color></b>" },
            { "Poisoned", "<b><color=#02521e>Poisoned</color></b>" },
            { "DamageReduction", "<b><color=#e3b76b>Damage Reduction</color></b>" },
            { "MovementBoost", "<b><color=#3bafd9>Movement Boost</color></b>" },
            { "RainbowTaste", "<b><color=#FF0000>R</color><color=#FF7F00>a</color><color=#FFFF00>i</color><color=#7FFF00>n</color><color=#00FF00>b</color><color=#00FF7F>o</color><color=#00FEFF>w</color><color=#007FFF> T</color><color=#0000FF>a</color><color=#7F00FF>s</color><color=#FF00FE>t</color><color=#FF007F>e</color></b>" },
            { "SeveredHands", "<b><color=red>Severed Hands</color></b>" },
            { "Stained", "<b><color=#543601>Stained</color></b>" },
            { "Vitality", "<b><color=#71c788>Vitality</color></b>" },
            { "Hypothermia", "<b><color=#42e9ff>Hypothermia</color></b>" },
            { "Scp1853", "<b><color=#c9f59a>SCP1853</color></b>" },
            { "CardiacArrest", "<b><color=red>Cardiac Arrest</color></b>" },
            { "AntiScp207", "<b><color=#fa70ff>Anti Scp207</color></b>" },
            { "Invisible", "<b><color=#540042>Invisible</color></b>" },
            { "Scp207", "<b><color=#bb80ff>Scp207</color></b>" },
            { "BodyshotReduction", "<b><color=#c9af3a>Bodyshot Reduction</color></b>" },
            { "Hemorrhage", "<b><color=#ff0000>Hemorrhage</color></b>" },
            { "Disabled", "<b><color=#828282>Disabled</color></b>" },
            { "Corroding", "<b><color=#5e8c60>Corroding</color></b>" },
            { "Concussed", "<b><color=#5e728c>Concussed</color></b>" },
            { "Scanned", "<b><color=#ffff00>Scanned</color></b>" },
            { "Ghostly", "<b><color=#f2f3f5>Ghostly</color></b>" },
            { "Strangled", "<b><color=#687fad>Strangled</color></b>" },
            { "SilentWalk", "<b><color=#a9a9a9>Silent Walk</color></b>" },
            { "Sinkhole", "<b><color=#29362d>Sink Hole</color></b>" },
            { "Blurred", "<b><color=#a9a9a9>Blurred</color></b>" }
        };
        #endregion
        #region string functions
        public static List<string> SplitString(string s, int n)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < s.Length; i += n)
            {
                result.Add(s.Substring(i, Math.Min(n, s.Length - i)));
            }
            return result;
        }
        #endregion
    }
}
