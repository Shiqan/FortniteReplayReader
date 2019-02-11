namespace FortniteReplayReader.Core.Models
{
    public class PlayerElimination
    {
        public string Eliminated { get; set; }
        public string Eliminator { get; set; }
        public GunType GunType { get; set; }
        public string Time { get; set; }
        public bool Knocked { get; set; }
    }
}