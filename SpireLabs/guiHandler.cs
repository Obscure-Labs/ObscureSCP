using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpireLabs
{
    internal class guiHandler
    {
        internal static IEnumerator<float> sendHint(Player p, string h, int t)
        {
            hint[p.Id] = h;
            yield return Timing.WaitForSeconds(t);
            hint[p.Id] = string.Empty;
        }

        internal static string[] hint = new string[60];
        internal static IEnumerator<float> displayGUI(Player p)
        {
            hint[p.Id] = string.Empty;
            yield return Timing.WaitForSeconds(5f);
            Log.Info("Displaying GUI");
            while (true)
            {
                yield return Timing.WaitForSeconds(1f);
                Log.Info("Entered Loop");
                string msg = $"\n\n\n<b><align=left><size=20>";
                if (p.ActiveEffects == null || p.ActiveEffects.Count() == 0 || !p.IsAlive)
                {
                    Log.Info("No effects");
                    msg += $"<b><align=center><size=30>{hint[p.Id]}\n\n\n\n\n\n\n\n<b><align=center><size=15>SpireLabs - discord.gg/f8uEpZWcBv/";
                }
                else
                {
                    if (hint[p.Id] != string.Empty)
                    {
                        msg += $"<b><align=center><size=30>{hint[p.Id]}\n\n\n\n<b><align=left><size=20>";
                    }
                    else
                    {
                        msg += "\r\n\r\n\r\n\r\n\r\n";
                    }
                    int effectCount = 0;
                    Log.Info("Checking Effects");
                    for (int i = 0; i < p.ActiveEffects.Count(); i++)
                    {
                        effectCount++;
                        Log.Info("Checking Effect Type");
                        switch (p.ActiveEffects.ElementAt(i).name)
                        {
                            case "Asphyxiated":
                                msg += "<color=#95c700>Asphyxiated</color>\n";
                                break;
                            case "Bleeding":
                                msg += "<color=#750004>Bleeding</color>\n";
                                break;
                            case "Blinded":
                                msg += "<color=#394380>Blinded</color>\n";
                                break;
                            case "Burned":
                                msg += "<color=#de831b>Burned</color>\n";
                                break;
                            case "Deafened":
                                msg += "<color=#4a2700>Deafened</color>\n";
                                break;
                            case "Ensnared":
                                msg += "<color=#00750e>Ensnared</color>\n";
                                break;
                            case "Exhausted":
                                msg += "<color=#696969>Exhausted</color>\n";
                                break;
                            case "Flashed":
                                msg += "<color=#d6fffd>Flashed</color>\n";
                                break;
                            case "Invigorated":
                                msg += "<color=green>Invigorated</color>\n";
                                break;
                            case "Poisoned":
                                msg += "<color=#02521e>Poisoned</color>\n";
                                break;
                            case "SinkHole":
                                msg += "<color=#474747>SinkHole</color>\n";
                                break;
                            case "DamageReduction":
                                msg += "<color=#e3b76b>DamageReduction</color>\n";
                                break;
                            case "MovementBoost":
                                msg += "<color=#3bafd9>MovementBoost</color>\n";
                                break;
                            case "RainbowTaste":
                                msg += "<color=red>R</color><color=orange>a</color><color=yellow>i</color><color=green>n</color><color=blue>b</color><color=purple>o</color><color=red>w</color><color=orange>T</color><color=yellow>a</color><color=green>s</color><color=blue>t</color><color=purple>e</color>\n";
                                break;
                            case "SeveredHands":
                                msg += "<color=red>SeveredHands</color>\n";
                                break;
                            case "Stained":
                                msg += "<color=#543601>Stained</color>\n";
                                break;
                            case "Vitality":
                                msg += "<color=#71c788>Vitality</color>\n";
                                break;
                            case "Hypothermia":
                                msg += "<color=#42e9ff>Hypothermia</color>\n";
                                break;
                            case "Scp1853":
                                msg += "<color=#c9f59a>SCP1853</color>\n";
                                break;
                            case "CardiacArrest":
                                msg += "<color=red>CardiacArrest</color>\n";
                                break;
                            case "AntiScp207":
                                msg += "<color=#fa70ff>AntiScp207</color>\n";
                                break;
                            case "Invisible":
                                msg += "<color=#540042>Invisible</color>\n";
                                break;
                            case "Scp207":
                                msg += "<color=#bb80ff>Scp207</color>\n";
                                break;
                            case "BodyshotReduction":
                                msg += "<color=#c9af3a>BodyshotReduction</color>\n";
                                break;
                            case "Hemorrhage":
                                msg += "<color=red>Hemorrhage</color>\n";
                                break;
                            case "Disabled":
                                msg += "<color=#828282>Disabled</color>\n";
                                break;
                            case "Corroding":
                                msg += "<color=#5e8c60>Corroding</color>\n";
                                break;
                            case "Concussed":
                                msg += "<color=#5e728c>Concussed</color>\n";
                                break;
                            case "Scanned":
                                msg += "<color=#ffff00>Scanned</color>\n";
                                break;
                        }
                    }
                    msg += "\n<b><align=center><size=15>SpireLabs - https://discord.gg/f8uEpZWcBv/";
                }
                p.ShowHint(msg, 1.25f);
            }
        }
    }
}
