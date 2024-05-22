using System.ComponentModel;

namespace ObscureLabs.Configs
{
    public class HintConfig
    {
        [Description("Hint height")]
        public int HintHeight { get; set; } = -4;

        [Description("Time between hints")]
        public int TimeBetweenHints { get; set; } = 120;
    }
}
