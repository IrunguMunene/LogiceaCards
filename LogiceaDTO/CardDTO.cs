using System;

namespace LogiceaDTO
{
    public class CardDTO
    {
        public CardDTO() 
        {
        }

        public CardDTO (int id)
        {
            Id = id;
        }
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
