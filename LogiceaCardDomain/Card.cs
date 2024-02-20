using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogiceaCardDomain
{
    public class Card
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
