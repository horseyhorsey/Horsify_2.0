namespace Horsesoft.Music.Data.Model
{
    public class HorsifyMenuJson
    {
        public HorsifyMenuJson[] Items { get; set; }
    }

    public class HorsifyMenu
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public HorsifyMenuItem[] Items { get; set; }
    }

    public class HorsifyMenuItem
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
    }
}
