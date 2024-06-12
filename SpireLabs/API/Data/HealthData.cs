namespace ObscureLabs.API.Data
{
    public class HealthData
    {
        public HealthData(bool isEnabled, int health, int increase)
        {
            IsEnabled = isEnabled = false;
            Health = health = 0;
            Increase = increase = 0;
        }

        public HealthData() { }

        public bool IsEnabled { get; set; }

        public int Health { get; set; }

        public int Increase { get; set; }
    }
}
