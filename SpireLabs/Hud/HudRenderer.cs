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
            GmdInfo
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
            yield return Timing.WaitForSeconds(5f);
            while (true)
            {
                string s = string.Empty;
                if (Player.Get(p).IsAlive)
                {
                    List<Hint> oldHints = new List<Hint>();
                    s += $"<align=left><pos=540><line-height=0><voffset=9999><size=16>TOPANCHOR</size></voffset></line-height></pos></align>";
                    //s += $"<align=left><pos=540><line-height=0><voffset=0><size=16>is center?</size></voffset></line-height></pos></align>";
                    foreach (Hint hint in hint[p.PlayerId])
                    {
                        if(hint.Position == HintPosition.TmpHint && hint.StartTime.AddSeconds(hint.Time) < DateTime.UtcNow)
                        {
                            oldHints.Add(hint);
                            continue;
                        }

                        int vOffset = -69420; // Arbitraty number that would never get used
                        int baseXPos = 600; // Center position (will be modded based on aspect ratio later)

                        switch (hint.Position)
                        {
                            case HintPosition.TmpHint:
                                vOffset = 300;
                                break;
                            case HintPosition.PmtHint:
                                vOffset = 200;
                                baseXPos = 200; // Slightly off-center for PmtHint
                                break;
                            case HintPosition.GmdInfo:
                                vOffset = 100;
                                baseXPos = 400; // Centered for GmdInfo
                                break;

                        }
                        //s += $"<align=left><line-height=0><voffset=146><size=16><pos=600>{text}</pos></size></voffset></line-height></align>";
                        s += $"<align=left><line-height=0><voffset={vOffset}><size=16><pos={baseXPos - (GetStringWidth(hint.Content, 16) / 2 / 3)}>{hint.Content}</pos></size></voffset></line-height></align>";
                    }
                    s += $"<align=left><pos=540><line-height=0><voffset=-9999><size=16>BOTTOMANCHOR</size></voffset></line-height></pos></align>";
                }
                Player.Get(p).ShowHint(s, 1f);
                Log.SendRaw(s, ConsoleColor.White);
                yield return Timing.WaitForSeconds(0.9f);
            }

        }

        public static int GetStringWidth(string text, int fontSize)
        {
            float width = 0;
            foreach (char c in text)
            {
                if (char.IsControl(c)) continue;
                float ratio = fontSize / (BaseFontSize * 1.25f);
                float cw = ChWidth.TryGetValue(c, out float charWidth) ? charWidth * ratio : DefaultFontWidth * ratio;
                Log.Info("Character: " + c + ", Width: " + cw + ", Ratio: " + ratio);
                width += cw;
            }
            Log.Error($"Calculated width for text '{text}' with font size {fontSize} is {width}.");
            return Mathf.RoundToInt(width);
        }
    }
}
