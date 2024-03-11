namespace HomeWorkOTUS.Models.Clients
{
    public class Client
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string SerName { get; set; }

        public int Age { get; set; }

        public bool IsMale { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
