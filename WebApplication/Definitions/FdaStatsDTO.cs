namespace WebApplication.Definitions
{
    public class StatsDTO
    {
        public double Credits { get; set; }
        public double? Download { get; set; }
        public double? Processing { get; set; }
        public double? Upload { get; set; }
        public double? Queueing { get; set; }
    }
}
