using Exiled.API.Enums;

namespace ObscureLabs.API.Data
{
    public class EffectData
    {
        public EffectData(EffectType type, float duration)
        {
            Type = type;
            Duration = duration;
        }

        public EffectType Type { get; }

        public float Duration { get; }

        public string GetMessage()
        {
            switch (Type)
            {
                case EffectType.Asphyxiated:
                    return "<b><color=#95c700>Asphyxiated</color></b>";
                case EffectType.Bleeding:
                    return "<b><color=#750004>Bleeding</color></b>";
                case EffectType.Blinded:
                    return "<b><color=#394380>Blinded</color></b>";
                case EffectType.Burned:
                    return "<b><color=#de831b>Burned</color></b>";
                case EffectType.Deafened:
                    return "<b><color=#4a2700>Deafened</color></b>";
                case EffectType.Ensnared:
                    return "<b><color=#00750e>Ensnared</color></b>";
                case EffectType.Exhausted:
                    return "<b><color=#696969>Exhausted</color></b>>";
                case EffectType.Flashed:
                    return "<b><color=#d6fffd>Flashed</color></b>";
                case EffectType.Invigorated:
                    return "<b><color=green>Invigorated</color></b>";
                case EffectType.Poisoned:
                    return "<b><color=#02521e>Poisoned</color></b>";
                case EffectType.DamageReduction:
                    return "<b><color=#e3b76b>Damage Reduction</color></b>";
                case EffectType.MovementBoost:
                    return "<b><color=#3bafd9>Movement Boost</color></b>";
                case EffectType.RainbowTaste:
                    return "<b><color=#FF0000>R</color><color=#FF7F00>a</color><color=#FFFF00>i</color><color=#7FFF00>n</color><color=#00FF00>b</color><color=#00FF7F>o</color><color=#00FEFF>w</color><color=#007FFF> T</color><color=#0000FF>a</color><color=#7F00FF>s</color><color=#FF00FE>t</color><color=#FF007F>e</color></b>";
                case EffectType.SeveredHands:
                    return "<b><color=red>Severed Hands</color></b>";
                case EffectType.Stained:
                    return "<b><color=#543601>Stained</color></b>";
                case EffectType.Vitality:
                    return "<b><color=#71c788>Vitality</color></b>";
                case EffectType.Hypothermia:
                    return "<b><color=#42e9ff>Hypothermia</color></b>";
                case EffectType.Scp1853:
                    return "<b><color=#c9f59a>SCP1853</color></b>";
                case EffectType.CardiacArrest:
                    return "<b><color=red>Cardiac Arrest</color></b>";
                case EffectType.AntiScp207:
                    return "<b><color=#fa70ff>Anti Scp207</color></b>";
                case EffectType.Invisible:
                    return "<b><color=#540042>Invisible</color></b>";
                case EffectType.Scp207:
                    return "<b><color=#bb80ff>Scp207</color></b>";
                case EffectType.BodyshotReduction:
                    return "<b><color=#c9af3a>Bodyshot Reduction</color></b>";
                case EffectType.Hemorrhage:
                    return "<b><color=#ff0000>Hemorrhage</color></b>";
                case EffectType.Disabled:
                    return "<b><color=#828282>Disabled</color></b>";
                case EffectType.Corroding:
                    return "<b><color=#5e8c60>Corroding</color></b>";
                case EffectType.Concussed:
                    return "<b><color=#5e728c>Concussed</color></b>";
                case EffectType.Scanned:
                    return "<b><color=#ffff00>Scanned</color></b>";
                case EffectType.Ghostly:
                    return "<b><color=#f2f3f5>Ghostly</color></b>";
                case EffectType.Strangled:
                    return "<b><color=#687fad>Strangled</color></b>";
                case EffectType.SilentWalk:
                    return "<b><color=#a9a9a9>Silent Walk</color></b>";
            }

            return string.Empty;
        }
    }
}
