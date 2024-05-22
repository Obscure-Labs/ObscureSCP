namespace ObscureLabs.API.Data
{
    public class HealthData
    {
        public HealthData(bool isEnabled, int health, int increase)
        {
            IsEnabled = isEnabled;
            Health = health;
            Increase = increase;
        }

        public HealthData() { }

        public bool IsEnabled { get; set; }

        public int Health { get; set; }

        public int Increase { get; set; }
    }
}
