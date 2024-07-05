namespace KiberHyron.Data
{
    public class RoleGame
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public uint Color { get; set; }
        public ulong Master { get; set; }
        public ulong Role { get; set; }
        public List<ulong> Players { get; set; } = new List<ulong>();
    }
}
