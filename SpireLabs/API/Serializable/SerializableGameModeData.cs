namespace ObscureLabs.API.Data
{
    public class SerializableGameModeData
    {
        public SerializableGameModeData(bool isGamemodeRound, int lastGamemode, bool isNextRoundGamemode)
        {
            IsGameModeRound = isGamemodeRound;
            LastGameMode = lastGamemode;
            IsNextRoundGameMode = isNextRoundGamemode;
        }

        public bool IsGameModeRound { get; set; }

        public int LastGameMode { get; set; }

        public bool? IsNextRoundGameMode { get; set; }
    }
}
