namespace api.Models
{
    public class HandValue
    {
        public HandRank Rank { get; set; }
        public List<Rank> Kickers { get; set; } = new();
    }
}