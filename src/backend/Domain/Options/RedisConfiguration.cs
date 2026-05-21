namespace Domain.Data.Configurations
{
    public class RedisConfiguration
    {
        public bool Enabled { get; set; }
        public string ConnectionStrings { get; set; } = string.Empty;
        public string Connection { get; set; } = string.Empty;
        public string EffectiveConnectionString =>
            string.IsNullOrWhiteSpace(ConnectionStrings) ? Connection : ConnectionStrings;
    }
}
