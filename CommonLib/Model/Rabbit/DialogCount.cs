namespace CommonLib.Model.Rabbit
{
    public class DialogCount
    {
        public Guid To { get; set; }
        public Guid From { get; set; }
        public bool IsCreated { get; set; }
        public int[]? DialogsId { get; set; }
        public int? LastRead { get; set; }
    }
}
