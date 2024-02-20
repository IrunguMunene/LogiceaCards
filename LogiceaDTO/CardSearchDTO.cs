using System;
using System.Collections.Generic;

namespace LogiceaDTO
{
    public class CardSearchDTO
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<string> SortBy { get; set; }
    }
}
