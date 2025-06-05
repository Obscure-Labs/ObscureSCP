using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
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
            hint[p.PlayerId] = new(){ new Hint("THIS IS A TEMP HINT", 5000f, HintPosition.TmpHint), new Hint("THIS IS A PERM HINT", 0f, HintPosition.PmtHint), new Hint("THIS IS A GMD INFO HINT", 0f, HintPosition.GmdInfo) };
            yield return Timing.WaitForSeconds(5f);
            Timing.RunCoroutine(EffectChecker(p));
            Log.Info($"Current aspect ratio is: {p.aspectRatioSync.AspectRatio}");
            while (true)
            {
                string s = string.Empty;
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
                    List<Hint> oldHints = new List<Hint>();
                    s += $"<align=left><pos=540><line-height=0><voffset=9999><size=16>TOPANCHOR</size></voffset></line-height></pos></align>";
                    //s += $"<align=left><pos=540><line-height=0><voffset=0><size=16>is center?</size></voffset></line-height></pos></align>";
                    int effectCount = 0;
                    int gmdCount = 0;
                    try
                    {
                        foreach (Hint hint in hint[p.PlayerId])
                        {
                            if(hint.Position == HintPosition.EftInfo)
                            {
                                continue;
                            }
                            if (hint.Position == HintPosition.TmpHint && hint.StartTime.AddSeconds(hint.Time) < DateTime.UtcNow)
                            {
                                oldHints.Add(hint);
                                continue;
                            }


                            int vOffset = -69420; // Arbitraty number that would never get used
                            int baseXPos = -69420; // Center position (will be modded based on aspect ratio later)

                            switch (hint.Position)
                            {
                                case HintPosition.TmpHint:
                                    vOffset = 670;
                                    break;
                                case HintPosition.PmtHint:
                                    vOffset = -300;
                                    //baseXPos = Mathf.RoundToInt(-675*xScalar); // Slightly off-center for PmtHint
                                    break;
                                case HintPosition.GmdInfo:
                                    vOffset = 200 + (gmdCount * 15);
                                    baseXPos = Mathf.RoundToInt(-675 * xScalar); // Centered for GmdInfo
                                    gmdCount++;
                                    break;
                            }
                            //s += $"<align=left><line-height=0><voffset=146><size=16><pos=600>{text}</pos></size></voffset></line-height></align>";
                            s += $"<align=left><line-height=0><voffset={vOffset}><size=16><pos={(baseXPos == -69420 ? 600 - (GetStringWidth(hint.Content, 16) / 2 / 3) : baseXPos)}>{hint.Content}</pos></size></voffset></line-height></align>";
                        }
                        hint[p.PlayerId].RemoveAll(x => oldHints.Contains(x));
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error rendering hints: {ex.Message}");
                    }
                    try
                    {
                        if(effectCount == 0)
                        {
                            s += $"<align=left><line-height=0><voffset={170 - (gmdCount * 15)}><size=16><pos={Mathf.RoundToInt(-675 * xScalar)}>{"☐☐☐☐☐"}</pos></size></voffset></line-height></align>";
                        }
                        foreach (Hint hint in hint[p.PlayerId].Where(x => x.Position == HintPosition.EftInfo))
                        {
                            if (effectCount >= 5) { break; }
                            s += $"<align=left><line-height=0><voffset={140 - (gmdCount * 15) - (effectCount * 15)}><size=16><pos={Mathf.RoundToInt(-675 * xScalar)}>{hint.Content}</pos></size></voffset></line-height></align>";
                            effectCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error rendering effects: {ex.Message}");
                    }
                    string watermarkText = $"<color=#CD02FF>O</color><color=#BD02FC>b</color><color=#AD02F9>s</color><color=#9D02F6>c</color><color=#8D02F3>u</color><color=#7D02F0>r</color><color=#6D02ED>e</color><color=#5D02EA>L</color><color=#4D02E7>a</color><color=#3D02E4>b</color><color=#2D02E1>s</color>";
                    s += $"<align=left><line-height=0><voffset={-370}><size=16><pos={600 - (GetStringWidth("ObscureLabs", 16) / 2 / 3)}>{watermarkText}</pos></size></voffset></line-height></align>";
                    s += $"<align=left><line-height=0><voffset={-385}><size=16><pos={600 - (GetStringWidth("chatobscnet'", 16) / 2 / 3)}>{"chat.obsc.net"}</pos></size></voffset></line-height></align>";
                    s += $"<align=left><pos=540><line-height=0><voffset=-9999><size=16>BOTTOMANCHOR</size></voffset></line-height></pos></align>";

                    try
                    {
                        Player.Get(p).SendConsoleMessage(s, Color.white.ToString());
                        Player.Get(p).ShowHint(s, 1f);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error showing hint: {ex.Message}");
                    }
                    yield return Timing.WaitForSeconds(0.9f);
                }
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
                            hint[p.PlayerId].Add(new Hint($"{effect.name} {effect.Intensity} - {Mathf.RoundToInt(effect.TimeLeft)}s", 0, HintPosition.EftInfo));
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
    }
}
