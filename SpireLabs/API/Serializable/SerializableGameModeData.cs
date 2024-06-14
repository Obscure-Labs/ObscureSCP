namespace ObscureLabs.API.Data
{
    public class SerializableGameModeData
    {
        public bool IsGameModeRound { get; set; } = false;

        public int LastGameMode { get; set; } = -1;

        public bool IsNextRoundGameMode { get; set; } = false;
    }
}
