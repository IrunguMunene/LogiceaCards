using System.ComponentModel.DataAnnotations.Schema;

namespace LogiceaCardDomain
{
    public class Status
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
    }
}
