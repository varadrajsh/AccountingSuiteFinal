namespace AccountingSuite.Models.Master
{
    public class State
    {
        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;
        public int RegionId { get; set; }

        // Optional navigation property
        public Region? Region { get; set; }
    }
}
