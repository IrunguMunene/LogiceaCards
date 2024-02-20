using System.Collections.Generic;

namespace LogiceaDTO
{
    public class PaginatedResult
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<CardDTO> Data { get; set; } = new List<CardDTO>();
    }
}
