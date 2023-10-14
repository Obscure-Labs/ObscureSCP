using Exiled.API.Features;
using MEC;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SpireLabs
{
    internal class guiHandler
    {
        internal static bool killLoop = false;
        private static string joinLeave = string.Empty;
        internal static IEnumerator<float> sendJoinLeave(Player p)
        {
            Timing.WaitForSeconds(3f);
            while(joinLeave != string.Empty)
            {
                yield return Timing.WaitForSeconds(0.5f);
            }
            if (joinLeave == string.Empty)
            {
                joinLeave = $"Welcome, {p.DisplayNickname}!";
                yield return Timing.WaitForSeconds(3);
                joinLeave = string.Empty;
            }
        }
        internal static IEnumerator<float> sendHint(Player p, string h, int t)
        {
            while (hint[p.Id] != string.Empty)
            {
                yield return Timing.WaitForSeconds(0.5f);
            }
            if (hint[p.Id] == string.Empty)
            {
                hint[p.Id] = h;
                yield return Timing.WaitForSeconds(t);
                hint[p.Id] = string.Empty;
            }           
        }

        internal static string[] hint = new string[60];

        internal static void startHints()
        {
            for(int i = 0; i < hint.Length; i++)
            {
                hint[i] = string.Empty;
            }        
        }

        internal static IEnumerator<float> displayGUI(Player p)
        {
            hint[p.Id] = string.Empty;
            yield return Timing.WaitForSeconds(5f);
            Log.Debug("Displaying GUI");
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                string[] effects = { null, null, null, null, null, null, null, null, null };
                Log.Debug("Created array");
                Log.Debug("Success");
                if (p.Role == RoleTypeId.Spectator)
                {
                    yield return Timing.WaitForSeconds(1f);
                }
                else
                {
                    effects[0] = null;
                    effects[1] = null;
                    effects[2] = null;
                    effects[3] = null;
                    effects[4] = null;
                    effects[5] = null;
                    effects[6] = null;
                    effects[7] = null;
                    effects[8] = null;
                    Log.Debug("Entered Loop");
                    if ((p.Role == RoleTypeId.Spectator) || (p.Role == RoleTypeId.None))
                    { }
                    else
                    {
                        Log.Debug("Checking Effects");
                        Log.Debug($"Player has {p.ActiveEffects.Count()} effects");
                        Log.Debug($"success");
                        for (int i = 0; i < p.ActiveEffects.Count(); i++)
                        {
                            try
                            {
                                Log.Debug("Checking Effect Type");
                                switch (p.ActiveEffects.ElementAt(i).name)
                                {
                                    case "Asphyxiated":
                                        effects[i] = "<size=25><b><color=#95c700>Asphyxiated</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Bleeding":
                                        effects[i] = "<size=25><b><color=#750004>Bleeding</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Blinded":
                                        effects[i] = "<size=25><b><color=#394380>Blinded</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Burned":
                                        effects[i] = "<size=25><b><color=#de831b>Burned</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Deafened":
                                        effects[i] = "<size=25><b><color=#4a2700>Deafened</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Ensnared":
                                        effects[i] = "<size=25><b><color=#00750e>Ensnared</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Exhausted":
                                        effects[i] = "<size=25><b><color=#696969>Exhausted</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Flashed":
                                        effects[i] = "<size=25><b><color=#d6fffd>Flashed</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Invigorated":
                                        effects[i] = "<size=25><b><color=green>Invigorated</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Poisoned":
                                        effects[i] = "<size=25><b><color=#02521e>Poisoned</color></b></size><size=10>\n\t</size>";
                                        break;
                                    //case "SinkHole":
                                    //    effects[i] = "<size=25><b><color=#02521e>Sink Hole</color></b></size><size=10>\n\t</size>";
                                    //    break;
                                    case "DamageReduction":
                                        effects[i] = "<size=25><b><color=#e3b76b>Damage Reduction</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "MovementBoost":
                                        effects[i] = "<size=25><b><color=#3bafd9>Movement Boost</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "RainbowTaste":
                                        effects[i] = "<size=25><b><color=#FF0000>R</color><color=#FF7F00>a</color><color=#FFFF00>i</color><color=#7FFF00>n</color><color=#00FF00>b</color><color=#00FF7F>o</color><color=#00FEFF>w</color><color=#007FFF> T</color><color=#0000FF>a</color><color=#7F00FF>s</color><color=#FF00FE>t</color><color=#FF007F>e</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "SeveredHands":
                                        effects[i] = "<size=25><b><color=red>Severed Hands</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Stained":
                                        effects[i] = "<size=25><b><color=#543601>Stained</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Vitality":
                                        effects[i] = "<size=25><b><color=#71c788>Vitality</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Hypothermia":
                                        effects[i] = "<size=25><b><color=#42e9ff>Hypothermia</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Scp1853":
                                        effects[i] = "<size=25><b><color=#c9f59a>SCP1853</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "CardiacArrest":
                                        effects[i] = "<size=25><b><color=red>Cardiac Arrest</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "AntiScp207":
                                        effects[i] = "<size=25><b><color=#fa70ff>Anti Scp207</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Invisible":
                                        effects[i] = "<size=25><b><color=#540042>Invisible</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Scp207":
                                        effects[i] = "<size=25><b><color=#bb80ff>Scp207</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "BodyshotReduction":
                                        effects[i] = "<size=25><b><color=#c9af3a>Bodyshot Reduction</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Hemorrhage":
                                        effects[i] = "<size=25><b><color=red>Hemorrhage</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Disabled":
                                        effects[i] = "<size=25><b><color=#828282>Disabled</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Corroding":
                                        effects[i] = "<size=25><b><color=#5e8c60>Corroding</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Concussed":
                                        effects[i] = "<size=25><b><color=#5e728c>Concussed</color></b></size><size=10>\n\t</size>";
                                        break;
                                    case "Scanned":
                                        effects[i] = "<size=25><b><color=#ffff00>Scanned</color></b></size><size=10>\n\t</size>";
                                        break;
                                }
                                Log.Debug($"{p.DisplayNickname} has effect {effects[i]} as player id {i}");
                            }
                            catch(Exception ex)
                            {
                                Log.Error(ex.ToString());
                            }
                        }
                    }
                    Log.Debug("Completed Message");
                    if (hint[p.Id].Length > 70)
                    {
                        p.ShowHint($"<size=15>\t</size>\n{hint[p.Id]}\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n<align=left>\t\n{effects[7] ?? "\t"}\n{effects[6] ?? "\t"}\n{effects[5] ?? "\t"}\n{effects[4] ?? "\t"}\n{effects[3] ?? "\t"}\n{effects[2] ?? "\t"}\n{effects[1] ?? "\t"}\n{effects[0] ?? "\t"}</align><align=center>\n\t\n\t\n\t\n<b><align=center><size=15><b><color=#B300FF>S</color><color=#AE15FF>p</color><color=#AA2AFF>i</color><color=#A63FFF>r</color><color=#A255FF>e</color><color=#9D6AFF>L</color><color=#997FFF>a</color><color=#9594FF>b</color><color=#91AAFF>s</color> <color=#88D4FF>-</color> </b>Discord.gg/f8uEpZWcBv/</size>\n\t\n\t\n\t\n\t\n\t\n\t\n\t", 1.25f);
                    }
                    else
                    {
                        p.ShowHint($"<size=15>{joinLeave}\t</size>\n{hint[p.Id]}\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n\t\n<align=left>\t\n{effects[7] ?? "\t"}\n{effects[6] ?? "\t"}\n{effects[5] ?? "\t"}\n{effects[4] ?? "\t"}\n{effects[3] ?? "\t"}\n{effects[2] ?? "\t"}\n{effects[1] ?? "\t"}\n{effects[0] ?? "\t"}</align><align=center>\n\t\n\t\n\t\n<b><align=center><size=15><b><color=#B300FF>S</color><color=#AE15FF>p</color><color=#AA2AFF>i</color><color=#A63FFF>r</color><color=#A255FF>e</color><color=#9D6AFF>L</color><color=#997FFF>a</color><color=#9594FF>b</color><color=#91AAFF>s</color> <color=#88D4FF>-</color> </b>Discord.gg/f8uEpZWcBv/</size>\n\t\n\t\n\t\n\t\n\t\n\t\n\t", 1.25f);
                    }
                    Log.Debug("Shown Hint");
                }
                if (killLoop) break;
            }
        }
    }
}
